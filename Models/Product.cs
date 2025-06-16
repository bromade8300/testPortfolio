using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using System.ComponentModel.DataAnnotations;

namespace testPortfolio.Models
{
    public class Product
    {
        [Key]
        public int Id { get; set; }
        public string name { get; set; } = string.Empty;
        public string? description { get; set; } = string.Empty;
        public string? price { get; set; } = "0.00";

        public List<Picture>? images { get; set; }
        public DateTime dateAdded { get; set; } = DateTime.Now;
        public bool isPublic { get; set; } = true;
    }
}
