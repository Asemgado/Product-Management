public class Transaction
{
    public int transaction_id { get; set; }
    public int product_id { get; set; }
    public decimal transaction_amount { get; set; }
    public DateTime transaction_date { get; set; }
    public string? transaction_type { get; set; }
    public decimal transaction_price { get; set; }

    // Navigation property for related product
    public Product? Product { get; set; }
}
