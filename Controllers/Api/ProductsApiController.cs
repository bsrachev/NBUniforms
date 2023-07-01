namespace NBUniforms.Controllers.Api
{
    using NBUniforms.Models.Api.Products;
    using NBUniforms.Services.Products;
    using Microsoft.AspNetCore.Mvc;

    [ApiController]
    [Route("api/products")]
    public class ProductsApiController : ControllerBase
    {
        private readonly IProductService products;

        public ProductsApiController(IProductService products)
            => this.products = products;

        [HttpGet]
        public ProductQueryServiceModel All([FromQuery] AllProductsApiRequestModel query)
            => this.products.All(
                query.CategoryId,
                query.SearchTerm,
                query.Sorting,
                query.CurrentPage,
                query.ProductsPerPage);
    }
}