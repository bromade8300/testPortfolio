using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using System.ComponentModel.DataAnnotations;

namespace testPortfolio.Models
{
    public class Card
    {
        public string name { get; set; } = string.Empty;
        public string? shortDescription { get; set; } = string.Empty;
        public string? description { get; set; } = string.Empty;

        public string? imagePath { get; set; } = string.Empty;

        public string? secondaryImagePath { get; set; } = string.Empty;

        public string? price { get; set; } = "0.00";
    }
}
