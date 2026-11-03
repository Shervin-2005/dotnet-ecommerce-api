using Application.DTOs.Brand;
using Application.Interfaces.Brand;
using Application.Interfaces.Common;
using Application.Services;
using AutoMapper;
using Domain.Entities;
using Domain.Exceptions;
using Moq;

namespace Application.UnitTests.Services;

public class BrandGroupServiceTests
{
    [Fact]
    public async Task DeleteAsync_WhenBrandGroupDoesNotExist_ReturnsFalse()
    {
        // Arrange
        const int id = 1;

        var unitOfWorkMock = new Mock<IUnitOfWork>();
        var repositoryMock = new Mock<IBrandGroupRepository>();
        var mapperMock = new Mock<IMapper>();

        unitOfWorkMock.Setup(u => u.BrandGroups).Returns(repositoryMock.Object);

        repositoryMock.Setup(r => r.GetByIdWithBrandsAsync(id)).ReturnsAsync((BrandGroup?)null);

        var service = new BrandGroupService(unitOfWorkMock.Object, mapperMock.Object);

        // Act
        var result = await service.DeleteAsync(id);

        // Assert
        Assert.False(result);

        repositoryMock.Verify(r => r.Delete(It.IsAny<BrandGroup>()), Times.Never);

        unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Never);
    }
    
    [Fact]
    public async Task DeleteAsync_WhenBrandGroupContainsBrands_ThrowsConflictException()
    {
        // Arrange
        const int id = 1;

        var group = new BrandGroup
        {
            BrandGroupId = id,
            Name = "Coffee",
            Brands = new List<Brand>
            {
                new Brand()
            }
        };

        var unitOfWorkMock = new Mock<IUnitOfWork>();
        var repositoryMock = new Mock<IBrandGroupRepository>();
        var mapperMock = new Mock<IMapper>();

        unitOfWorkMock.Setup(u => u.BrandGroups).Returns(repositoryMock.Object);

        repositoryMock.Setup(r => r.GetByIdWithBrandsAsync(id)).ReturnsAsync(group);

        var service = new BrandGroupService(unitOfWorkMock.Object, mapperMock.Object);

        // Act & Assert
        await Assert.ThrowsAsync<ConflictException>(() => service.DeleteAsync(id));

        repositoryMock.Verify(r => r.Delete(It.IsAny<BrandGroup>()), Times.Never);

        unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Never);
    }
    
    [Fact]
    public async Task DeleteAsync_WhenBrandGroupIsEmpty_DeletesGroupAndReturnsTrue()
    {
        // Arrange
        const int id = 1;

        var group = new BrandGroup
        {
            BrandGroupId = id,
            Name = "Coffee",
            Brands = new List<Brand>()
        };

        var unitOfWorkMock = new Mock<IUnitOfWork>();
        var repositoryMock = new Mock<IBrandGroupRepository>();
        var mapperMock = new Mock<IMapper>();

        unitOfWorkMock.Setup(u => u.BrandGroups).Returns(repositoryMock.Object);

        repositoryMock.Setup(r => r.GetByIdWithBrandsAsync(id)).ReturnsAsync(group);

        var service = new BrandGroupService(unitOfWorkMock.Object, mapperMock.Object);

        // Act
        var result = await service.DeleteAsync(id);

        // Assert
        Assert.True(result);

        repositoryMock.Verify(r => r.Delete(group), Times.Once);

        unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
    }
    
    [Fact]
    public async Task CreateAsync_WhenNameIsValid_CreatesBrandGroupWithSlug()
    {
        // Arrange
        var dto = new CreateBrandGroupDto
        {
            Name = "Coffee Beans",
            DisplayOrder = 1
        };

        var group = new BrandGroup
        {
            Name = dto.Name,
            DisplayOrder = dto.DisplayOrder
        };

        var unitOfWorkMock = new Mock<IUnitOfWork>();
        var repositoryMock = new Mock<IBrandGroupRepository>();
        var mapperMock = new Mock<IMapper>();

        unitOfWorkMock .Setup(u => u.BrandGroups).Returns(repositoryMock.Object);

        mapperMock.Setup(m => m.Map<BrandGroup>(dto)).Returns(group);

        mapperMock.Setup(m => m.Map<BrandGroupDto>(group))
            .Returns(new BrandGroupDto
            {
                Name = "Coffee Beans",
                Slug = "coffee-beans",
                DisplayOrder = 1
            });

        repositoryMock.Setup(r => r.SlugExistsAsync("coffee-beans", null)).ReturnsAsync(false);

        var service = new BrandGroupService(unitOfWorkMock.Object, mapperMock.Object);

        // Act
        var result = await service.CreateAsync(dto);

        // Assert
        Assert.Equal("coffee-beans", result.Slug);

        repositoryMock.Verify(r => r.AddAsync(group), Times.Once);

        unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
    }
    
    [Fact]
    public async Task CreateAsync_WhenSlugAlreadyExists_GeneratesUniqueSlug()
    {
        // Arrange
        var dto = new CreateBrandGroupDto
        {
            Name = "Coffee Beans",
            DisplayOrder = 1
        };

        var group = new BrandGroup
        {
            Name = dto.Name,
            DisplayOrder = dto.DisplayOrder
        };

        var unitOfWorkMock = new Mock<IUnitOfWork>();
        var repositoryMock = new Mock<IBrandGroupRepository>();
        var mapperMock = new Mock<IMapper>();

        unitOfWorkMock.Setup(u => u.BrandGroups).Returns(repositoryMock.Object);

        mapperMock.Setup(m => m.Map<BrandGroup>(dto)).Returns(group);

        mapperMock.Setup(m => m.Map<BrandGroupDto>(group))
            .Returns(() => new BrandGroupDto
            {
                Name = group.Name,
                Slug = group.Slug,
                DisplayOrder = group.DisplayOrder
            });

        repositoryMock.SetupSequence(r => r.SlugExistsAsync("coffee-beans", null)).ReturnsAsync(true);

        repositoryMock.Setup(r => r.SlugExistsAsync("coffee-beans-2", null)).ReturnsAsync(true);

        repositoryMock.Setup(r => r.SlugExistsAsync("coffee-beans-3", null)).ReturnsAsync(false);

        // Act
        var service = new BrandGroupService(unitOfWorkMock.Object, mapperMock.Object);

        var result = await service.CreateAsync(dto);

        // Assert
        Assert.Equal("coffee-beans-3", result.Slug);

        repositoryMock.Verify(r => r.SlugExistsAsync(It.IsAny<string>(), null), Times.Exactly(3));

        repositoryMock.Verify(r => r.AddAsync(group), Times.Once);

        unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
    }
    
    [Fact]
    public async Task UpdateAsync_WhenBrandGroupDoesNotExist_ReturnsFalse()
    {
        // Arrange
        const int id = 1;

        var dto = new UpdateBrandGroupDto
        {
            Name = "New Coffee",
            DisplayOrder = 2
        };

        var unitOfWorkMock = new Mock<IUnitOfWork>();
        var repositoryMock = new Mock<IBrandGroupRepository>();
        var mapperMock = new Mock<IMapper>();

        unitOfWorkMock.Setup(u => u.BrandGroups).Returns(repositoryMock.Object);

        repositoryMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync((BrandGroup?)null);

        var service = new BrandGroupService(unitOfWorkMock.Object, mapperMock.Object);

        // Act
        var result = await service.UpdateAsync(id, dto);

        // Assert
        Assert.False(result);

        repositoryMock.Verify(r => r.Update(It.IsAny<BrandGroup>()), Times.Never);

        unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Never);
    }
    
    [Fact]
    public async Task UpdateAsync_WhenBrandGroupExists_UpdatesGroupAndReturnsTrue()
    {
        // Arrange
        const int id = 1;

        var group = new BrandGroup
        {
            BrandGroupId = id,
            Name = "Old Coffee",
            Slug = "old-coffee",
            DisplayOrder = 1
        };

        var dto = new UpdateBrandGroupDto
        {
            Name = "New Coffee",
            DisplayOrder = 2
        };

        var unitOfWorkMock = new Mock<IUnitOfWork>();
        var repositoryMock = new Mock<IBrandGroupRepository>();
        var mapperMock = new Mock<IMapper>();

        unitOfWorkMock.Setup(u => u.BrandGroups).Returns(repositoryMock.Object);

        repositoryMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(group);

        repositoryMock.Setup(r => r.SlugExistsAsync("new-coffee", id)).ReturnsAsync(false);

        var service = new BrandGroupService(unitOfWorkMock.Object, mapperMock.Object);

        // Act
        var result = await service.UpdateAsync(id, dto);

        // Assert
        Assert.True(result);
        Assert.Equal("New Coffee", group.Name);
        Assert.Equal("new-coffee", group.Slug);
        Assert.Equal(2, group.DisplayOrder);

        repositoryMock.Verify(r => r.Update(group), Times.Once);

        unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
    }
    
    [Fact]
    public async Task UpdateAsync_WhenSlugAlreadyExists_GeneratesUniqueSlug()
    {
        // Arrange
        const int id = 1;

        var group = new BrandGroup
        {
            BrandGroupId = id,
            Name = "Old Coffee",
            Slug = "old-coffee",
            DisplayOrder = 1
        };

        var dto = new UpdateBrandGroupDto
        {
            Name = "Coffee Beans",
            DisplayOrder = 2
        };

        var unitOfWorkMock = new Mock<IUnitOfWork>();
        var repositoryMock = new Mock<IBrandGroupRepository>();
        var mapperMock = new Mock<IMapper>();

        unitOfWorkMock.Setup(u => u.BrandGroups).Returns(repositoryMock.Object);

        repositoryMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(group);

        repositoryMock
            .SetupSequence(r => r.SlugExistsAsync(It.IsAny<string>(), id))
            .ReturnsAsync(true)
            .ReturnsAsync(true)
            .ReturnsAsync(false);

        var service = new BrandGroupService(unitOfWorkMock.Object, mapperMock.Object);

        // Act
        var result = await service.UpdateAsync(id, dto);

        // Assert
        Assert.True(result);
        Assert.Equal("coffee-beans-3", group.Slug);

        repositoryMock.Verify(r => r.Update(group), Times.Once);

        unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
    }
    
    [Fact]
    public async Task UpdateAsync_WhenNameDoesNotChange_GeneratesSameSlug()
    {
        // Arrange
        const int id = 1;

        var group = new BrandGroup
        {
            BrandGroupId = id,
            Name = "Coffee Beans",
            Slug = "coffee-beans",
            DisplayOrder = 1
        };

        var dto = new UpdateBrandGroupDto
        {
            Name = "Coffee Beans",
            DisplayOrder = 2
        };

        var unitOfWorkMock = new Mock<IUnitOfWork>();
        var repositoryMock = new Mock<IBrandGroupRepository>();
        var mapperMock = new Mock<IMapper>();

        unitOfWorkMock.Setup(u => u.BrandGroups).Returns(repositoryMock.Object);

        repositoryMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(group);

        repositoryMock.Setup(r => r.SlugExistsAsync("coffee-beans", id)).ReturnsAsync(false);

        var service = new BrandGroupService(unitOfWorkMock.Object, mapperMock.Object);

        // Act
        var result = await service.UpdateAsync(id, dto);

        // Assert
        Assert.True(result);
        Assert.Equal("coffee-beans", group.Slug);

        repositoryMock.Verify(r => r.SlugExistsAsync("coffee-beans", id), Times.Once);

        repositoryMock.Verify(r => r.Update(group), Times.Once);

        unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
    }
    
    [Fact]
    public async Task UpdateAsync_WhenNameContainsSpecialCharacters_GeneratesValidSlug()
    {
        // Arrange
        const int id = 1;

        var group = new BrandGroup
        {
            BrandGroupId = id,
            Name = "Old Name",
            Slug = "old-name",
            DisplayOrder = 1
        };

        var dto = new UpdateBrandGroupDto
        {
            Name = "  Coffee & Tea!  ",
            DisplayOrder = 2
        };

        var unitOfWorkMock = new Mock<IUnitOfWork>();
        var repositoryMock = new Mock<IBrandGroupRepository>();
        var mapperMock = new Mock<IMapper>();

        unitOfWorkMock.Setup(u => u.BrandGroups).Returns(repositoryMock.Object);

        repositoryMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(group);

        repositoryMock.Setup(r => r.SlugExistsAsync("coffee-tea", id)).ReturnsAsync(false);

        var service = new BrandGroupService(unitOfWorkMock.Object, mapperMock.Object);

        // Act
        var result = await service.UpdateAsync(id, dto);

        // Assert
        Assert.True(result);
        Assert.Equal("coffee-tea", group.Slug);
    }
    
    [Fact]
    public async Task UpdateAsync_WhenNameProducesEmptySlug_UsesFallbackSlug()
    {
        // Arrange
        const int id = 1;

        var group = new BrandGroup
        {
            BrandGroupId = id,
            Name = "Old Name",
            Slug = "old-name",
            DisplayOrder = 1
        };

        var dto = new UpdateBrandGroupDto
        {
            Name = "!!!",
            DisplayOrder = 2
        };

        var unitOfWorkMock = new Mock<IUnitOfWork>();
        var repositoryMock = new Mock<IBrandGroupRepository>();
        var mapperMock = new Mock<IMapper>();

        unitOfWorkMock.Setup(u => u.BrandGroups).Returns(repositoryMock.Object);

        repositoryMock.Setup(r => r.GetByIdAsync(id)) .ReturnsAsync(group);

        repositoryMock.Setup(r => r.SlugExistsAsync("brand-group", id)).ReturnsAsync(false);

        var service = new BrandGroupService(unitOfWorkMock.Object, mapperMock.Object);

        // Act
        var result = await service.UpdateAsync(id, dto);

        // Assert
        Assert.True(result);
        Assert.Equal("brand-group", group.Slug);
    }
    
    [Fact]
    public async Task GetByIdAsync_WhenBrandGroupExists_ReturnsBrandGroupDto()
    {
        // Arrange
        const int id = 1;

        var group = new BrandGroup
        {
            BrandGroupId = id,
            Name = "Coffee",
            Slug = "coffee",
            DisplayOrder = 1
        };

        var expectedDto = new BrandGroupDto
        {
            BrandGroupId = id,
            Name = "Coffee",
            Slug = "coffee",
            DisplayOrder = 1
        };

        var unitOfWorkMock = new Mock<IUnitOfWork>();
        var repositoryMock = new Mock<IBrandGroupRepository>();
        var mapperMock = new Mock<IMapper>();

        unitOfWorkMock.Setup(u => u.BrandGroups).Returns(repositoryMock.Object);

        repositoryMock.Setup(r => r.GetByIdWithBrandsAsync(id)) .ReturnsAsync(group);

        mapperMock.Setup(m => m.Map<BrandGroupDto>(group)).Returns(expectedDto);

        var service = new BrandGroupService(unitOfWorkMock.Object, mapperMock.Object);

        // Act
        var result = await service.GetByIdAsync(id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(id, result.BrandGroupId);
        Assert.Equal("Coffee", result.Name);
        Assert.Equal("coffee", result.Slug);
    }
    
    [Fact]
    public async Task GetByIdAsync_WhenBrandGroupDoesNotExist_ReturnsNull()
    {
        // Arrange
        const int id = 1;

        var unitOfWorkMock = new Mock<IUnitOfWork>();
        var repositoryMock = new Mock<IBrandGroupRepository>();
        var mapperMock = new Mock<IMapper>();

        unitOfWorkMock.Setup(u => u.BrandGroups).Returns(repositoryMock.Object);
        
        repositoryMock.Setup(r => r.GetByIdWithBrandsAsync(id)).ReturnsAsync((BrandGroup?)null);

        var service = new BrandGroupService(unitOfWorkMock.Object, mapperMock.Object);

        // Act
        var result = await service.GetByIdAsync(id);

        // Assert
        Assert.Null(result);

        mapperMock.Verify(m => m.Map<BrandGroupDto>(It.IsAny<BrandGroup>()), Times.Never);
    }
    
    [Fact]
    public async Task GetAllWithBrandsAsync_WhenGroupsExist_ReturnsMappedDtos()
    {
        // Arrange
        var groups = new List<BrandGroup>
        {
            new()
            {
                BrandGroupId = 1,
                Name = "Coffee",
                Slug = "coffee",
                DisplayOrder = 1
            },
            new()
            {
                BrandGroupId = 2,
                Name = "Tea",
                Slug = "tea",
                DisplayOrder = 2
            }
        };

        var expectedDtos = new List<BrandGroupDto>
        {
            new()
            {
                BrandGroupId = 1,
                Name = "Coffee",
                Slug = "coffee",
                DisplayOrder = 1
            },
            new()
            {
                BrandGroupId = 2,
                Name = "Tea",
                Slug = "tea",
                DisplayOrder = 2
            }
        };

        var unitOfWorkMock = new Mock<IUnitOfWork>();
        var repositoryMock = new Mock<IBrandGroupRepository>();
        var mapperMock = new Mock<IMapper>();

        unitOfWorkMock.Setup(u => u.BrandGroups) .Returns(repositoryMock.Object);

        repositoryMock.Setup(r => r.GetAllWithBrandsAsync()).ReturnsAsync(groups);

        mapperMock.Setup(m => m.Map<List<BrandGroupDto>>(groups)) .Returns(expectedDtos);

        var service = new BrandGroupService(unitOfWorkMock.Object, mapperMock.Object);

        // Act
        var result = await service.GetAllWithBrandsAsync();

        // Assert
        Assert.Equal(2, result.Count);
        Assert.Equal("Coffee", result[0].Name);
        Assert.Equal("Tea", result[1].Name);
    }
}