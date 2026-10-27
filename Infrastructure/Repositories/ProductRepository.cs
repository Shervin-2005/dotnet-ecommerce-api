using Application.DTOs.Common;
using Application.DTOs.Product;
using Application.Interfaces.Product;
using Domain.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class ProductRepository : GenericRepository<Product>, IProductRepository
    {
        public ProductRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Product>> GetByCategoryAsync(int categoryId) =>
            await _dbSet.Where(p => p.CategoryId == categoryId).ToListAsync();

        public async Task<Product?> GetWithDetailsAsync(int id) =>
            await _dbSet
                .Include(p => p.Category)
                .Include(p => p.Brand)
                .Include(p => p.Images)
                .Include(p => p.Specifications)
                .Include(p => p.Attributes)
                .ThenInclude(a => a.Values)
                .Include(p => p.Variants)
                .ThenInclude(v => v.AttributeValues)
                .ThenInclude(av => av.ProductAttribute)
                .FirstOrDefaultAsync(p => p.ProductId == id);
        
        public async Task<IEnumerable<Product>> GetAllWithDetailsAsync()
        {
            return await _context.Products
                .Include(p => p.Category)
                .Include(p => p.Brand)
                .Include(p => p.Images)
                .Include(p => p.Specifications)
                .Include(p => p.Attributes)
                .ThenInclude(a => a.Values)
                .Include(p => p.Variants)
                .ThenInclude(v => v.AttributeValues)
                .ThenInclude(av => av.ProductAttribute)
                .ToListAsync();
        } 
        
        public async Task<PagedResult<Product>> SearchAsync(ProductSearchRequest request, CancellationToken cancellationToken = default) 
        { 
            var now = DateTime.UtcNow;
            
            var hasSearch = !string.IsNullOrWhiteSpace(request.Search);
            
            var escapedSearchTerm = hasSearch ? EscapeLikePattern(request.Search!.Trim()) : null;
            
            IQueryable<Product> query = _context.Products.AsNoTracking();
            
            if (hasSearch) 
            { 
                var pattern = $"%{escapedSearchTerm}%";
                
                query = query.Where(p => 
                    EF.Functions.ILike(p.ProductName, pattern, "\\") ||
                    EF.Functions.ILike(p.Description, pattern, "\\") || 
                    (p.Brand != null && EF.Functions.ILike(p.Brand.BrandName, pattern, "\\")) ||
                     (p.Category != null && EF.Functions.ILike(p.Category.CategoryName, pattern, "\\")) ||
                     p.Variants.Any(v => EF.Functions.ILike(v.Sku, pattern, "\\"))); 
            }
            
            //Filter by Category
            if(request.CategoryId.HasValue)
                query = query.Where(p => p.CategoryId == request.CategoryId.Value); 
            
            
            //Filter by Brand
            if (request.BrandId.HasValue) 
                query = query.Where(p => p.BrandId == request.BrandId.Value); 
            

            //Calculate relevance and effective price in SQL.
            var exactPattern = escapedSearchTerm;
            var startsWithPattern = hasSearch ? $"{escapedSearchTerm}%" : null;
            var containsPattern = hasSearch ? $"%{escapedSearchTerm}%" : null;
            
            var pricedQuery = query.Select(p => new 
            { 
                Product = p,
                Relevance = hasSearch 
                    ? EF.Functions.ILike(p.ProductName, exactPattern!, "\\") ? 100 
                    : EF.Functions.ILike(p.ProductName, startsWithPattern!, "\\") ? 80
                    : EF.Functions.ILike(p.ProductName, containsPattern!, "\\") ? 60
                    : p.Brand != null && EF.Functions.ILike(p.Brand.BrandName, containsPattern!, "\\") ? 40
                    : p.Category != null && EF.Functions.ILike(p.Category.CategoryName, containsPattern!, "\\") ? 30
                    : EF.Functions.ILike(p.Description, containsPattern!, "\\") ? 20
                    : p.Variants.Any(v => EF.Functions.ILike(v.Sku, containsPattern!, "\\")) ? 10 : 0 : 0,
               
                EffectivePrice = p.Variants.Any() 
                    ? p.Variants.Min(v => 
                        v.SaleStartAt.HasValue &&
                        v.SaleEndAt.HasValue && 
                        v.SaleStartAt.Value <= now && 
                        now < v.SaleEndAt.Value && 
                        v.SalePrice < v.OriginalPrice ? v.SalePrice : v.OriginalPrice) 
                    : p.SaleStartAt.HasValue && 
                      p.SaleEndAt.HasValue &&
                      p.SaleStartAt.Value <= now &&
                      now < p.SaleEndAt.Value &&
                      p.SalePrice < p.OriginalPrice
                    ? p.SalePrice
                    : p.OriginalPrice 
            });

    // Filter by effective price.
    if (request.MinPrice.HasValue)
        pricedQuery = pricedQuery.Where(x => x.EffectivePrice >= request.MinPrice.Value);
    
    if (request.MaxPrice.HasValue)
        pricedQuery = pricedQuery.Where(x => x.EffectivePrice <= request.MaxPrice.Value);
    
    var totalCount = await pricedQuery.CountAsync(cancellationToken);
    
    var sortByPrice = string.Equals(request.SortBy, "price", StringComparison.OrdinalIgnoreCase);

    var sortByName = string.Equals(request.SortBy, "name", StringComparison.OrdinalIgnoreCase);

    var descending = string.Equals(request.SortOrder, "desc", StringComparison.OrdinalIgnoreCase);

    var pageQuery = sortByPrice 
        ? descending 
            ? pricedQuery
                .OrderByDescending(x => x.EffectivePrice)
                .ThenBy(x => x.Product.ProductId) 
            : pricedQuery
                .OrderBy(x => x.EffectivePrice)
                .ThenBy(x => x.Product.ProductId)
        : sortByName || !hasSearch
            ? descending 
                ? pricedQuery
                    .OrderByDescending(x => x.Product.ProductName)
                    .ThenBy(x => x.Product.ProductId)
                : pricedQuery
                    .OrderBy(x => x.Product.ProductName)
                    .ThenBy(x => x.Product.ProductId)
            
            : pricedQuery
                .OrderByDescending(x => x.Relevance)
                .ThenBy(x => x.Product.ProductName)
                .ThenBy(x => x.Product.ProductId);

    // Fetch IDs for the requested page.
    var skip = (int)Math.Min(((long)request.Page - 1) * request.PageSize, int.MaxValue);

    var pageIds = await pageQuery
        .Skip(skip)
        .Take(request.PageSize)
        .Select(x => x.Product.ProductId)
        .ToListAsync(cancellationToken);

    // Load details only for products on this page.
    var products = await _context.Products
        .AsNoTracking()
        .AsSplitQuery()
        .Where(p => pageIds.Contains(p.ProductId))
        .Include(p => p.Category)
        .Include(p => p.Brand)
        .Include(p => p.Images)
        .Include(p => p.Specifications)
        .Include(p => p.Attributes)
            .ThenInclude(a => a.Values)
        .Include(p => p.Variants)
            .ThenInclude(v => v.AttributeValues)
                .ThenInclude(av => av.ProductAttribute)
        .ToListAsync(cancellationToken);

    // Preserve the order selected by the database.
    var positions = pageIds
        .Select((id, index) => new { id, index })
        .ToDictionary(x => x.id, x => x.index);

    var orderedProducts = products
        .OrderBy(p => positions[p.ProductId])
        .ToList();

    return new PagedResult<Product>
    {
        Items = orderedProducts,
        Page = request.Page,
        PageSize = request.PageSize,
        TotalCount = totalCount
    }; 
        }
        
        private static string EscapeLikePattern(string value) 
        { 
            return value
                .Replace("\\", "\\\\")
                .Replace("%", "\\%")
                .Replace("_", "\\_"); 
        } 
    } 
}
