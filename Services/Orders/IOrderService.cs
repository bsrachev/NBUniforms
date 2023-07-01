﻿namespace NBUniforms.Services.Orders
{
    using NBUniforms.Models;
    using System.Collections.Generic;

    public interface IOrderService
    {
        int Create(OrderFormServiceModel order, string userId, int cartId);

        OrderQueryServiceModel All(string userId, bool isAdmin);

        double GetOrderTotalPrice(int orderId);

        //OrderFormServiceModel FindById(int id);

        IEnumerable<OrderProductsQueryModel> GetOrderProductsForOrder(int orderId);
        void CompleateOrder(int id);

        void DeleteOrder(int id);

        OrderInfoServiceModel FindById(int id);
    }
}
