﻿namespace NBUniforms.Models
{
    using NBUniforms.Data.Models;
    using NBUniforms.Data.Models.Enums;
    using System.Collections.Generic;

    public class CartProductsQueryModel
    {
        public int Id { get; init; }

        public int ProductId { get; init; }

        public string ProductName { get; init; }

        public string ProductImageUrl { get; init; }

        public ProductSize Size { get; set; }

        public int Quantity { get; set; }

        public int MaxQuantityAvailable { get; set; }

        public double Price { get; set; }

        public double ProductTotalPrice { get; set; }

        public ICollection<ProductSizeQuantity> SizeQuantities { get; set; }
    }
}
