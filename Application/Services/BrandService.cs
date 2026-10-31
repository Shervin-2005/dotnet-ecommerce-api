using Application.DTOs.Brand;
using Application.Interfaces;
using Application.Interfaces.Brand;
using AutoMapper;
using Domain.Entities;
using Domain.Exceptions;

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
                var brandGroup = await _unitOfWork.BrandGroups.GetByIdAsync(dto.BrandGroupId);

                if (brandGroup is null)
                    throw new NotFoundException("Brand group not found.");
                
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
            
            await _imageStorageService.DeleteAsync(brand.MainImageUrl);

            _unitOfWork.Brands.Delete(brand);
            await _unitOfWork.SaveChangesAsync();
            return true;

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
            string? newImageUrl = null;

            var brand = await _unitOfWork.Brands.GetByIdAsync(id);

            if (brand is null) 
                return false;
            
            if (dto.BrandGroupId.HasValue)
            {
                var brandGroup = await _unitOfWork.BrandGroups.GetByIdAsync(dto.BrandGroupId.Value);

                if (brandGroup is null)
                    throw new NotFoundException("Brand group not found.");

                brand.BrandGroupId = dto.BrandGroupId.Value;
            }

            var oldImageUrl = brand.MainImageUrl;
            
            try
            {
                brand.BrandName= dto.BrandName;
                
                if (dto.Image is not null)
                {
                    var extension = Path.GetExtension(dto.ImageName);
                    var imageName = $"main{extension}";

                    newImageUrl = await _imageStorageService.UploadAsync(dto.Image, $"Brands/{brand.ImageFolderId}/images", imageName, dto.ContentType!);

                    brand.MainImageUrl = newImageUrl;
                }

                _unitOfWork.Brands.Update(brand);

                await _unitOfWork.SaveChangesAsync();

                if (newImageUrl is not null && !string.IsNullOrEmpty(oldImageUrl) && !string.Equals(oldImageUrl, newImageUrl, StringComparison.OrdinalIgnoreCase))
                {
                    await _imageStorageService.DeleteAsync(oldImageUrl);
                }

                return true;
            }
            catch
            {
                if (newImageUrl is not null)
                {
                    await _imageStorageService.DeleteAsync(newImageUrl);
                }
                throw;
            }
            
        }
    }
}
