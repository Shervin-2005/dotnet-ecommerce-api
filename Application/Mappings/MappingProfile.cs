using Application.DTOs;
using Application.DTOs.Auth;
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

            // User
            CreateMap<User, UserDto>();
            
            // Review
            CreateMap<ProductReview, ReviewDto>()
                .ForMember(
                    dest => dest.ReviewerName,
                    opt => opt.MapFrom(src =>
                        $"{src.User.FirstName} {src.User.LastName}".Trim())
                );
            
            //Cart
            CreateMap<CartItem, CartItemDto>()
                .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product.ProductName))
                .ForMember(dest => dest.UnitPrice, opt => opt.MapFrom(src => src.Product.Price))
                .ForMember(dest => dest.LineTotal, opt => opt.MapFrom(src => src.Product.Price * src.Quantity))
                .ForMember(dest => dest.ProductImageUrl, opt => opt.MapFrom(src =>
                    src.Product.Images.Where(i => i.IsMain).Select(i => i.ImageUrl).FirstOrDefault()));
            //Cart Item
            CreateMap<Cart, CartDto>()
                .ForMember(dest => dest.TotalItems, opt => opt.MapFrom(src => src.Items.Sum(i => i.Quantity)))
                .ForMember(dest => dest.TotalPrice, opt => opt.MapFrom(src => src.Items.Sum(i => i.Quantity * i.Product.Price)));
            
            //Order Item
            CreateMap<OrderItem, OrderItemDto>()
                .ForMember(dest => dest.LineTotal, opt => opt.MapFrom(src => src.UnitPrice * src.Quantity));
            //Order
            CreateMap<Order, OrderDto>();
        }
    }
}