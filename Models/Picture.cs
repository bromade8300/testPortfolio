using System.ComponentModel.DataAnnotations;

namespace testPortfolio.Models
{
    public class Picture
    {
        [Key]
        public int Id { get; set; }
        public string name { get; set; } = string.Empty;
        public PictureUsage usage { get; set; }
        public string path { get; set; } = string.Empty;

        public string? description { get; set; }
        public DateTime dateAdded { get; set; } = DateTime.Now;
        public bool isPublic { get; set; } = true;
    }
}
