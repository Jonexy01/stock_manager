using System.ComponentModel.DataAnnotations;

namespace stockmanager.Models
{
    public class Stock
    {
        [Required]
        public Guid Id { get; set; }
        [Required]
        public string Name { get; set; }
        public string Type { get; set; }
        [Required]
        public int Price { get; set; }
        [Required]
        public int Quantity { get; set; }
        public DateTime Created { get; set; }
    }
}
