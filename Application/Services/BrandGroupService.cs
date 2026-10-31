using Application.DTOs.Brand;
using Application.Interfaces;
using Application.Interfaces.Brand;
using AutoMapper;
using Domain.Entities;
using System.Text.RegularExpressions;

namespace Application.Services;

public class BrandGroupService : IBrandGroupService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public BrandGroupService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IReadOnlyList<BrandGroupDto>> GetAllWithBrandsAsync()
    {
        var groups = await _unitOfWork.BrandGroups.GetAllWithBrandsAsync();

        return _mapper.Map<List<BrandGroupDto>>(groups);
    }

    public async Task<BrandGroupDto?> GetByIdAsync(int id)
    {
        var group = await _unitOfWork.BrandGroups.GetByIdWithBrandsAsync(id);

        return group is null ? null : _mapper.Map<BrandGroupDto>(group);
    }

    public async Task<BrandGroupDto> CreateAsync(CreateBrandGroupDto dto)
    {
        var group = _mapper.Map<BrandGroup>(dto);

        group.Slug = await GenerateUniqueSlugAsync(dto.Name);

        await _unitOfWork.BrandGroups.AddAsync(group);
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<BrandGroupDto>(group);
    }

    public async Task<bool> UpdateAsync(int id, UpdateBrandGroupDto dto)
    {
        var group = await _unitOfWork.BrandGroups.GetByIdAsync(id);

        if (group is null)
            return false;

        group.Name = dto.Name;
        group.DisplayOrder = dto.DisplayOrder;
        group.Slug = await GenerateUniqueSlugAsync(dto.Name, id);

        _unitOfWork.BrandGroups.Update(group);

        await _unitOfWork.SaveChangesAsync();

        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var group = await _unitOfWork.BrandGroups.GetByIdWithBrandsAsync(id);

        if (group is null)
            return false;

        if (group.Brands.Any())
            throw new InvalidOperationException("Cannot delete a brand group that contains brands.");

        _unitOfWork.BrandGroups.Delete(group);

        await _unitOfWork.SaveChangesAsync();

        return true;
    }

    private async Task<string> GenerateUniqueSlugAsync(string name, int? excludeId = null)
    {
        var baseSlug = Regex.Replace(
            name.Trim().ToLowerInvariant(),
            @"[^\p{L}\p{Nd}]+",
            "-").Trim('-');

        if (string.IsNullOrWhiteSpace(baseSlug))
            baseSlug = "brand-group";

        var slug = baseSlug;
        var counter = 2;

        while (await _unitOfWork.BrandGroups.SlugExistsAsync(slug, excludeId))
        {
            slug = $"{baseSlug}-{counter}";
            counter++;
        }

        return slug;
    }
}