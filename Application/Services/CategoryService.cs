using Application.DTOs;
using Application.Interfaces;
using AutoMapper;
using Domain.Entities;

namespace Application.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IImageStorageService _imageStorageService;

        public CategoryService(IUnitOfWork unitOfWork, IMapper mapper, IImageStorageService imageStorageService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _imageStorageService = imageStorageService;
        }
        public async Task<CategoryDto> CreateAsync(CreateCategoryDto dto)
        {
            string imageUrl = "";
            try
            {
                var category = _mapper.Map<Category>(dto);

                var extension = Path.GetExtension(dto.ImageName);
                var imageName = $"main{extension}";

                var folderId = Guid.NewGuid();
                category.ImageFolderId = folderId;
                imageUrl = await _imageStorageService.UploadAsync(dto.Image, $"Categories/{folderId}/images",
                                                                  imageName, dto.ContentType);
                category.MainImageUrl = imageUrl;
                await _unitOfWork.Categories.AddAsync(category);
                await _unitOfWork.SaveChangesAsync();
                return _mapper.Map<CategoryDto>(category);
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

        public async Task<bool> DeleteAsync(int id)
        {
            var category = await _unitOfWork.Categories.GetByIdAsync(id);

            if (category is null)
                return false;

            try
            {
                await _imageStorageService.DeleteAsync(category.MainImageUrl);

                _unitOfWork.Categories.Delete(category);

                await _unitOfWork.SaveChangesAsync();

                return true;
            }
            catch
            {
                throw;
            }
        }

        public async Task<IEnumerable<CategoryDto>> GetAllAsync()
        {
            var categories = await _unitOfWork.Categories.GetAllAsync();
            return _mapper.Map<IEnumerable<CategoryDto>>(categories);
        }

        public async Task<CategoryDto?> GetByIdAsync(int id)
        {
            var category = await _unitOfWork.Categories.GetByIdAsync(id);
            return category is null ? null : _mapper.Map<CategoryDto>(category);
        }

        public async Task<bool> UpdateAsync(int id, UpdateCategoryDto dto)
        {
            string imageUrl = "";
            try
            {

                var category = await _unitOfWork.Categories.GetByIdAsync(id);

                if (category is null) return false;

                var oldImageUrl = category.MainImageUrl;

                var extension = Path.GetExtension(dto.ImageName);
                var imageName = $"main{extension}";

                imageUrl = await _imageStorageService.UploadAsync(dto.Image, $"Categories/{category.ImageFolderId}/images",
                                                                      imageName, dto.ContentType);

                category.MainImageUrl = imageUrl;

                _mapper.Map(dto, category);
                _unitOfWork.Categories.Update(category);
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
