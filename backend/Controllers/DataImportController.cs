using System.Data;
using System.Diagnostics;
using backend.Data;
using Bogus;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace backend.Controller;

[ApiController]
[Route("[controller]")]
public class DataImportController : ControllerBase
{
    private readonly AppDbContext _context;
    public DataImportController(AppDbContext context)
    {
        _context = context;
    }
    [HttpPost("efcore-bulk-insert")]
    public async Task<IActionResult> EFCoreBulkInsert(int count = 10000)
    {
        var faker = new Faker<Product>()
            .RuleFor(p => p.Id, f => Guid.NewGuid())
            .RuleFor(p => p.Name, f => f.Commerce.ProductName())
            .RuleFor(p => p.SKU, f => f.Commerce.Ean13())
            .RuleFor(p => p.Price, f => f.Random.Decimal(1, 1000))
            .RuleFor(p => p.CreatedAt, f => f.Date.Past());

        var products = faker.Generate(count); // create a fake product object

        // Starts a timer for performance measurement
        var stopwatch = Stopwatch.StartNew();

        await _context.Products.AddRangeAsync(products);
        await _context.SaveChangesAsync();

        stopwatch.Stop();
        // Stop


        return Ok($"{count} products were added using EF Core in {stopwatch.ElapsedMilliseconds} ms.");
    }

    [HttpPost("dapper-bulk-insert")]
    public async Task<IActionResult> DapperBulkInsert(int count = 10000)
    {
        var faker = new Faker<Product>()
            .RuleFor(p => p.Id, f => Guid.NewGuid())
            .RuleFor(p => p.Name, f => f.Commerce.ProductName())
            .RuleFor(p => p.SKU, f => f.Commerce.Ean13())
            .RuleFor(p => p.Price, f => f.Random.Decimal(1, 1000))
            .RuleFor(p => p.CreatedAt, f => f.Date.Past());

        var products = faker.Generate(count);

        var stopwatch = Stopwatch.StartNew();

        // Create DataTable for SqlBulkCopy
        var dataTable = new DataTable();
        dataTable.Columns.Add("Id", typeof(Guid));
        dataTable.Columns.Add("Name", typeof(string));
        dataTable.Columns.Add("SKU", typeof(string));
        dataTable.Columns.Add("Price", typeof(decimal));
        dataTable.Columns.Add("CreatedAt", typeof(DateTime));

        foreach (var product in products)
        {
            dataTable.Rows.Add(product.Id, product.Name, product.SKU, product.Price, product.CreatedAt);
        }

        // SqlBulkCopy
        using (var connection = new SqlConnection(_context.Database.GetConnectionString()))
        {
            await connection.OpenAsync();

            using (var bulkCopy = new SqlBulkCopy(connection))
            {
                bulkCopy.DestinationTableName = "Products";

                // Column Mapping
                bulkCopy.ColumnMappings.Add("Id", "Id");
                bulkCopy.ColumnMappings.Add("Name", "Name");
                bulkCopy.ColumnMappings.Add("SKU", "SKU");
                bulkCopy.ColumnMappings.Add("Price", "Price");
                bulkCopy.ColumnMappings.Add("CreatedAt", "CreatedAt");

                await bulkCopy.WriteToServerAsync(dataTable);
            }
        }

        stopwatch.Stop();

        return Ok($"{count} products were added using Dapper (SqlBulkCopy) in {stopwatch.ElapsedMilliseconds} ms.");
    }

    // orderCount => how many orders will be created
    // maxItemsPerOrder => The maximum number of items per order
    [HttpPost("generate-orders")]
    public async Task<IActionResult> GenerateOrders(int orderCount = 5000, int maxItemsPerOrder = 5)
    {
        var stopwatch = Stopwatch.StartNew();

        // 1- Get All Product IDs
        var productIds = await _context.Products.Select(p => p.Id).ToListAsync(); // All product IDs
        if (productIds.Count == 0)
        {
            return BadRequest("Please create product data first. Orders must be linked to products.");
        }

        var rand = new Random();
        var newOrders = new List<Order>();
        var newOrderItems = new List<OrderItem>();

        // 2- create fake orders
        var orderFaker = new Faker<Order>()
            .RuleFor(o => o.Id, f => Guid.NewGuid())
            .RuleFor(o => o.OrderDate, f => f.Date.Past(2)); // over the past 2 years

        var orders = orderFaker.Generate(orderCount);

        // 3- Generate random orders
        foreach (var order in orders)
        {
            newOrders.Add(order);
            var itemCount = rand.Next(1, maxItemsPerOrder + 1); // Random number of products per order

            for (int i = 0; i < itemCount; i++)
            {
                var randomProduct = await _context.Products.FindAsync(productIds[rand.Next(0, productIds.Count)]); // random select product

                if (randomProduct == null) continue;

                var orderItemFaker = new Faker<OrderItem>()
                    .RuleFor(oi => oi.Id, f => Guid.NewGuid())
                    .RuleFor(oi => oi.OrderId, f => order.Id)
                    .RuleFor(oi => oi.ProductId, f => randomProduct.Id)
                    .RuleFor(oi => oi.Quantity, f => f.Random.Int(1, 10))
                    .RuleFor(oi => oi.Price, f => randomProduct.Price);

                newOrderItems.Add(orderItemFaker.Generate());
            }
        }

        // 4 - Add to database
        await _context.Orders.AddRangeAsync(newOrders);
        await _context.OrderItems.AddRangeAsync(newOrderItems);
        await _context.SaveChangesAsync();

        stopwatch.Stop();

        return Ok($"{newOrders.Count} orders and {newOrderItems.Count} order items were added via EF Core in {stopwatch.ElapsedMilliseconds} ms.");

    }
}