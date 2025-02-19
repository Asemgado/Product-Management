
namespace Product_Management.Models
{
    public class TransactionsViewModel
    {
        public int transaction_id { get; set; }
        public string? product_name { get; set; }
        public int product_code { get; set; }
        public decimal transaction_amount { get; set; }
        public DateTime transaction_date { get; set; }
        public string? transaction_type { get; set; }
        public decimal transaction_price { get; set; }

       
    }
}
