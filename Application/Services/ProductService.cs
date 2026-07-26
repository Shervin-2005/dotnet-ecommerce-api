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
            var product = await _unitOfWork.Products.GetByIdAsync(id);
            if (product is null) return false;

            _unitOfWork.Products.Delete(product);
            await _unitOfWork.SaveChangesAsync();
            return true;
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
    }
}
