﻿namespace NBUniforms.Services.Products
{
    using System.Collections.Generic;
    using System.Linq;
    using NBUniforms.Models;
    using NBUniforms.Data;
    using NBUniforms.Data.Models;
    using NBUniforms.Service.Products;
    using NBUniforms.Data.Models.Enums;
    using AutoMapper.QueryableExtensions;
    using AutoMapper;

    public class ProductService : IProductService
    {
        private readonly NBUniformsDbContext data;
        private readonly IConfigurationProvider mapper;

        public ProductService(NBUniformsDbContext data, IMapper mapper)
        { 
            this.data = data;
            this.mapper = mapper.ConfigurationProvider;
        } 

        public int Create(ProductFormServiceModel product, string convertedName)
        {
            var productData = new Product
            {
                Name = product.Name,
                ConvertedName = convertedName,
                Price = product.Price,
                Description = product.Description,
                ImageUrl = product.ImageUrl,
                CategoryId = product.CategoryId
            };

            this.data.Products.Add(productData);
            this.data.SaveChanges();

            return productData.Id;
        }

        public bool Edit(int id, string name, string description, string imageUrl, double price, int categoryId, string convertedName, int quantityS, int quantityM, int quantityL)
        {
            var productData = this.data.Products.Find(id);

            productData.Name = name;
            productData.Description = description;
            productData.ImageUrl = imageUrl;
            productData.Price = price;
            productData.CategoryId = categoryId;
            productData.ConvertedName = convertedName;

            var productDataQuantities = this.data.ProductSizeQuantities.Where(p => p.ProductId == id).ToList();

            productDataQuantities.Where(x => x.Size == ProductSize.S).FirstOrDefault().Quantity = quantityS;
            productDataQuantities.Where(x => x.Size == ProductSize.M).FirstOrDefault().Quantity = quantityM;
            productDataQuantities.Where(x => x.Size == ProductSize.L).FirstOrDefault().Quantity = quantityL;

            this.data.SaveChanges();

            return true;
        }

        public void Remove(Product product)
        {
            this.data.Products.Remove(product);
            this.data.SaveChanges();
        }

        public void AddProductToCart(CartProduct cartProduct)
        {
            this.data.CartProducts.Add(cartProduct);
            this.data.SaveChanges();
        }

        public IEnumerable<LatestProductServiceModel> Latest()
            => this.data
                .Products
                .OrderByDescending(p => p.Id)  
                .ProjectTo<LatestProductServiceModel>(this.mapper)
                .Take(3)
                .ToList();

        public ProductQueryServiceModel All(
            int categoryId,
            string searchTerm,
            ProductSorting sorting,
            int currentPage,
            int productsPerPage)
        {
            var productsQuery = this.data.Products.AsQueryable();

            if (categoryId != 0)
            {
                productsQuery = productsQuery.Where(c => c.CategoryId == categoryId);
            }
            
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                productsQuery = productsQuery.Where(p =>
                    (p.Name).ToLower().Contains(searchTerm.ToLower()) ||
                    p.Description.ToLower().Contains(searchTerm.ToLower()));
            }

            productsQuery = sorting switch
            {
                ProductSorting.LastAdded => productsQuery.OrderByDescending(p => p.Id),
                ProductSorting.FirstAdded => productsQuery.OrderBy(p => p.Id),
                ProductSorting.TheMostExpensive => productsQuery.OrderByDescending(p => p.Price),
                ProductSorting.TheCheapest or _ => productsQuery.OrderBy(p => p.Price),
            };

            var totalProducts = productsQuery.Count();

            var products = productsQuery
                //.OrderByDescending(p => p.Id)  //this way the sortings wont work
                .Skip((currentPage - 1) * productsPerPage)
                .Take(productsPerPage)
                .Select(p => new ProductServiceModel
                {
                    Id = p.Id,
                    Name = p.Name,
                    ConvertedName = p.ConvertedName,
                    Price = p.Price,
                    ImageUrl = p.ImageUrl,
                    CategoryId = p.CategoryId,
                    CategoryName = p.Category.Name
                })
                .ToList();

            return new ProductQueryServiceModel
            {
                TotalProducts = totalProducts,
                CurrentPage = currentPage,
                ProductsPerPage = productsPerPage,
                Products = products,
            };
        }

        public Product Find(int id)
            => this.data.Products.Find(id);

        public ProductDetailsServiceModel FindById(int id)
            => this.data
            .Products
            .Where(p => p.Id == id)
            .Select(p => new ProductDetailsServiceModel
            {
                Id = p.Id,
                Name = p.Name,
                ConvertedName = p.ConvertedName,
                Description = p.Description,
                Price = p.Price,
                ImageUrl = p.ImageUrl,
                CategoryId = p.CategoryId,
                CategoryName = p.Category.Name
            })
            .FirstOrDefault();

        public ProductDetailsServiceModel FindByConvertedName(string convertedName)
            => this.data
            .Products
            .Where(p => p.ConvertedName == convertedName)
            .Select(p => new ProductDetailsServiceModel
            {
                Id = p.Id,
                Name = p.Name,
                ConvertedName = p.ConvertedName,
                Description = p.Description,
                Price = p.Price,
                ImageUrl = p.ImageUrl,
                CategoryId = p.CategoryId,
                CategoryName = p.Category.Name
            })
            .FirstOrDefault();

        public ICollection<ProductSizeQuantity> ProductSizeQuantity(int productId)
            => this.data.ProductSizeQuantities.Where(p => p.ProductId == productId).ToList();

        public void RemoveProductSizeQuantities(ProductSizeQuantity psq)
        {
            this.data.ProductSizeQuantities.Remove(psq);
        }

        public void AddQuantities(ProductFormServiceModel product, int ProductQuantityMin, int productId)
        {
            if (product.QuantityS >= ProductQuantityMin)
            {
                this.data.ProductSizeQuantities.Add(CreateProductSizeQuantity(ProductSize.S, product.QuantityS, productId));
                this.data.SaveChanges();
            }
            if (product.QuantityM >= ProductQuantityMin)
            {
                this.data.ProductSizeQuantities.Add(CreateProductSizeQuantity(ProductSize.M, product.QuantityM, productId));
                this.data.SaveChanges();
            }
            if (product.QuantityL >= ProductQuantityMin)
            {
                this.data.ProductSizeQuantities.Add(CreateProductSizeQuantity(ProductSize.L, product.QuantityL, productId));
                this.data.SaveChanges();
            }
        }

        public IEnumerable<ProductCategoryServiceModel> AllCategories()
        => this.data
                .Categories
                .Select(p => new ProductCategoryServiceModel
                {
                    Id = p.Id,
                    Name = p.Name
                })
                .ToList();

        public Category ProductCategory(string productCategoryName)
            => this.data.Categories.FirstOrDefault(c => c.Name == productCategoryName);

        public bool CategoryExists(int categoryId)
        => this.data
            .Categories
            .Any(p => p.Id == categoryId);

        public int MaxQuantityOfSizeAvailable(int productId, ProductSize size)
        => this.data
            .ProductSizeQuantities
            .Where(p => p.ProductId == productId && p.Size == size)
            .FirstOrDefault()
            .Quantity;

        public string CreateConvertedName(ProductFormServiceModel product)
        {
            string convertedName = product.Name.ToLower();
            string[] nameWords = product.Name.Split(' ');
            foreach (var word in nameWords)
            {
                convertedName += "-" + word.ToLower();
            }

            return convertedName;
        }

        public bool ConvertedNameExists(string convertedName)
            => this.data.Products.Any(p => p.ConvertedName == convertedName);

        private ProductSizeQuantity CreateProductSizeQuantity(ProductSize size, int quantity, int productId)
        {
            var product = this.data.Products.Find(productId);

            var productData = new ProductSizeQuantity
            {
                ProductId = product.Id,
                Size = (ProductSize)size,
                Quantity = quantity
            };

            return productData;
        }
    }
}