using System.CodeDom.Compiler;
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Product_Management.Models;


namespace Product_Management.Controllers;

public class ProductsController : Controller
{
    private readonly ILogger<ProductsController> _logger;

    private readonly DbHelper _dbHelper;

    public ProductsController(ILogger<ProductsController> logger, DbHelper dbHelper)
    {
        _logger = logger;
        _dbHelper = dbHelper;
    }

    [HttpGet]
    public IActionResult Enter()
    {
        return View();
    }

    [HttpPost]
    public IActionResult Enter(Product product)
    {
        _logger.LogInformation($"Enter new product: {product.product_name}");
        using (var connection = _dbHelper.GetConnection())
        {
            connection.Open();
            string query = "INSERT INTO Products (product_code, product_name, product_wight, product_price, product_quantity) VALUES (@product_code, @product_name, @product_wight, @product_price, @product_quantity)";
            using (var command = new SqlCommand(query, connection))
            {
                int code = GeneratedCode();
                command.Parameters.AddWithValue("@product_code", code);
                command.Parameters.AddWithValue("@product_name", product.product_name);
                command.Parameters.AddWithValue("@product_wight", product.product_wight);
                command.Parameters.AddWithValue("@product_price", product.product_price);
                command.Parameters.AddWithValue("@product_quantity", product.product_quantity);
                command.ExecuteNonQuery();
            }
        }
        return RedirectToAction("Index");
    }

    [HttpGet]
    public IActionResult Index()
    {
        _logger.LogInformation($"Get all products");
        List<Product> products = new List<Product>();
        using (var connection = _dbHelper.GetConnection())
        {
            connection.Open();
            string query = "SELECT * FROM Products";
            using (var command = new SqlCommand(query, connection))
            {
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Product product = new Product
                        {
                            product_id = reader.GetInt32(0),
                            product_code = reader.GetInt32(1),
                            product_name = reader.GetString(2),
                            product_wight = reader.GetDecimal(3),
                            product_price = reader.GetDecimal(4),
                            product_quantity = reader.GetInt32(5)
                        };
                        products.Add(product);
                    }
                }
            }
        }
        return View(products);
    }

    [HttpGet]
    public IActionResult Delete(int id)
    {
        _logger.LogInformation($"Delete product with id: {id}");
        using (var connection = _dbHelper.GetConnection())
        {
            connection.Open();
            string query = "DELETE FROM Products WHERE product_id = @product_id";
            using (var command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@product_id", id);
                command.ExecuteNonQuery();
            }
        }
        return RedirectToAction("Index");
    }


    private static readonly Random _random = new Random();
    
    private int GeneratedCode()
    {
        return _random.Next(1000, 9999);
    }
}