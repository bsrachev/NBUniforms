namespace NBUniforms.Services.Products
{
    using System.Collections.Generic;
    using NBUniforms.Data.Models;
    using NBUniforms.Data.Models.Enums;
    using NBUniforms.Models;
    using NBUniforms.Service.Products;

    public interface IProductService
    {
        ProductQueryServiceModel All(
            int categoryId,
            string searchTerm,
            ProductSorting sorting,
            int currentPage,
            int productsPerPage);

        IEnumerable<LatestProductServiceModel> Latest();

        Product Find(int id);

        ProductDetailsServiceModel FindById(int Id);

        ProductDetailsServiceModel FindByConvertedName(string convertedName);

        int Create(ProductFormServiceModel product, string convertedName);

        bool Edit(
                int id,
                string name,
                string description,
                string imageUrl,
                double price,
                int categoryId,
                string convertedName,
                int quantityS,
                int quantityM,
                int quantityL);

        void Remove(Product product);

        void AddProductToCart(CartProduct cartProduct);

        ICollection<ProductSizeQuantity> ProductSizeQuantity(int productId);

        void RemoveProductSizeQuantities(ProductSizeQuantity psq);

        void AddQuantities(ProductFormServiceModel product, int ProductQuantityMin, int productId);

        IEnumerable<ProductCategoryServiceModel> AllCategories();

        Category ProductCategory(string productCategoryName);

        bool CategoryExists(int categoryId);

        string CreateConvertedName(ProductFormServiceModel product);

        bool ConvertedNameExists(string convertedName);

        int MaxQuantityOfSizeAvailable(int productId, ProductSize size);
    }
}