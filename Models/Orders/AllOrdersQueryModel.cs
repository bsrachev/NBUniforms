namespace NBUniforms.Models.Orders
{
    using NBUniforms.Services.Orders;
    using System.Collections.Generic;

    public class AllOrdersQueryModel
    {
        public IEnumerable<OrderServiceModel> Orders { get; set; }
    }
}
