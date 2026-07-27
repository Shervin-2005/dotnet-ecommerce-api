using Application.DTOs;
using Application.Interfaces;
using AutoMapper;
using Domain.Entities;

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
                
                return _mapper.Map<ProductDto>(product);
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

            try
            {
                foreach(var image in product.Images)
                {
                    await _imageStorageService.DeleteAsync(image.ImageUrl);
                }

                _unitOfWork.Products.Delete(product);
                await _unitOfWork.SaveChangesAsync();
                return true;

            }
            catch
            {
                throw;
            }
        }

        public async Task<IEnumerable<ProductDto>> GetAllAsync()
        {
            var products = await _unitOfWork.Products.GetAllAsync();
            return _mapper.Map<IEnumerable<ProductDto>>(products);
        }

        public async Task<ProductDto?> GetByIdAsync(int id)
        {
            var product = await _unitOfWork.Products.GetWithDetailsAsync(id);
            return product is null ? null : _mapper.Map<ProductDto>(product);
        }

        public async Task<bool> UpdateAsync(int id, UpdateProductDto dto)
        {
            var product = await _unitOfWork.Products.GetByIdAsync(id);
            if (product is null) return false;

            _mapper.Map(dto, product);
            _unitOfWork.Products.Update(product);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }
       
        public async Task<bool> AddImageAsync(int id, ProductImageDto dto)
        {
            var product = await _unitOfWork.Products.GetWithDetailsAsync(id);
            if (product is null) return false;

            string? imageUrl = null;
            try
            {
                await using var image = dto.Image;

                var targetOrder = Math.Clamp(dto.DisplayOrder, 0, product.Images.Count);
                foreach (var existing in product.Images.Where(i => i.DisplayOrder >= targetOrder))
                    existing.DisplayOrder += 1;

                var folderId = product.ImageFolderId;
                var extension = Path.GetExtension(dto.ImageName);
                var imageName = $"{Guid.NewGuid()}{extension}";

                imageUrl = await _imageStorageService.UploadAsync(
                    image, $"Products/{product.ImageFolderId}", imageName, dto.ContentType);

                // Only one image can be main
                if (dto.IsMain)
                {
                    foreach (var existing in product.Images)
                        existing.IsMain = false;
                }

                var productImage = new ProductImage
                {
                    ProductId = product.ProductId,
                    ImageUrl = imageUrl,
                    IsMain = dto.IsMain,
                    DisplayOrder = dto.DisplayOrder
                };

                await _unitOfWork.ProductImages.AddAsync(productImage);
                await _unitOfWork.SaveChangesAsync();

                _mapper.Map<ProductImageDto>(productImage);

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

            try
            {
                await _imageStorageService.DeleteAsync(deletedUrl);
            }
            catch
            {
                throw;
            }

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

    }
}
