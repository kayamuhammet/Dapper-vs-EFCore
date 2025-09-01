using System.Diagnostics;
using backend.Data;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("[controller]")]
public class ReportsController : ControllerBase
{
    private readonly AppDbContext _context;

    public ReportsController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet("topsellers-efcore")]
    public async Task<IActionResult> GetTopSellersEfCore()
    {
        var stopwatch = Stopwatch.StartNew();

        var report = await _context.OrderItems
            .AsNoTracking()
            .GroupBy(x => new { x.ProductId, x.Product!.Name })
            .Select(g => new TopSellerDto
            {
                ProductId = g.Key.ProductId,
                ProductName = g.Key.Name,
                TotalQuantity = g.Sum(x => x.Quantity),
                TotalRevenue = g.Sum(x => x.Quantity * x.Price)
            })
            .OrderByDescending(r => r.TotalRevenue)
            .Take(20)
            .ToListAsync();

        stopwatch.Stop();

        return Ok(new { ElapsedMilliseconds = stopwatch.ElapsedMilliseconds, Data = report });
    }

    [HttpGet("topsellers-dapper")]
    public async Task<IActionResult> GetTopSellersDapper()
    {
        var stopwatch = Stopwatch.StartNew();

        var sql = @"
            SELECT TOP 20
                p.Id AS ProductId,
                p.Name AS ProductName,
                SUM(x.Quantity) AS TotalQuantity,
                SUM(x.Quantity * x.Price) AS TotalRevenue
            FROM OrderItems x
            JOIN Products p ON x.ProductId = p.Id
            GROUP BY p.Id, p.Name
            ORDER BY TotalRevenue DESC";

        using (var connection = new SqlConnection(_context.Database.GetConnectionString()))
        {
            var report = await connection.QueryAsync<TopSellerDto>(sql);
            stopwatch.Stop();

            return Ok(new { ElapsedMilliseconds = stopwatch.ElapsedMilliseconds, Data = report.ToList() });
        }
    }

}