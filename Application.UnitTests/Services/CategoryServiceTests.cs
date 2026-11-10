using Application.DTOs.Category;
using Application.Interfaces.Category;
using Application.Interfaces.Common;
using Application.Services;
using AutoMapper;
using Domain.Entities;
using Domain.Exceptions;
using Moq;

namespace Application.UnitTests.Services;

public class CategoryServiceTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<ICategoryRepository> _categoryRepositoryMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<IImageStorageService> _imageStorageServiceMock;

    private readonly CategoryService _categoryService;

    public CategoryServiceTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _categoryRepositoryMock = new Mock<ICategoryRepository>();
        _mapperMock = new Mock<IMapper>();
        _imageStorageServiceMock = new Mock<IImageStorageService>();

        _unitOfWorkMock
            .Setup(u => u.Categories)
            .Returns(_categoryRepositoryMock.Object);

        _categoryService = new CategoryService(
            _unitOfWorkMock.Object,
            _mapperMock.Object,
            _imageStorageServiceMock.Object);
    }
    
    [Fact]
    public async Task GetAllAsync_WhenCategoriesExist_ReturnsMappedDtos()
    {
        // Arrange
        var categories = new List<Category>
        {
            new()
            {
                CategoryId = 1,
                CategoryName = "Coffee",
                MainImageUrl = "coffee.jpg"
            },
            new()
            {
                CategoryId = 2,
                CategoryName = "Tea",
                MainImageUrl = "tea.jpg"
            }
        };

        var expectedDtos = new List<CategoryDto>
        {
            new()
            {
                CategoryId = 1,
                CategoryName = "Coffee",
                MainImageUrl = "coffee.jpg"
            },
            new()
            {
                CategoryId = 2,
                CategoryName = "Tea",
                MainImageUrl = "tea.jpg"
            }
        };

        _categoryRepositoryMock.Setup(r => r.GetAllAsync()).ReturnsAsync(categories);

        _mapperMock.Setup(m => m.Map<IEnumerable<CategoryDto>>(categories)).Returns(expectedDtos);

        // Act
        var result = await _categoryService.GetAllAsync();

        // Assert
        Assert.Equal(2, result.Count());
        Assert.Equal("Coffee", result.First().CategoryName);
        Assert.Equal("Tea", result.Last().CategoryName);

        _categoryRepositoryMock.Verify(r => r.GetAllAsync(), Times.Once);

        _mapperMock.Verify(m => m.Map<IEnumerable<CategoryDto>>(categories), Times.Once);
    }
    
    [Fact]
    public async Task GetAllAsync_WhenNoCategoriesExist_ReturnsEmptyCollection()
    {
        // Arrange
        var categories = new List<Category>();
        var expectedDtos = new List<CategoryDto>();

        _categoryRepositoryMock.Setup(r => r.GetAllAsync()).ReturnsAsync(categories);

        _mapperMock.Setup(m => m.Map<IEnumerable<CategoryDto>>(categories)).Returns(expectedDtos);

        // Act
        var result = await _categoryService.GetAllAsync();

        // Assert
        Assert.Empty(result);

        _categoryRepositoryMock.Verify(r => r.GetAllAsync(), Times.Once);
    }
    
    [Fact]
    public async Task GetByIdAsync_WhenCategoryExists_ReturnsMappedDto()
    {
        // Arrange
        const int id = 1;

        var category = new Category
        {
            CategoryId = id,
            CategoryName = "Coffee",
            MainImageUrl = "coffee.jpg"
        };

        var expectedDto = new CategoryDto
        {
            CategoryId = id,
            CategoryName = "Coffee",
            MainImageUrl = "coffee.jpg"
        };

        _categoryRepositoryMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(category);

        _mapperMock.Setup(m => m.Map<CategoryDto>(category)).Returns(expectedDto);

        // Act
        var result = await _categoryService.GetByIdAsync(id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(id, result.CategoryId);
        Assert.Equal("Coffee", result.CategoryName);

        _categoryRepositoryMock.Verify(r => r.GetByIdAsync(id), Times.Once);

        _mapperMock.Verify(m => m.Map<CategoryDto>(category), Times.Once);
    }
    
    [Fact]
    public async Task GetByIdAsync_WhenCategoryDoesNotExist_ReturnsNull()
    {
        // Arrange
        const int id = 1;

        _categoryRepositoryMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync((Category?)null);

        // Act
        var result = await _categoryService.GetByIdAsync(id);

        // Assert
        Assert.Null(result);

        _categoryRepositoryMock.Verify(r => r.GetByIdAsync(id), Times.Once);

        _mapperMock.Verify(m => m.Map<CategoryDto>(It.IsAny<Category>()), Times.Never);
    }
    
    [Fact]
    public async Task GetTreeAsync_WhenNoCategoriesExist_ReturnsEmptyCollection()
    {
        // Arrange
        var categories = new List<Category>();

        _categoryRepositoryMock.Setup(r => r.GetAllAsync()).ReturnsAsync(categories);

        // Act
        var result = await _categoryService.GetTreeAsync();

        // Assert
        Assert.Empty(result);

        _categoryRepositoryMock.Verify(r => r.GetAllAsync(), Times.Once);
    }
    
    [Fact]
    public async Task GetTreeAsync_WhenCategoriesHaveParentChildRelationships_BuildsCorrectTree()
    {
        // Arrange
        var categories = new List<Category>
        {
            new()
            {
                CategoryId = 1,
                CategoryName = "Coffee",
                MainImageUrl = "coffee.jpg",
                ParentCategoryId = null
            },
            new()
            {
                CategoryId = 2,
                CategoryName = "Espresso",
                MainImageUrl = "espresso.jpg",
                ParentCategoryId = 1
            },
            new()
            {
                CategoryId = 3,
                CategoryName = "Filter Coffee",
                MainImageUrl = "filter.jpg",
                ParentCategoryId = 1
            }
        };

        _categoryRepositoryMock.Setup(r => r.GetAllAsync()).ReturnsAsync(categories);

        // Act
        var result = (await _categoryService.GetTreeAsync()).ToList();

        // Assert
        Assert.Single(result);

        var root = result[0];

        Assert.Equal(1, root.CategoryId);
        Assert.Equal("Coffee", root.CategoryName);

        Assert.Equal(2, root.SubCategories.Count);

        Assert.Contains(root.SubCategories, c => c.CategoryId == 2 && c.CategoryName == "Espresso");

        Assert.Contains(root.SubCategories, c => c.CategoryId == 3 && c.CategoryName == "Filter Coffee");

        _categoryRepositoryMock.Verify(r => r.GetAllAsync(), Times.Once);
    }
    
    [Fact]
    public async Task GetTreeAsync_WhenMultipleRootsAndNestedCategoriesExist_BuildsCorrectTrees()
    {
        // Arrange
        var categories = new List<Category>
        {
            new()
            {
                CategoryId = 1,
                CategoryName = "Coffee",
                MainImageUrl = "coffee.jpg"
            },
            new()
            {
                CategoryId = 2,
                CategoryName = "Espresso",
                MainImageUrl = "espresso.jpg",
                ParentCategoryId = 1
            },
            new()
            {
                CategoryId = 3,
                CategoryName = "Arabica Espresso",
                MainImageUrl = "arabica.jpg",
                ParentCategoryId = 2
            },
            new()
            {
                CategoryId = 4,
                CategoryName = "Filter Coffee",
                MainImageUrl = "filter.jpg",
                ParentCategoryId = 1
            },
            new()
            {
                CategoryId = 5,
                CategoryName = "Tea",
                MainImageUrl = "tea.jpg"
            },
            new()
            {
                CategoryId = 6,
                CategoryName = "Green Tea",
                MainImageUrl = "green-tea.jpg",
                ParentCategoryId = 5
            }
        };

        _categoryRepositoryMock.Setup(r => r.GetAllAsync()).ReturnsAsync(categories);

        // Act
        var result = (await _categoryService.GetTreeAsync()).ToList();

        // Assert
        Assert.Equal(2, result.Count);

        var coffee = result.Single(c => c.CategoryId == 1);
        var tea = result.Single(c => c.CategoryId == 5);

        Assert.Equal(2, coffee.SubCategories.Count);

        var espresso = coffee.SubCategories.Single(c => c.CategoryId == 2);

        Assert.Single(espresso.SubCategories);
        Assert.Equal(3, espresso.SubCategories[0].CategoryId);

        Assert.Single(tea.SubCategories);
        Assert.Equal(6, tea.SubCategories[0].CategoryId);

        _categoryRepositoryMock.Verify(r => r.GetAllAsync(), Times.Once);
    }
    
    private static CreateCategoryDto CreateCategoryDto(int? parentCategoryId = null)
    {
        return new CreateCategoryDto
        {
            CategoryName = "Coffee",
            Image = new MemoryStream(new byte[] { 1, 2, 3 }),
            ImageName = "coffee.jpg",
            ContentType = "image/jpeg",
            ParentCategoryId = parentCategoryId
        };
    }
    
    [Fact]
    public async Task CreateAsync_WhenParentCategoryIsNull_CreatesRootCategory()
    {
        // Arrange
        var dto = CreateCategoryDto();

        var category = new Category
        {
            CategoryName = "Coffee"
        };

        var expectedDto = new CategoryDto
        {
            CategoryId = 1,
            CategoryName = "Coffee",
            MainImageUrl = "coffee-image-url"
        };

        _mapperMock.Setup(m => m.Map<Category>(dto)).Returns(category);

        _imageStorageServiceMock
            .Setup(s => s.UploadAsync(
                dto.Image,
                It.Is<string>(p => p.StartsWith("Categories/") && p.EndsWith("/images")),
                "main.jpg",
                dto.ContentType))
            .ReturnsAsync("coffee-image-url");

        _mapperMock.Setup(m => m.Map<CategoryDto>(category)).Returns(expectedDto);

        // Act
        var result = await _categoryService.CreateAsync(dto);

        // Assert
        Assert.Equal(1, result.CategoryId);
        Assert.Equal("Coffee", result.CategoryName);
        Assert.Equal("coffee-image-url", result.MainImageUrl);

        Assert.NotEqual(Guid.Empty, category.ImageFolderId);
        Assert.Equal("coffee-image-url", category.MainImageUrl);

        _categoryRepositoryMock.Verify(r => r.AddAsync(category), Times.Once);

        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);

        _imageStorageServiceMock.Verify(
            s => s.UploadAsync(
                dto.Image,
                It.IsAny<string>(),
                "main.jpg",
                dto.ContentType),
            Times.Once);
    }
    
    [Fact]
    public async Task CreateAsync_WhenParentCategoryExists_CreatesSubcategory()
    {
        // Arrange
        const int parentId = 10;

        var dto = CreateCategoryDto(parentId);

        var parentCategory = new Category
        {
            CategoryId = parentId,
            CategoryName = "Coffee"
        };

        var category = new Category
        {
            CategoryName = "Espresso"
        };

        _categoryRepositoryMock.Setup(r => r.GetByIdAsync(parentId)).ReturnsAsync(parentCategory);

        _mapperMock.Setup(m => m.Map<Category>(dto)).Returns(category);

        _imageStorageServiceMock
            .Setup(s => s.UploadAsync(
                dto.Image,
                It.IsAny<string>(),
                "main.jpg",
                dto.ContentType))
            .ReturnsAsync("espresso-image-url");

        _mapperMock
            .Setup(m => m.Map<CategoryDto>(category))
            .Returns(new CategoryDto
            {
                CategoryId = 11,
                CategoryName = "Espresso",
                MainImageUrl = "espresso-image-url",
                ParentCategoryId = parentId
            });

        // Act
        var result = await _categoryService.CreateAsync(dto);

        // Assert
        Assert.Equal(parentId, result.ParentCategoryId);
        Assert.Equal("espresso-image-url", result.MainImageUrl);

        _categoryRepositoryMock.Verify(r => r.GetByIdAsync(parentId), Times.Once);

        _categoryRepositoryMock.Verify(r => r.AddAsync(category), Times.Once);

        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
    }
    
    [Fact]
    public async Task CreateAsync_WhenParentCategoryDoesNotExist_ThrowsNotFoundException()
    {
        // Arrange
        const int parentId = 999;

        var dto = CreateCategoryDto(parentId);

        _categoryRepositoryMock.Setup(r => r.GetByIdAsync(parentId)).ReturnsAsync((Category?)null);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => _categoryService.CreateAsync(dto));

        _imageStorageServiceMock.Verify(
            s => s.UploadAsync(
                It.IsAny<Stream>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>()),
            Times.Never);

        _categoryRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Category>()), Times.Never);

        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Never);
    }
    
    [Fact]
    public async Task CreateAsync_WhenSavingFails_DeletesUploadedImageAndThrows()
    {
        // Arrange
        var dto = CreateCategoryDto();

        var category = new Category
        {
            CategoryName = "Coffee"
        };

        _mapperMock.Setup(m => m.Map<Category>(dto)).Returns(category);

        _imageStorageServiceMock
            .Setup(s => s.UploadAsync(
                dto.Image,
                It.IsAny<string>(),
                "main.jpg",
                dto.ContentType))
            .ReturnsAsync("coffee-image-url");

        _unitOfWorkMock.Setup(u => u.SaveChangesAsync()).ThrowsAsync(new Exception("Database error"));

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _categoryService.CreateAsync(dto));

        _categoryRepositoryMock.Verify(r => r.AddAsync(category), Times.Once);

        _imageStorageServiceMock.Verify(s => s.DeleteAsync("coffee-image-url"), Times.Once);
    }
    
    private static UpdateCategoryDto CreateUpdateCategoryDto(int? parentCategoryId = null, Stream? image = null)
    {
        return new UpdateCategoryDto
        {
            CategoryName = "Updated Coffee",
            ParentCategoryId = parentCategoryId,
            Image = image,
            ImageName = image is not null ? "new-image.png" : null,
            ContentType = image is not null ? "image/png" : null
        };
    }
    
    [Fact]
    public async Task UpdateAsync_WhenCategoryDoesNotExist_ReturnsFalse()
    {
        // Arrange
        const int id = 1;

        var dto = CreateUpdateCategoryDto();

        _categoryRepositoryMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync((Category?)null);

        // Act
        var result = await _categoryService.UpdateAsync(id, dto);

        // Assert
        Assert.False(result);

        _categoryRepositoryMock.Verify(r => r.Update(It.IsAny<Category>()), Times.Never);

        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Never);

        _imageStorageServiceMock.Verify(
            s => s.UploadAsync(
                It.IsAny<Stream>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>()),
            Times.Never);
    }
    
    [Fact]
    public async Task UpdateAsync_WhenCategoryExists_UpdatesCategoryAndReturnsTrue()
    {
        // Arrange
        const int id = 1;

        var category = new Category
        {
            CategoryId = id,
            CategoryName = "Old Coffee",
            MainImageUrl = "old-image.jpg",
            ParentCategoryId = null
        };

        var dto = CreateUpdateCategoryDto();

        _categoryRepositoryMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(category);

        // Act
        var result = await _categoryService.UpdateAsync(id, dto);

        // Assert
        Assert.True(result);
        Assert.Equal("Updated Coffee", category.CategoryName);
        Assert.Null(category.ParentCategoryId);
        Assert.Equal("old-image.jpg", category.MainImageUrl);

        _categoryRepositoryMock.Verify(
            r => r.Update(category), Times.Once);

        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);

        _imageStorageServiceMock.Verify(
            s => s.UploadAsync(
                It.IsAny<Stream>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>()),
            Times.Never);
    }
    
    [Fact]
    public async Task UpdateAsync_WhenNewParentExists_UpdatesParentCategory()
    {
        // Arrange
        const int id = 1;
        const int newParentId = 2;

        var category = new Category
        {
            CategoryId = id,
            CategoryName = "Coffee",
            MainImageUrl = "coffee.jpg"
        };

        var parentCategory = new Category
        {
            CategoryId = newParentId,
            CategoryName = "Tea"
        };

        var categories = new List<Category>
        {
            category,
            parentCategory
        };

        var dto = CreateUpdateCategoryDto(newParentId);

        _categoryRepositoryMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(category);

        _categoryRepositoryMock.Setup(r => r.GetAllAsync()).ReturnsAsync(categories);

        // Act
        var result = await _categoryService.UpdateAsync(id, dto);

        // Assert
        Assert.True(result);
        Assert.Equal(newParentId, category.ParentCategoryId);

        _categoryRepositoryMock.Verify(r => r.Update(category), Times.Once);

        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
    }
    
    [Fact]
    public async Task UpdateAsync_WhenParentCategoryIsNull_MovesCategoryToRoot()
    {
        // Arrange
        const int id = 1;

        var category = new Category
        {
            CategoryId = id,
            CategoryName = "Espresso",
            MainImageUrl = "espresso.jpg",
            ParentCategoryId = 2
        };

        var dto = CreateUpdateCategoryDto(null);

        _categoryRepositoryMock.Setup(r => r.GetByIdAsync(id)) .ReturnsAsync(category);

        // Act
        var result = await _categoryService.UpdateAsync(id, dto);

        // Assert
        Assert.True(result);
        Assert.Null(category.ParentCategoryId);

        _categoryRepositoryMock.Verify(r => r.GetAllAsync(), Times.Never);

        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
    }
    
    [Fact]
    public async Task UpdateAsync_WhenParentCategoryDoesNotExist_ThrowsNotFoundException()
    {
        // Arrange
        const int id = 1;
        const int parentId = 999;

        var category = new Category
        {
            CategoryId = id,
            CategoryName = "Coffee",
            MainImageUrl = "coffee.jpg"
        };

        var categories = new List<Category>
        {
            category
        };

        var dto = CreateUpdateCategoryDto(parentId);

        _categoryRepositoryMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(category);

        _categoryRepositoryMock.Setup(r => r.GetAllAsync()).ReturnsAsync(categories);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => _categoryService.UpdateAsync(id, dto));

        _categoryRepositoryMock.Verify(r => r.Update(It.IsAny<Category>()), Times.Never);

        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Never);
    }
    
    [Fact]
    public async Task UpdateAsync_WhenCategoryIsAssignedAsItsOwnParent_ThrowsBadRequestException()
    {
        // Arrange
        const int id = 1;

        var category = new Category
        {
            CategoryId = id,
            CategoryName = "Coffee",
            MainImageUrl = "coffee.jpg"
        };

        var categories = new List<Category>
        {
            category
        };

        var dto = CreateUpdateCategoryDto(id);

        _categoryRepositoryMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(category);

        _categoryRepositoryMock.Setup(r => r.GetAllAsync()).ReturnsAsync(categories);

        // Act & Assert
        await Assert.ThrowsAsync<BadRequestException>(() => _categoryService.UpdateAsync(id, dto));

        _categoryRepositoryMock.Verify(r => r.Update(It.IsAny<Category>()), Times.Never);

        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Never);
    }
    
    [Fact]
    public async Task UpdateAsync_WhenMovingCategoryUnderItsDescendant_ThrowsBadRequestException()
    {
        // Arrange
        const int id = 1;
        const int descendantId = 3;

        var categories = new List<Category>
        {
            new()
            {
                CategoryId = 1,
                CategoryName = "Coffee"
            },
            new()
            {
                CategoryId = 2,
                CategoryName = "Espresso",
                ParentCategoryId = 1
            },
            new()
            {
                CategoryId = 3,
                CategoryName = "Arabica Espresso",
                ParentCategoryId = 2
            }
        };

        var category = categories[0];

        var dto = CreateUpdateCategoryDto(descendantId);

        _categoryRepositoryMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(category);

        _categoryRepositoryMock.Setup(r => r.GetAllAsync()).ReturnsAsync(categories);

        // Act & Assert
        await Assert.ThrowsAsync<BadRequestException>(() => _categoryService.UpdateAsync(id, dto));

        _categoryRepositoryMock.Verify(r => r.Update(It.IsAny<Category>()), Times.Never);

        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Never);
    }
    
    [Fact]
    public async Task UpdateAsync_WhenNewImageIsProvided_UpdatesImageAndDeletesOldImage()
    {
        // Arrange
        const int id = 1;
        var folderId = Guid.NewGuid();

        var category = new Category
        {
            CategoryId = id,
            CategoryName = "Coffee",
            MainImageUrl = "old-image.jpg",
            ImageFolderId = folderId
        };

        var image = new MemoryStream(new byte[] { 1, 2, 3 });
        var dto = CreateUpdateCategoryDto(image: image);

        _categoryRepositoryMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(category);

        _imageStorageServiceMock
            .Setup(s => s.UploadAsync(
                image,
                $"Categories/{folderId}/images",
                "main.png",
                "image/png"))
            .ReturnsAsync("new-image.png");

        // Act
        var result = await _categoryService.UpdateAsync(id, dto);

        // Assert
        Assert.True(result);
        Assert.Equal("new-image.png", category.MainImageUrl);

        _imageStorageServiceMock.Verify(
            s => s.UploadAsync(
                image,
                $"Categories/{folderId}/images",
                "main.png",
                "image/png"),
            Times.Once);

        _imageStorageServiceMock.Verify( s => s.DeleteAsync("old-image.jpg"), Times.Once);

        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
    }
    
    [Fact]
    public async Task UpdateAsync_WhenSavingFails_DeletesNewImageAndThrows()
    {
        // Arrange
        const int id = 1;
        var folderId = Guid.NewGuid();

        var category = new Category
        {
            CategoryId = id,
            CategoryName = "Coffee",
            MainImageUrl = "old-image.jpg",
            ImageFolderId = folderId
        };

        var image = new MemoryStream(new byte[] { 1, 2, 3 });
        var dto = CreateUpdateCategoryDto(image: image);

        _categoryRepositoryMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(category);

        _imageStorageServiceMock
            .Setup(s => s.UploadAsync(
                image,
                $"Categories/{folderId}/images",
                "main.png",
                "image/png"))
            .ReturnsAsync("new-image.png");

        _unitOfWorkMock.Setup(u => u.SaveChangesAsync()).ThrowsAsync(new Exception("Database error"));

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _categoryService.UpdateAsync(id, dto));

        _imageStorageServiceMock.Verify(s => s.DeleteAsync("new-image.png"), Times.Once);

        _imageStorageServiceMock.Verify(s => s.DeleteAsync("old-image.jpg"), Times.Never);
    }
    
    [Fact]
    public async Task DeleteAsync_WhenCategoryDoesNotExist_ReturnsFalse()
    {
        // Arrange
        const int id = 1;

        _categoryRepositoryMock
            .Setup(r => r.GetByIdAsync(id))
            .ReturnsAsync((Category?)null);

        // Act
        var result = await _categoryService.DeleteAsync(id);

        // Assert
        Assert.False(result);

        _imageStorageServiceMock.Verify(
            s => s.DeleteAsync(It.IsAny<string>()),
            Times.Never);

        _categoryRepositoryMock.Verify(
            r => r.Delete(It.IsAny<Category>()),
            Times.Never);

        _unitOfWorkMock.Verify(
            u => u.SaveChangesAsync(),
            Times.Never);
    }
    
    [Fact]
    public async Task DeleteAsync_WhenCategoryExists_DeletesCategoryAndItsImage()
    {
        // Arrange
        const int id = 1;

        var category = new Category
        {
            CategoryId = id,
            CategoryName = "Coffee",
            MainImageUrl = "coffee.jpg"
        };

        _categoryRepositoryMock
            .Setup(r => r.GetByIdAsync(id))
            .ReturnsAsync(category);

        _imageStorageServiceMock
            .Setup(s => s.DeleteAsync(category.MainImageUrl))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _categoryService.DeleteAsync(id);

        // Assert
        Assert.True(result);

        _imageStorageServiceMock.Verify(
            s => s.DeleteAsync(category.MainImageUrl),
            Times.Once);

        _categoryRepositoryMock.Verify(
            r => r.Delete(category),
            Times.Once);

        _unitOfWorkMock.Verify(
            u => u.SaveChangesAsync(),
            Times.Once);
    }
}