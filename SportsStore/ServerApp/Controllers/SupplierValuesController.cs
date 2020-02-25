using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using ServerApp.Models;
using ServerApp.Models.BindingTargets;

namespace ServerApp.Controllers
{
	[Route("api/suppliers")]
	[ApiController]
	public class SupplierValuesController : ControllerBase
	{
		private DataContext context;

		public SupplierValuesController(DataContext context)
		{
			this.context = context;
		}

		[HttpGet]
		public IEnumerable<Supplier> GetSuppliers()
		{
			return context.Supplier;
		}

		[HttpPost]
		public IActionResult CreateSupplier([FromBody]SupplierData sData)
		{
			Console.WriteLine("Create supplier - " + sData.Name + " valid" + ModelState.IsValid);
			if (ModelState.IsValid)
			{
				Supplier s = sData.Supplier;
				context.Add(s);
				context.SaveChanges();
				return Ok(s.SupplierId);
			}
			else
			{
				return BadRequest(ModelState);
			}
		}

		[HttpPut("{id}")]
		public IActionResult ReplaceSupplier(long id, [FromBody] SupplierData sdata)
		{
			if (ModelState.IsValid)
			{
				Supplier s = sdata.Supplier;
				s.SupplierId = id;
				context.Update(s);
				context.SaveChanges();
				return Ok();
			}
			else
			{
				return BadRequest(ModelState);
			}
		}

		[HttpDelete("{id}")]
		public void DeleteSupplier(long id)
		{
			context.Remove(new Supplier { SupplierId = id });
			context.SaveChanges();
		}
	}
}