using System.ComponentModel.DataAnnotations;

public class Product
{
    public Guid Id { get; set; }
    [MaxLength(200)]
    public string? Name { get; set; }
    [MaxLength(100)]
    public string? SKU { get; set; }
    public decimal Price { get; set; }
    public DateTime CreatedAt { get; set; }
}