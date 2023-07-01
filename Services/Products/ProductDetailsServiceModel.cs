namespace NBUniforms.Services.Products
{
    using NBUniforms.Data.Models;
    using System.Collections.Generic;
    
    public class ProductDetailsServiceModel : ProductServiceModel
    {
        public string Description { get; init; }

        public ICollection<ProductSizeQuantity> SizeQuantities { get; set; }

        public bool NoteAfterOrder { get; set; }
    }
}
