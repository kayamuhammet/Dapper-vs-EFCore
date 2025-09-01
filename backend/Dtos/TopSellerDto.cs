public class TopSellerDto
{
    public Guid ProductId { get; set; }
    public string? ProductName { get; set; }
    public int TotalQuantity { get; set; }
    public decimal TotalRevenue { get; set; }
}