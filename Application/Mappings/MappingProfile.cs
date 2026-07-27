using Application.DTOs;
using AutoMapper;
using Domain.Entities;

namespace Application.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Product
            CreateMap<Product, ProductDto>()
                .ForMember(dest => dest.CategoryName,
                    opt => opt.MapFrom(src => src.Category != null ? src.Category.CategoryName : null))
                .ForMember(dest => dest.BrandName,
                    opt => opt.MapFrom(src => src.Brand != null ? src.Brand.BrandName : null));

            CreateMap<CreateProductDto, Product>()
                .ForMember(dest => dest.Images,
                 opt => opt.Ignore());
            CreateMap<UpdateProductDto, Product>();

            //ProductImage
            CreateMap<ProductImage, ProductImageDto>();

            // Brand
            CreateMap<Brand, BrandDto>();
            CreateMap<CreateBrandDto, Brand>();
            CreateMap<UpdateBrandDto, Brand>();

            // Category
            CreateMap<Category, CategoryDto>();
            CreateMap<CreateCategoryDto, Category>();
            CreateMap<UpdateCategoryDto, Category>();
        }
    }
}