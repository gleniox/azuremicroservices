using AutoMapper;
using ECommerce.Api.Orders.Db;
using ECommerce.Api.Orders.Interfaces;
using ECommerce.Api.Orders.Models;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Api.Orders.Providers
{
    public class OrdersProvider : IOrdersProvider
    {
        private readonly OrdersDbContext dbContext;
        private readonly ILogger<OrdersProvider> logger;
        private readonly IMapper mapper;

        public OrdersProvider(OrdersDbContext dbContext, ILogger<OrdersProvider> logger, IMapper mapper)
        {
            this.dbContext = dbContext;
            this.logger = logger;
            this.mapper = mapper;

            SeedData();
        }

        private void SeedData()
        {
            if (!dbContext.Orders.Any())
            {

                dbContext.Orders.Add(new Db.Order
                {
                    Id = 1,
                    CustomerId = 1,
                    OrderDate = DateTime.UtcNow,
                    Total = 800.00M,
                    Items = new List<Db.OrderItem>
                {
                    new Db.OrderItem { Id = 1, OrderId = 1, ProductId = 1, Quantity = 10, UnitPrice = 10.00M },
                    new Db.OrderItem { Id = 2, OrderId = 1, ProductId = 2, Quantity = 20, UnitPrice = 5.00M },
                    new Db.OrderItem { Id = 3, OrderId = 1, ProductId = 3, Quantity = 30, UnitPrice = 20.00M }
                }
                });

                dbContext.Orders.Add(new Db.Order
                {
                    Id = 2,
                    CustomerId = 1,
                    OrderDate = DateTime.UtcNow,
                    Total = 200.00M,
                    Items = new List<Db.OrderItem>
                {
                    new Db.OrderItem { Id = 4, OrderId = 2, ProductId = 1, Quantity = 10, UnitPrice = 10.00M },
                    new Db.OrderItem { Id = 5, OrderId = 2, ProductId = 2, Quantity = 20, UnitPrice = 5.00M },
                }
                });

                dbContext.SaveChanges();
            }
        }

        public async Task<(bool IsSuccess, IEnumerable<Models.Order> Orders, string ErrorMessage)> GetOrdersAsync(int customerId)
        {
            try
            {
                var orders = await dbContext.Orders.Where(p => p.CustomerId == customerId).ToListAsync();

                if (orders != null && orders.Any())
                {
                    var result = mapper.Map<IEnumerable<Db.Order>, IEnumerable<Models.Order>>(orders);
                    return (true, result, null);
                }

                return (false, null, "Not found!");
            }
            catch (Exception ex)
            {
                logger?.LogError(ex.ToString());
                return (false, null, ex.Message);
            }
        }
    }
}
