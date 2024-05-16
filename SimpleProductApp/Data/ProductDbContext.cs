using Microsoft.EntityFrameworkCore;
using SimpleProductApp.Models;

namespace SimpleProductApp.Data;

public class ProductDbContext : DbContext
{
	public DbSet<Product> Products { get; set; }
	public ProductDbContext(DbContextOptions<ProductDbContext> options) 
		: base(options)
	{
		
	}
}