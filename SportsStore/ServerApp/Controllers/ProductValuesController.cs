using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ServerApp.Models;
using Microsoft.EntityFrameworkCore;
using ServerApp.Models.BindingTargets;

namespace ServerApp.Controllers
{
	[Route("api/products")]
	[ApiController]
	public class ProductValuesController : ControllerBase
	{
		private DataContext context;

		public ProductValuesController(DataContext context)
		{
			this.context = context;
		}

		[HttpGet("{id}")]
		public Product GetProduct(long id)
		{
			//System.Threading.Thread.Sleep(5000);
			Product result = context.Products
					.Include(p => p.Supplier).ThenInclude(s => s.Products)
					.Include(p => p.Ratings)
					.FirstOrDefault(p => p.ProductId == id);
			if (result != null)
			{
				if (result.Supplier != null)
				{
					result.Supplier.Products = result.Supplier.Products.Select(p =>
						new Product
						{
							ProductId = p.ProductId,
							Name = p.Name,
							Category = p.Category,
							Description = p.Description,
							Price = p.Price,
						});
				}

				if (result.Ratings != null)
				{
					foreach (Rating r in result.Ratings)
					{
						r.Product = null;
					}
				}
			}
			return result;
		}

		[HttpGet]
		public IEnumerable<Product> GetProducts(string category, string search, bool related = false)
		{
			IQueryable<Product> query = context.Products;
			if (!string.IsNullOrWhiteSpace(category))
			{
				string catLower = category.ToLower();
				query = query.Where(p => p.Category.ToLower().Contains(catLower));
			}
			if (!string.IsNullOrWhiteSpace(search))
			{
				string searchLower = search.ToLower();
				query = query.Where(p => p.Name.ToLower().Contains(searchLower)
					|| p.Description.ToLower().Contains(searchLower)
				);
			}

			if (related)
			{
				query = query.Include(p => p.Supplier).Include(p => p.Ratings);
				List<Product> data = query.ToList();
				data.ForEach(p =>
				{
					if (p.Supplier != null)
					{
						p.Supplier.Products = null;
					}
					if (p.Ratings != null)
					{
						p.Ratings.ForEach(r => r.Product = null);
					}
				});
				return data;
			}
			else
			{
				return query;
			}
		}

		[HttpPost]
		public IActionResult CreateProduct([FromBody] ProductData pData)
		{
			if (ModelState.IsValid)
			{
				Product p = pData.Product;
				if (p.Supplier != null && p.Supplier.SupplierId != 0)
				{
					context.Attach(p.Supplier);
				}
				context.Add(p);
				context.SaveChanges();
				return Ok(p.ProductId);
			}
			else
			{
				return BadRequest(ModelState);
			}
		}

		[HttpPut("{id}")]
		public IActionResult ReplaceProduct(long id, [FromBody] ProductData pData)
		{
			if (ModelState.IsValid)
			{
				Product p = pData.Product;
				p.ProductId = id;
				if (p.Supplier != null && p.Supplier.SupplierId != 0)
				{
					context.Attach(p.Supplier);
				}
				context.Update(p);
				context.SaveChanges();
				return Ok();
			}
			else
			{
				return BadRequest(ModelState);
			}
		}

		[HttpDelete("{id}")]
		public void DeleteProduct(long id)
		{
			context.Products.Remove(new Product { ProductId = id });
			context.SaveChanges();
		}
	}
}