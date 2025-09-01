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
        

        return Ok($"EF Core ile {count} adet ürün {stopwatch.ElapsedMilliseconds} ms'de eklendi.");
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

        return Ok($"Dapper (SqlBulkCopy) ile {count} adet ürün {stopwatch.ElapsedMilliseconds} ms'de eklendi.");
    }
}