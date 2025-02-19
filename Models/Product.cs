public class Product
{
    public int product_id { get; set; }
    public int product_code { get; set; }
    public string? product_name { get; set; }
    public decimal product_wight { get; set; }
    public decimal product_price { get; set; }
    public int product_quantity { get; set; }

    // Navigation property for related transactions
    public ICollection<Transaction>? Transactions { get; set; }
}
