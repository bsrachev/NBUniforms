﻿namespace NBUniforms.Controllers
{
    using System;
    using System.Linq;
    using System.Collections.Generic;
    using NBUniforms.Services.Products;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Caching.Memory;

    using static WebConstants.Cache;

    public class HomeController : Controller
    {
        private readonly IProductService products;
        private readonly IMemoryCache cache;

        public HomeController(IProductService products, IMemoryCache cache)
        {
            this.products = products;
            this.cache = cache;
        }

        public IActionResult Index()
        {
            /*var latestProducts = this.cache.Get<List<LatestProductServiceModel>>(LatestProductsCacheKey);

            if (latestProducts == null)
            {
                latestProducts = this.products
                   .Latest()
                   .ToList();

                var cacheOptions = new MemoryCacheEntryOptions()
                    .SetAbsoluteExpiration(TimeSpan.FromMinutes(15));

                this.cache.Set(LatestProductsCacheKey, latestProducts, cacheOptions);
            }*/

            var latestProducts = this.products
                   .Latest()
                   .ToList();

            return View(latestProducts);
        }

        public IActionResult Error() => View();
    }
}
