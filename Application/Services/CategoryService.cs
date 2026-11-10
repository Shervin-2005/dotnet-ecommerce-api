using Application.DTOs.Category;
using Application.Interfaces.Category;
using Application.Interfaces.Common;
using AutoMapper;
using Domain.Entities;
using Domain.Exceptions;

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
                await ValidateParentCategoryAsync(dto.ParentCategoryId);
                
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
            
            await _imageStorageService.DeleteAsync(category.MainImageUrl);

            _unitOfWork.Categories.Delete(category);

            await _unitOfWork.SaveChangesAsync();
                
            return true;
        }

        public async Task<IEnumerable<CategoryDto>> GetAllAsync()
        {
            var categories = await _unitOfWork.Categories.GetAllAsync();
            return _mapper.Map<IEnumerable<CategoryDto>>(categories);
        }

        public async Task<IEnumerable<CategoryTreeDto>> GetTreeAsync()
        {
            var categories = await _unitOfWork.Categories.GetAllAsync();

            var categoryDtos = categories.ToDictionary(
                c => c.CategoryId,
                c => new CategoryTreeDto
                {
                    CategoryId = c.CategoryId,
                    CategoryName = c.CategoryName,
                    MainImageUrl = c.MainImageUrl
                });

            var rootCategories = new List<CategoryTreeDto>();

            foreach (var category in categories)
            {
                var currentDto = categoryDtos[category.CategoryId];

                if (category.ParentCategoryId is null)
                {
                    rootCategories.Add(currentDto);
                }
                else if (categoryDtos.TryGetValue(category.ParentCategoryId.Value, out var parentDto))
                {
                    parentDto.SubCategories.Add(currentDto);
                }
            }

            return rootCategories;
        }
        
        public async Task<CategoryDto?> GetByIdAsync(int id)
        {
            var category = await _unitOfWork.Categories.GetByIdAsync(id);
            return category is null ? null : _mapper.Map<CategoryDto>(category);
        }
        public async Task<bool> UpdateAsync(int id, UpdateCategoryDto dto)
        {
            string? newImageUrl = null;

            var category = await _unitOfWork.Categories.GetByIdAsync(id);

            if (category is null) 
                return false;
            
            await ValidateParentCategoryAsync(dto.ParentCategoryId, id);

            var oldImageUrl = category.MainImageUrl;
            
            try
            {
                category.CategoryName = dto.CategoryName;
                category.ParentCategoryId = dto.ParentCategoryId;
                
                if (dto.Image is not null)
                {
                    var extension = Path.GetExtension(dto.ImageName);
                    var imageName = $"main{extension}";

                    newImageUrl = await _imageStorageService.UploadAsync(dto.Image, $"Categories/{category.ImageFolderId}/images", imageName, dto.ContentType!);

                    category.MainImageUrl = newImageUrl;
                }

                _unitOfWork.Categories.Update(category);

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
        private async Task ValidateParentCategoryAsync(int? parentCategoryId)
        {
            if (parentCategoryId is null)
                return;

            var parent = await _unitOfWork.Categories.GetByIdAsync(parentCategoryId.Value);

            if (parent is null)
                throw new NotFoundException("Parent category not found.");
        }
        private async Task ValidateParentCategoryAsync(int? parentCategoryId, int categoryId)
        {
            if (parentCategoryId is null)
                return;

            var categories = await _unitOfWork.Categories.GetAllAsync();

            var categoryDictionary = categories.ToDictionary(c => c.CategoryId);

            if (!categoryDictionary.ContainsKey(parentCategoryId.Value))
                throw new NotFoundException("Parent category not found.");

            var currentParentId = parentCategoryId;

            while (currentParentId is not null)
            {
                if (currentParentId == categoryId)
                    throw new BadRequestException("A category cannot be moved under one of its descendants.");

                if (!categoryDictionary.TryGetValue(currentParentId.Value, out var parent))
                    break;

                currentParentId = parent.ParentCategoryId;
            }
        }
    }
}
