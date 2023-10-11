using AutoMapper;
using ECommerce.Api.Products.Db;
using ECommerce.Api.Products.Profiles;
using ECommerce.Api.Products.Providers;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace ECommerce.Api.Products.Tests
{
    public class ProductsServiceTest
    {
        [Fact]
        public async Task GetProductsReturnsAllProducts()
        {
            var options = new DbContextOptionsBuilder<ProductsDbContext>()
                .UseInMemoryDatabase(nameof(GetProductsReturnsAllProducts))
                .EnableSensitiveDataLogging(true)
                .Options;
            var dbContext = new ProductsDbContext(options);
            CreateProducts(dbContext);

            var productProfile = new ProductProfile();
            var configuration = new MapperConfiguration(cfg => cfg.AddProfile(productProfile));
            var mapper = new Mapper(configuration);

            var productsProvider = new ProductsProvider(dbContext, null, mapper);

            var products = await productsProvider.GetProductsAsync();
            Assert.True(products.IsSuccess);
            Assert.True(products.Products.Any());
            Assert.Null(products.ErrorMessage);
        }

        [Fact]
        public async Task GetProductReturnsProductUsingValidId()
        {
            var options = new DbContextOptionsBuilder<ProductsDbContext>()
                .UseInMemoryDatabase(nameof(GetProductReturnsProductUsingValidId))
                .EnableSensitiveDataLogging(true)
                .Options;
            var dbContext = new ProductsDbContext(options);
            CreateProducts(dbContext);

            var productProfile = new ProductProfile();
            var configuration = new MapperConfiguration(cfg => cfg.AddProfile(productProfile));
            var mapper = new Mapper(configuration);

            var productsProvider = new ProductsProvider(dbContext, null, mapper);

            var products = await productsProvider.GetProductAsync(101);
            Assert.True(products.IsSuccess);
            Assert.NotNull(products.Product);
            Assert.True(products.Product.Id == 101);
            Assert.Null(products.ErrorMessage);
        }

        [Fact]
        public async Task GetProductReturnsProductUsingInvalidId()
        {
            var options = new DbContextOptionsBuilder<ProductsDbContext>()
                .UseInMemoryDatabase(nameof(GetProductReturnsProductUsingValidId))
                .EnableSensitiveDataLogging(true)
                .Options;
            var dbContext = new ProductsDbContext(options);
            CreateProducts(dbContext);

            var productProfile = new ProductProfile();
            var configuration = new MapperConfiguration(cfg => cfg.AddProfile(productProfile));
            var mapper = new Mapper(configuration);

            var productsProvider = new ProductsProvider(dbContext, null, mapper);

            var products = await productsProvider.GetProductAsync(-1);
            Assert.False(products.IsSuccess);
            Assert.Null(products.Product);
            Assert.NotNull(products.ErrorMessage);
        }

        private void CreateProducts(ProductsDbContext dbContext)
        {
            for (int i = 1; i <= 10; i++)
            {
                dbContext.Products.Add(new Product()
                {
                    Id = i + 100,
                    Name = Guid.NewGuid().ToString(),
                    Inventory = i + 10,
                    Price = (decimal)(i * 3.14)
                });
            }
        }
    }
}