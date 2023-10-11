using ECommerce.Api.Search.Interfaces;

namespace ECommerce.Api.Search.Services
{
    public class SearchService : ISearchService
    {
        private readonly IOrdersService ordersService;
        private readonly IProductsService productsService;
        private readonly ICustomersService customersService;

        public SearchService(IOrdersService ordersService, IProductsService productsService, ICustomersService customersService)
        {
            this.ordersService = ordersService;
            this.productsService = productsService;
            this.customersService = customersService;
        }

        public async Task<(bool IsSuccess, dynamic SearchResults)> SearchAsync(int customerId)
        {
            var ordersResult = await ordersService.GetOrdersAsync(customerId);
            var productsResult = await productsService.GetProductsAsync();
            var customersResult = await customersService.GetCustomerAsync(customerId);

            if (ordersResult.IsSuccess)
            {

                foreach (var order in ordersResult.Orders)
                {
                    foreach (var orderItem in order.Items)
                    {
                        orderItem.ProductName = productsResult.IsSuccess
                            ? productsResult.Products.FirstOrDefault(p => p.Id == orderItem.ProductId)?.Name
                            : "Product information is not available";
                    }
                }

                var result = new
                {
                    Customer = customersResult.IsSuccess ? customersResult.Customer
                            : new { Name = "Customer information is not available" },
                    Orders = ordersResult.Orders
                };

                return (true, result);
            }

            return (false, null);
        }
    }
}
