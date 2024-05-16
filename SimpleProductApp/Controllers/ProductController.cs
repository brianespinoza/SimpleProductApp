using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using SimpleProductApp.Data;
using SimpleProductApp.Models;

namespace SimpleProductApp.Controllers;

[Route("api/v1/[controller]")]
[ApiController]
public class ProductController : ControllerBase
{
	private readonly ProductDbContext _context;

	public ProductController(ProductDbContext context)
	{
		_context = context;
	}

	[HttpGet]
	public async Task<ActionResult<IEnumerable<Product>>> GetProducts()
	{
		return await _context.Products.ToListAsync();
	}

	[HttpGet("{id:int}")]
	public async Task<ActionResult<Product>> GetProduct(int id)
	{
		var product = await _context.Products.FindAsync(id);
		if (product == null) return NotFound();
		return product;
	}

	
	[HttpPost]
	public async Task<ActionResult<Product>> CreateProduct(Product product)
	{
		if (product.Id != null) Conflict();
		
		_context.Products.Add(product);
		try
		{
			await _context.SaveChangesAsync();
		}
		catch (Exception e)
		{
			return Problem();
		}

		return CreatedAtAction(nameof(GetProduct), new { id = product.Id }, product);
	}

	[HttpPut("{id:int}")]
	public async Task<IActionResult> UpdateProduct(int id, Product product)
	{
		if (id != product.Id)
		{
			return BadRequest("id and product id mismatch");
		}

		_context.Entry(product).State = EntityState.Modified;

		try
		{
			await _context.SaveChangesAsync();
		}
		catch (DbUpdateException e)
		{
			if (!_context.Products.Any(p => p.Id == id))
			{
				return NotFound();
			}

			throw;
		}

		return NoContent();
	}

	[HttpDelete("{id:int}")]
	public async Task<IActionResult> DeleteProduct(int id)
	{
		var product = await _context.Products.FindAsync(id);
		if (product == null) return NotFound();

		_context.Products.Remove(product);
		try
		{
			await _context.SaveChangesAsync();
		}
		catch (Exception e)
		{
			if (await _context.Products.AnyAsync(p => p.Id == id)) 
				return Problem("Operation could not be completed.");
			throw;
		}

		return NoContent();
	}
}