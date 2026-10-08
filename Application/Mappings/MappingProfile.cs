using Application.DTOs.Product;
using Application.DTOs.Category;
using Application.DTOs.Cart;
using Application.DTOs.Order;
using Application.DTOs.Review;
using Application.DTOs.Payment;
using Application.DTOs.UserAndAuth;
using Application.DTOs.Brand;
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
            
            //Product Attribute
            CreateMap<ProductAttribute, ProductAttributeDto>();
            CreateMap<ProductAttributeValue, ProductAttributeValueDto>();

            //Product Variant
            CreateMap<ProductVariant, ProductVariantDto>();

            //ProductImage
            CreateMap<ProductImage, ProductImageDto>();
            
            //ProductSpecification
            CreateMap<ProductSpecification, ProductSpecificationDto>();

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
            
            //Cart Item
            CreateMap<CartItem, CartItemDto>()
                .ForMember(dest => dest.ProductVariantId,
                    opt => opt.MapFrom(src => src.ProductVariantId))
                .ForMember(dest => dest.ProductName,
                    opt => opt.MapFrom(src => src.Product.ProductName))
                .ForMember(dest => dest.UnitPrice,
                    opt => opt.MapFrom(src =>
                        src.ProductVariant != null
                            ? src.ProductVariant.SalePrice
                            : src.Product.SalePrice))
                .ForMember(dest => dest.LineTotal,
                    opt => opt.MapFrom(src =>
                        (src.ProductVariant != null
                            ? src.ProductVariant.SalePrice
                            : src.Product.SalePrice) * src.Quantity))
                .ForMember(dest => dest.ProductImageUrl,
                    opt => opt.MapFrom(src =>
                        src.Product.Images
                            .Where(i => i.IsMain)
                            .Select(i => i.ImageUrl)
                            .FirstOrDefault()));
            //Cart 
            CreateMap<Cart, CartDto>()
                .ForMember(dest => dest.TotalItems,
                    opt => opt.MapFrom(src =>
                        src.Items.Sum(i => i.Quantity)))
                .ForMember(dest => dest.TotalPrice,
                    opt => opt.MapFrom(src =>
                        src.Items.Sum(i =>
                            i.Quantity *
                            (i.ProductVariant != null
                                ? i.ProductVariant.SalePrice
                                : i.Product.SalePrice))));
            
            //Order Item
            CreateMap<OrderItem, OrderItemDto>()
                .ForMember(dest => dest.LineTotal,
                    opt => opt.MapFrom(src => src.UnitPrice * src.Quantity));
            
            //Order
            CreateMap<Order, OrderDto>();
            
            //Payment
            CreateMap<Payment, PaymentDto>();
        }
    }
}