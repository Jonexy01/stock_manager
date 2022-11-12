using System.ComponentModel.DataAnnotations;

namespace stockmanager.Models
{
    public class AddStockRequest
    {
        [Required]
        public string Name { get; set; }
        public string Type { get; set; }
        [Required]
        public int Price { get; set; }
        [Required]
        public int Quantity { get; set; }
    }
}
