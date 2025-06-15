using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using System.ComponentModel.DataAnnotations;

namespace testPortfolio.Models
{
    public class Product
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; } = string.Empty;
        public string? Price { get; set; } = "0.00";

        public List<Picture>? Images { get; set; }
        public DateTime DateAdded { get; set; } = DateTime.Now;
        public bool IsPublic { get; set; } = true;
    }
}
