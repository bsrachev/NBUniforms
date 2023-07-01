﻿namespace NBUniforms.Models.Products
{
    using NBUniforms.Service.Products;
    using NBUniforms.Services.Products;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    using static Data.DataConstants;

    public class AllProductsQueryModel
    {
        public const int ProductsPerPage = ProductsPerPageNumber;

        [Display(Name = "Search by text")]
        public string SearchTerm { get; init; }

        public ProductSorting Sorting { get; init; }

        public int CurrentPage { get; init; } = 1;

        public int TotalProducts { get; set; }

        [Display(Name = "Category")]
        public int CategoryId { get; init; }

        public IEnumerable<ProductCategoryServiceModel> Categories { get; set; }

        public IEnumerable<ProductServiceModel> Products { get; set; }


    }
}
