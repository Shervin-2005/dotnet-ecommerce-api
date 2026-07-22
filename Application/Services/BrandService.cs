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

        public BrandService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<BrandDto> CreateAsync(CreateBrandDto dto)
        {
            var brand = _mapper.Map<Brand>(dto);
            await _unitOfWork.Brands.AddAsync(brand);
            await _unitOfWork.SaveChangesAsync();
            return _mapper.Map<BrandDto>(brand);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var brand = await _unitOfWork.Brands.GetByIdAsync(id);
            if (brand is null) return false;

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
            var brand = await _unitOfWork.Brands.GetByIdAsync(id);
            if (brand is null) return false;

            _mapper.Map(dto, brand);
            _unitOfWork.Brands.Update(brand);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }
    }
}
