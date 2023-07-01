namespace NBUniforms.Controllers
{
    using AutoMapper;
    using NBUniforms.Data.Models;
    using NBUniforms.Data.Models.Enums;
    using NBUniforms.Models.Products;
    using NBUniforms.Services.Products;
    using NBUniforms.Services.Carts;
    using NBUniforms.Infrastructure;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using System.Collections.Generic;
    using System.Linq;

    using static Data.DataConstants;

    public class ProductsController : Controller
    {
        private readonly IProductService products;
        private readonly ICartService carts;
        private readonly IMapper mapper;

        public ProductsController(IProductService products, ICartService carts, IMapper mapper)
        {
            this.products = products;
            this.carts = carts;
            this.mapper = mapper;
        }

        public IActionResult All([FromQuery] AllProductsQueryModel query)
        {
            var queryResult = this.products.All(
                query.CategoryId,
                query.SearchTerm,
                query.Sorting,
                query.CurrentPage,
                AllProductsQueryModel.ProductsPerPage);

            var categories = this.products.AllCategories();

            query.TotalProducts = queryResult.TotalProducts;
            query.Products = queryResult.Products;
            query.Categories = categories;

            return View(query);
        }

        [Authorize(Roles = WebConstants.AdministratorRoleName)]
        public IActionResult Add() => View(new ProductFormServiceModel
        {
            Categories = this.products.AllCategories() //they are null so initializing them
        });

        [HttpPost]
        [Authorize(Roles = WebConstants.AdministratorRoleName)]
        public IActionResult Add(ProductFormServiceModel product)
        {
            if (!this.products.CategoryExists(product.CategoryId)) //validation - attributes cant
            {
                this.ModelState.AddModelError(nameof(product.CategoryId), "Category does not exist.");
            }

            string convertedName = this.products.CreateConvertedName(product);

            if (this.products.ConvertedNameExists(convertedName)) //validates that the converted name is unique
            {
                product.Categories = this.products.AllCategories();

                this.ModelState.AddModelError(nameof(product.Name), "A product with that name already exists.");

                return View(product);
            }

            if (product.QuantityS + product.QuantityM + product.QuantityL < 1)
            {
                this.ModelState.AddModelError("allSizesAreZero", AllSizesAreZeroErrMsg);
            }

            if (!ModelState.IsValid)
            {
                product.Categories = this.products.AllCategories(); 

                return View(product); 
            }

            var productId = this.products.Create(product, convertedName);

            this.products.AddQuantities(product, ProductQuantityMin, productId);

            return RedirectToAction(nameof(All));
        }

        [Authorize(Roles = WebConstants.AdministratorRoleName)]
        public IActionResult Edit(int id)
        {
            var product = this.products.FindById(id);

            ICollection<ProductSizeQuantity> allSizesForCurrentProduct = this.products.ProductSizeQuantity(product.Id);

            var productForm = this.mapper.Map<ProductFormServiceModel>(product);

            //shound be "set", not "init"
            productForm.Categories = this.products.AllCategories();
            productForm.QuantityS = allSizesForCurrentProduct.Where(x => x.Size == ProductSize.S).FirstOrDefault().Quantity;
            productForm.QuantityM = allSizesForCurrentProduct.Where(x => x.Size == ProductSize.M).FirstOrDefault().Quantity;
            productForm.QuantityL = allSizesForCurrentProduct.Where(x => x.Size == ProductSize.L).FirstOrDefault().Quantity;

            return View(productForm);
        }

        [HttpPost]
        [Authorize(Roles = WebConstants.AdministratorRoleName)]
        public IActionResult Edit(int id, ProductFormServiceModel product)
        {
            if (!this.products.CategoryExists(product.CategoryId))
            {
                this.ModelState.AddModelError(nameof(product.CategoryId), "Category does not exist.");
            }

            if (!ModelState.IsValid)
            {
                product.Categories = this.products.AllCategories();

                return View(product);
            }

            string newConvertedName = this.products.CreateConvertedName(product);

            var productIsEdited = this.products.Edit(
                id,
                product.Name,
                product.Description,
                product.ImageUrl,
                product.Price,
                product.CategoryId,
                newConvertedName,
                product.QuantityS,
                product.QuantityM,
                product.QuantityL);

            if (!productIsEdited)
            {
                return BadRequest();
            }

            return RedirectToAction(nameof(All));
        }

        [Authorize(Roles = WebConstants.AdministratorRoleName)]
        public IActionResult Delete(int id)
        {
            var product = this.products.Find(id);

            ICollection<ProductSizeQuantity> allSizesForCurrentProduct = this.products.ProductSizeQuantity(product.Id);

            foreach (var psq in allSizesForCurrentProduct)
            {
                this.products.RemoveProductSizeQuantities(psq);
            }

            this.products.Remove(product);

            return RedirectToAction(nameof(All));
        }

        public IActionResult Details(int id)
        {
            var product = this.products.FindById(id);

            var productCategory = this.products.ProductCategory(product.CategoryName);

            ICollection<ProductSizeQuantity> allSizesForCurrentProduct = this.products.ProductSizeQuantity(product.Id);

            return View(new ProductDetailsServiceModel
            {
                Id = product.Id,
                Name = product.Name,
                ImageUrl = product.ImageUrl,
                Description = product.Description,
                Price = product.Price,
                CategoryId = productCategory.Id,
                CategoryName = product.CategoryName,
                SizeQuantities = allSizesForCurrentProduct
            });
        }

        [HttpPost]
        public IActionResult Details(ProductSizeQuantityServiceModel productModel)
        {
            var quantityAvailable = this.products.MaxQuantityOfSizeAvailable(productModel.Id, productModel.Size);

            var product = this.products.FindById(productModel.Id);

            var successfulOrder = true;

            if (productModel.Quantity > quantityAvailable || productModel.Quantity <= 0)
            {
                this.ModelState.AddModelError(nameof(productModel.Quantity),
                    $"The quantity is unvailable. There are only {quantityAvailable} items with size {productModel.Size} left.");

                successfulOrder = false;
            }
            else
            {
                var currentUserCart = this.carts.GetUserCart(this.User.Id());

                bool cartProductAlreadyExists = this.carts.IsThisProductWithThisSizeInCart(currentUserCart.Id, productModel.Id, productModel.Size);

                if (cartProductAlreadyExists)
                {
                    this.ModelState.AddModelError(nameof(productModel.Size),
                    $"You already have this product with this size in your cart.");

                    successfulOrder = false;
                }
                else
                {
                    var cartProduct = new CartProduct
                    {
                        CartId = currentUserCart.Id,
                        ProductId = productModel.Id,
                        Quantity = productModel.Quantity,
                        Size = productModel.Size
                    };

                    this.products.AddProductToCart(cartProduct);
                }
            }

            var productCategory = this.products.ProductCategory(product.CategoryName);

            ICollection<ProductSizeQuantity> allSizesForCurrentProduct = this.products.ProductSizeQuantity(product.Id);

            return View(new ProductDetailsServiceModel
            {
                Id = product.Id,
                Name = product.Name,
                ImageUrl = product.ImageUrl,
                Description = product.Description,
                Price = product.Price,
                CategoryId = productCategory.Id,
                CategoryName = product.CategoryName,
                SizeQuantities = allSizesForCurrentProduct,
                NoteAfterOrder = successfulOrder
        });
        }
    }
}