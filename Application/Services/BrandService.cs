using Application.DTOs;
using Application.Interfaces;
using AutoMapper;
using Domain.Entities;

namespace Application.Services
{
    public class BrandService : IBrandService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IImageStorageService _imageStorageService;

        public BrandService(IUnitOfWork unitOfWork, IMapper mapper, IImageStorageService imageStorageService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _imageStorageService = imageStorageService;
        }

        public async Task<BrandDto> CreateAsync(CreateBrandDto dto)
        {
            string imageUrl = "";
            try
            {
                var brand = _mapper.Map<Brand>(dto);

                var extension = Path.GetExtension(dto.ImageName);
                var imageName = $"main{extension}";

                var folderId = Guid.NewGuid();
                brand.ImageFolderId = folderId;
                imageUrl = await _imageStorageService.UploadAsync(dto.Image, $"Brands/{folderId}/images",
                                                                  imageName, dto.ContentType);
                brand.MainImageUrl = imageUrl;

                await _unitOfWork.Brands.AddAsync(brand);
                await _unitOfWork.SaveChangesAsync();
                return _mapper.Map<BrandDto>(brand);
            }
            catch (Exception)
            {
                if (!string.IsNullOrEmpty(imageUrl))
                {
                    await _imageStorageService.DeleteAsync(imageUrl);
                }
                throw;
            }
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var brand = await _unitOfWork.Brands.GetByIdAsync(id);
            if (brand is null) return false;

            try
            {
                await _imageStorageService.DeleteAsync(brand.MainImageUrl);

                _unitOfWork.Brands.Delete(brand);
                await _unitOfWork.SaveChangesAsync();
                return true;
            }
            catch
            {

                throw;
            }    
        }

        public async Task<IEnumerable<BrandDto>> GetAllAsync()
        {
            var brands = await _unitOfWork.Brands.GetAllAsync();
            return _mapper.Map<IEnumerable<BrandDto>>(brands);
        }

        public async Task<BrandDto?> GetByIdAsync(int id)
        {
            var brand = await _unitOfWork.Brands.GetByIdAsync(id);
            return brand is null ? null : _mapper.Map<BrandDto>(brand);
        }

        public async Task<bool> UpdateAsync(int id, UpdateBrandDto dto)
        {
            string imageUrl = "";
            try
            {
                var brand = await _unitOfWork.Brands.GetByIdAsync(id);
                if (brand is null) return false;

                var oldImageUrl = brand.MainImageUrl;

                var extension = Path.GetExtension(dto.ImageName);
                var imageName = $"main{extension}";

                imageUrl = await _imageStorageService.UploadAsync(dto.Image, $"Brands/{brand.ImageFolderId}/images",
                                                                      imageName, dto.ContentType);
                brand.MainImageUrl = imageUrl;

                _mapper.Map(dto, brand);
                _unitOfWork.Brands.Update(brand);
                await _unitOfWork.SaveChangesAsync();

                if (!string.Equals(oldImageUrl, imageUrl, StringComparison.OrdinalIgnoreCase))
                {
                    await _imageStorageService.DeleteAsync(oldImageUrl);
                }

                return true;
            }
            catch
            {
                if (!string.IsNullOrEmpty(imageUrl))
                {
                    await _imageStorageService.DeleteAsync(imageUrl);
                }
                throw;
            }
            
        }
    }
}
