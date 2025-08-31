public class Order
{
    public Guid Id { get; set; }
    public DateTime OrderDate { get; set; }
    public ICollection<OrderItem>? OrderItems { get; set; }
}