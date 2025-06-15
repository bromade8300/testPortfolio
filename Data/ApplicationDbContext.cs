using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

public class ApplicationDbContext : IdentityDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

   public DbSet<testPortfolio.Models.Product> Products { get; set; }
   public DbSet<testPortfolio.Models.Picture> Pictures { get; set; }
}