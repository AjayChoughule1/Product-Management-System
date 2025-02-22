using System.ComponentModel.DataAnnotations;

namespace Product_Management_System.Models
{
    public class ProductViewModel
    {
        public int ProductId { get; set; }
        [Required]
        public string ProductCode { get; set; }
        public string ProductName { get; set; }
        public int CategoryId { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public decimal Amount { get; set; }
    }
}
