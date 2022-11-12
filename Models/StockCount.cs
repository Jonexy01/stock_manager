using System.ComponentModel.DataAnnotations;

namespace stockmanager.Models
{
    public class StockCount
    {
        [Required]
        public Guid Id { get; set; }
        [Required]
        public int QuantityToOperate { get; set; }
        [Required]
        public string operation { get; set; }
    }
}
