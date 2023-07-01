namespace NBUniforms.Infrastructure
{
    using AutoMapper;
    using NBUniforms.Data.Models;
    using NBUniforms.Services.Products;

    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            this.CreateMap<Product, LatestProductServiceModel>();

            this.CreateMap<ProductDetailsServiceModel, ProductFormServiceModel>();
        }
    }
}