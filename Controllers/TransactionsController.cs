using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Diagnostics.Eventing.Reader;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using Product_Management.Models;


namespace Product_Management.Controllers;
public class TransactionsController : Controller
{
    private readonly ILogger<TransactionsController> _logger;
    private readonly DbHelper _dbHelper;
    private List<Product> products = new();

    public TransactionsController(ILogger<TransactionsController> logger, DbHelper dbHelper)
    {
        _logger = logger;
        _dbHelper = dbHelper;
    }

    public IActionResult Transaction()
    {
        _logger.LogInformation("Transaction page visited");
        products = new List<Product>();
        using (var connection = _dbHelper.GetConnection())
        {
            connection.Open();
            using (var command = new SqlCommand("SELECT * FROM Products", connection))
            {
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        products.Add(new Product
                        {
                           product_id = reader.GetInt32(0),
                            product_code = reader.GetInt32(1),
                            product_name = reader.GetString(2),
                            product_wight = reader.GetDecimal(3),
                            product_price = reader.GetDecimal(4),
                            product_quantity = reader.GetInt32(5)
                        });
                    }
                }
            }
        }
        ViewBag.Products = new SelectList(products, "product_code", "product_name");
        return View();
    }

    [HttpPost]
    public IActionResult Transaction(Transaction transaction)
    {
        _logger.LogInformation("Transaction page visited");
        using (var connection = _dbHelper.GetConnection())
        {
            connection.Open();
            string updateQuery = "";
            
            if (transaction.transaction_type == "SELL")
            {
                var product = products.Find(p => p.product_id == transaction.product_id);
                if (product == null)
                {
                    return RedirectToAction("Error", new {message = "Product not found"});
                }
                if (product.product_quantity < transaction.transaction_amount)
                {
                    return RedirectToAction("Error", new {message = "There is not enough product in stock"});
                }
                else updateQuery = "UPDATE Products SET product_quantity = product_quantity - @Amount WHERE product_id = @ProductId";
            }
            
            else if (transaction.transaction_type == "BUY")
            {
                updateQuery = "UPDATE Products SET product_quantity = product_quantity + @Amount WHERE product_id = @ProductId";
            }
            
            using (var command = new SqlCommand(updateQuery, connection))
            {
                command.Parameters.AddWithValue("@ProductId", transaction.product_id);
                command.Parameters.AddWithValue("@Amount", transaction.transaction_amount);
                command.ExecuteNonQuery();
            }
        }
        using (var connection = _dbHelper.GetConnection())
        {
            var product = products.Find(p => p.product_id == transaction.product_id);
            if (product == null)
            {
                return RedirectToAction("Error", new {message = "Product not found"});
            }
            decimal product_price = product.product_price;
            connection.Open();
            string query = @"INSERT INTO Transactions 
                            (product_id, transaction_amount, transaction_date, transaction_type, transaction_price)
                            VALUES (@ProductId, @Amount, @Date, @Type, @Price)";
            
            using (var command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@ProductId", transaction.product_id);
                command.Parameters.AddWithValue("@Amount", transaction.transaction_amount);
                command.Parameters.AddWithValue("@Date", transaction.transaction_date);
                command.Parameters.AddWithValue("@Type", transaction.transaction_type);
                command.Parameters.AddWithValue("@Price", product_price * transaction.transaction_amount);
                command.ExecuteNonQuery();
            }
        }
        
        return RedirectToAction("Transactions");
    }
    public IActionResult Error(string message)
    {
        ViewBag.Message = message;
        return View();
    }
    
}