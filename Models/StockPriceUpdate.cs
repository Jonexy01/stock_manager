using System.ComponentModel.DataAnnotations;

namespace stockmanager.Models
{
    public class StockPriceUpdate
    {
        [Required]
        public Guid Id { get; set; }
        [Required]
        public int NewPrice { get; set; }
    }
}
