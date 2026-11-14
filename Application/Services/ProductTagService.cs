using Application.DTOs.Product;
using Application.Interfaces.Common;
using Application.Interfaces.Product;
using AutoMapper;
using Domain.Entities;
using Domain.Exceptions;

namespace Application.Services
{
    public class ProductTagService : IProductTagService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ProductTagService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<ProductTagDto> CreateAsync(CreateProductTagDto dto)
        {
            var name = dto.Name.Trim();

            var existingTag = await _unitOfWork.ProductTags.GetByNameAsync(name);

            if (existingTag is not null)
                throw new ConflictException("A tag with this name already exists.");

            var tag = _mapper.Map<ProductTag>(dto);

            tag.Name = name;
            tag.Slug = await GenerateUniqueSlugAsync(name);

            await _unitOfWork.ProductTags.AddAsync(tag);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<ProductTagDto>(tag);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var tag = await _unitOfWork.ProductTags.GetByIdAsync(id);

            if (tag is null)
                return false;

            _unitOfWork.ProductTags.Delete(tag);

            await _unitOfWork.SaveChangesAsync();

            return true;
        }

        public async Task<IEnumerable<ProductTagDto>> GetAllAsync()
        {
            var tags = await _unitOfWork.ProductTags.GetAllAsync();

            return _mapper.Map<IEnumerable<ProductTagDto>>(tags);
        }

        public async Task<ProductTagDto?> GetByIdAsync(int id)
        {
            var tag = await _unitOfWork.ProductTags.GetByIdAsync(id);

            return tag is null ? null : _mapper.Map<ProductTagDto>(tag);
        }

        public async Task<bool> UpdateAsync(int id, UpdateProductTagDto dto)
        {
            var tag = await _unitOfWork.ProductTags.GetByIdAsync(id);

            if (tag is null)
                return false;

            var name = dto.Name.Trim();

            var existingTag = await _unitOfWork.ProductTags.GetByNameAsync(name);

            if (existingTag is not null && existingTag.ProductTagId != id)
                throw new ConflictException("A tag with this name already exists.");

            if (tag.Name != name)
            {
                tag.Name = name;
                tag.Slug = await GenerateUniqueSlugAsync(name);
            }

            _unitOfWork.ProductTags.Update(tag);

            await _unitOfWork.SaveChangesAsync();

            return true;
        }

        private async Task<string> GenerateUniqueSlugAsync(string name)
        {
            var baseSlug = GenerateSlug(name);
            var slug = baseSlug;
            var counter = 2;

            while (await _unitOfWork.ProductTags.GetBySlugAsync(slug) is not null)
            {
                slug = $"{baseSlug}-{counter}";
                counter++;
            }

            return slug;
        }

        private static string GenerateSlug(string name)
        {
            var slug = new System.Text.StringBuilder();
            var previousWasSeparator = false;

            foreach (var character in name.Trim().ToLowerInvariant())
            {
                if (char.IsLetterOrDigit(character))
                {
                    slug.Append(character);
                    previousWasSeparator = false;
                }
                else if (!previousWasSeparator && slug.Length > 0)
                {
                    slug.Append('-');
                    previousWasSeparator = true;
                }
            }

            return slug.ToString().Trim('-');
        }
    }
}