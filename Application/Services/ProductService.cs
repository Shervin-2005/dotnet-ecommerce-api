using Application.DTOs.Product;
using Application.Interfaces;
using AutoMapper;
using Domain.Entities;
using Domain.Exceptions;

namespace Application.Services
{
    public class ProductService : IProductService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IImageStorageService _imageStorageService;

        public ProductService(IUnitOfWork unitOfWork, IMapper mapper, IImageStorageService imageStorageService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _imageStorageService = imageStorageService;
        }

        public async Task<ProductDto> CreateAsync(CreateProductDto dto)
        {
            var ImageUrls = new List<string>();

            try
            {
                var product = _mapper.Map<Product>(dto);
                product.SalePrice = dto.SalePrice ?? dto.OriginalPrice;

                product.ImageFolderId = Guid.NewGuid();
                int displayOrder = 1;

                foreach (var image in dto.Images)
                {
                    var extension = Path.GetExtension(image.ImageName);

                    var fileName = $"{displayOrder}{extension}";

                    var imageUrl = await _imageStorageService.UploadAsync(
                        image.Image,
                        $"Products/{product.ImageFolderId}",
                        fileName,
                        image.ContentType);

                    ImageUrls.Add(imageUrl);

                    product.Images.Add(new ProductImage
                    {
                        ImageUrl = imageUrl,
                        IsMain = displayOrder == 1,
                        DisplayOrder = displayOrder
                    });

                    displayOrder++;
                }
                await _unitOfWork.Products.AddAsync(product);

                await _unitOfWork.SaveChangesAsync();
                
                var createdProduct = await _unitOfWork.Products.GetWithDetailsAsync(
                    product.ProductId);
                
                return _mapper.Map<ProductDto>(createdProduct);
            }
            catch
            {
                foreach (var imageUrl in ImageUrls)
                {
                    await _imageStorageService.DeleteAsync(imageUrl);

                }

                throw;
            } 
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var product = await _unitOfWork.Products.GetWithDetailsAsync(id);
            if (product is null) return false;
            
                foreach(var image in product.Images)
                {
                    await _imageStorageService.DeleteAsync(image.ImageUrl);
                }

            _unitOfWork.Products.Delete(product);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<ProductDto>> GetAllAsync()
        {
            var products = await _unitOfWork.Products.GetAllWithDetailsAsync();
            var productList = products.ToList();

            var dtos = _mapper.Map<List<ProductDto>>(productList);
            
            var summaries = await _unitOfWork.Reviews.GetRatingSummariesAsync(productList.Select(p => p.ProductId));
            foreach (var dto in dtos)
            {
                if (summaries.TryGetValue(dto.ProductId, out var summary))
                {
                    dto.AverageRating = Math.Round(summary.AverageRating, 1);
                    dto.ReviewCount = summary.Count;
                }
            }

            return dtos;
        }

        public async Task<ProductDto?> GetByIdAsync(int id)
        {
            var product = await _unitOfWork.Products.GetWithDetailsAsync(id);
            if (product is null) return null;

            var dto = _mapper.Map<ProductDto>(product);

            var summaries = await _unitOfWork.Reviews.GetRatingSummariesAsync(new[] { id });
            if (summaries.TryGetValue(id, out var summary))
            {
                dto.AverageRating = Math.Round(summary.AverageRating, 1);
                dto.ReviewCount = summary.Count;
            }

            return dto;
        }

        public async Task<bool> UpdateAsync(int id, UpdateProductDto dto)
        {
            var product = await _unitOfWork.Products.GetWithDetailsAsync(id);
            if (product is null) return false;

            _mapper.Map(dto, product);
            product.SalePrice = dto.SalePrice ?? dto.OriginalPrice;
            _unitOfWork.Products.Update(product);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }
       
        public async Task<bool> AddImageAsync(int id, ProductImageUploadDto uploadDto)
        {
            var product = await _unitOfWork.Products.GetWithDetailsAsync(id);
            if (product is null) return false;

            string? imageUrl = null;
            try
            {
                await using var image = uploadDto.Image;

                var targetOrder = Math.Clamp(uploadDto.DisplayOrder, 0, product.Images.Count);
                foreach (var existing in product.Images.Where(i => i.DisplayOrder >= targetOrder))
                    existing.DisplayOrder += 1;

                var folderId = product.ImageFolderId;
                var extension = Path.GetExtension(uploadDto.ImageName);
                var imageName = $"{Guid.NewGuid()}{extension}";

                imageUrl = await _imageStorageService.UploadAsync(
                    image, $"Products/{product.ImageFolderId}", imageName, uploadDto.ContentType);

                // Only one image can be main
                if (uploadDto.IsMain)
                {
                    foreach (var existing in product.Images)
                        existing.IsMain = false;
                }

                var productImage = new ProductImage
                {
                    ProductId = product.ProductId,
                    ImageUrl = imageUrl,
                    IsMain = uploadDto.IsMain,
                    DisplayOrder = uploadDto.DisplayOrder
                };

                await _unitOfWork.ProductImages.AddAsync(productImage);
                await _unitOfWork.SaveChangesAsync();

                _mapper.Map<ProductImageUploadDto>(productImage);

                return true;
            }
            catch
            {
                // Upload succeeded but the DB write failed — don't leave an orphaned file in S3.
                if (!string.IsNullOrEmpty(imageUrl))
                    await _imageStorageService.DeleteAsync(imageUrl);
                throw;
            }
        }
        public async Task<bool> RemoveImageAsync(int productId, int imageId)
        {
            var product = await _unitOfWork.Products.GetWithDetailsAsync(productId);
            if (product is null) return false;

            var image = product.Images.FirstOrDefault(i => i.ImageId == imageId);
            if (image is null) return false;

            var removedOrder = image.DisplayOrder;

            var wasMain = image.IsMain;
            var deletedUrl = image.ImageUrl;

            _unitOfWork.ProductImages.Delete(image);

            foreach (var remaining in product.Images.Where(i => i.ImageId != imageId && i.DisplayOrder > removedOrder))
                 remaining.DisplayOrder -= 1;

            await _unitOfWork.SaveChangesAsync();
            
            await _imageStorageService.DeleteAsync(deletedUrl);
            
            if (wasMain)
            {
                var nextMain = product.Images
                    .Where(i => i.ImageId != imageId)
                    .OrderBy(i => i.DisplayOrder)
                    .FirstOrDefault();

                if (nextMain is not null)
                {
                    nextMain.IsMain = true;
                    await _unitOfWork.SaveChangesAsync();
                }
            }

            return true;
        }
        public async Task<bool> SetMainImageAsync(int productId, int imageId)
        {
            var product = await _unitOfWork.Products.GetWithDetailsAsync(productId);
            if (product is null) return false;

            var target = product.Images.FirstOrDefault(i => i.ImageId == imageId);
            if (target is null) return false;

            foreach (var image in product.Images)
                image.IsMain = image.ImageId == imageId;

            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        public async Task<bool> SetSpecificationsAsync(int productId, List<UpsertProductSpecificationDto> specs)
        {
            var product = await _unitOfWork.Products.GetWithDetailsAsync(productId);
            if (product is null) return false;

            if (specs.Count > 30)
                throw new BadRequestException("A product can have a maximum of 30 specifications.");

            if (specs.Count(x => x.IsMain) > 10)
                throw new BadRequestException("A product can have a maximum of 10 main specifications.");
            
            foreach (var existing in product.Specifications.ToList())
                _unitOfWork.ProductSpecifications.Delete(existing);

            foreach (var spec in specs)
            {
                await _unitOfWork.ProductSpecifications.AddAsync(new ProductSpecification
                {
                    ProductId = productId,
                    Key = spec.Key,
                    Value = spec.Value,
                    DisplayOrder = spec.DisplayOrder,
                    IsMain = spec.IsMain
                });
            }

            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        public async Task<bool> SetAttributesAsync(int productId, SetProductAttributesDto dto)
        {
            var product = await _unitOfWork.Products.GetWithDetailsAsync(productId);

            if (product is null)
                return false;

            if (product.Variants.Any())
                throw new BadRequestException("Product attributes cannot be changed while variants exist.");

            foreach (var existing in product.Attributes.ToList())
            {
                _unitOfWork.ProductAttributes.Delete(existing);
            }

            foreach (var attributeDto in dto.Attributes)
            {
                var attribute = new ProductAttribute
                {
                    ProductId = productId,
                    Name = attributeDto.Name.Trim()
                };

                foreach (var value in attributeDto.Values)
                {
                    attribute.Values.Add(new ProductAttributeValue
                    {
                        Value = value.Trim()
                    });
                }

                await _unitOfWork.ProductAttributes.AddAsync(attribute);
            }

            await _unitOfWork.SaveChangesAsync();

            return true;
        }
    }
}
