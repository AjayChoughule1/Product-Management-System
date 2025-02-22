using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Product_Management_System.Models;
using System.Data.SqlClient;

namespace Product_Management_System.Controllers
{
    public class ProductController : Controller
    {
        private readonly string connectionString = "Server=LAPTOP-S2EFS1EF\\SQLEXPRESS;Database=Product_Management;Trusted_Connection=true";

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                return RedirectToAction("Index");
            }
            return View(model);
        }

        public IActionResult Index()
        {
            ViewBag.Categories = GetCategories();
            return View(GetProducts());
        }

        [HttpPost]
        public IActionResult AddProduct(ProductViewModel model)
        {
            if (ModelState.IsValid)
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    string query = "INSERT INTO Product VALUES (@ProductCode, @ProductName, @CategoryId, @Quantity, @Price)";
                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@ProductCode", model.ProductCode);
                    cmd.Parameters.AddWithValue("@ProductName", model.ProductName);
                    cmd.Parameters.AddWithValue("@CategoryId", model.CategoryId);
                    cmd.Parameters.AddWithValue("@Quantity", model.Quantity);
                    cmd.Parameters.AddWithValue("@Price", model.Price);
                    con.Open();
                    cmd.ExecuteNonQuery();
                    con.Close();
                }
                return RedirectToAction("Index");
            }
            ViewBag.Categories = GetCategories();
            return View("Index", GetProducts());
        }

        [HttpPost]
        public IActionResult DeleteProducts(List<int> productIds)
        {
            if (productIds != null && productIds.Any())
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    string ids = string.Join(",", productIds);
                    string query = $"DELETE FROM Product WHERE ProductId IN ({ids})";
                    SqlCommand cmd = new SqlCommand(query, con);
                    con.Open();
                    cmd.ExecuteNonQuery();
                    con.Close();
                }
            }
            return RedirectToAction("Index");
        }

        public IActionResult SortProducts(string sortOrder)
        {
            // It is used for sorting the records
            var products = GetProducts();
            switch (sortOrder)
            {
                case "NameAsc":
                    products = products.OrderBy(p => p.ProductName).ToList();
                    break;
                case "NameDesc":
                    products = products.OrderByDescending(p => p.ProductName).ToList();
                    break;
            }
            ViewBag.Categories = GetCategories();
            return View("Index", products);
        }

        private List<SelectListItem> GetCategories()
        {
            List<SelectListItem> categories = new List<SelectListItem>();
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand("SELECT CategoryId, CategoryName FROM Category", con);
                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    categories.Add(new SelectListItem
                    {
                        Value = reader["CategoryId"].ToString(),
                        Text = reader["CategoryName"].ToString()
                    });
                }
                con.Close();
            }
            return categories;
        }

        private List<ProductViewModel> GetProducts()
        {
            List<ProductViewModel> products = new List<ProductViewModel>();
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand("SELECT * FROM Product", con);
                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    products.Add(new ProductViewModel
                    {
                        ProductId = Convert.ToInt32(reader["ProductId"]),
                        ProductCode = reader["ProductCode"].ToString(),
                        ProductName = reader["ProductName"].ToString(),
                        CategoryId = Convert.ToInt32(reader["CategoryId"]),
                        Quantity = Convert.ToInt32(reader["Quantity"]),
                        Price = Convert.ToDecimal(reader["Price"]),
                        Amount = Convert.ToInt32(reader["Quantity"]) * Convert.ToDecimal(reader["Price"])
                    });
                }
                con.Close();
            }
            return products;
        }
    }
}
