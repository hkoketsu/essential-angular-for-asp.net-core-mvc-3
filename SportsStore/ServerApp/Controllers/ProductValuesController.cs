using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ServerApp.Models;
using Microsoft.EntityFrameworkCore;

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
	}
}