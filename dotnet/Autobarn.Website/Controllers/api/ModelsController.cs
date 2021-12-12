using Autobarn.Data;
using Autobarn.Data.Entities;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using Autobarn.Website.Models;

namespace Autobarn.Website.Controllers.api {
	[Route("api/[controller]")]
	[ApiController]
	public class ModelsController : ControllerBase {
		private readonly IAutobarnDatabase db;

		public ModelsController(IAutobarnDatabase db) {
			this.db = db;
		}

		[HttpGet]
		[Produces("application/hal+json")]
		public IActionResult Get() {
			var models = db.ListModels().Select(model => model.ToHypermediaResource());
			return Ok(models);
		}

		[HttpGet("{id}")]
		public IActionResult Get(string id) {
			var vehicleModel = db.FindModel(id);
			if (vehicleModel == default) return NotFound();
			return Ok(vehicleModel.ToHypermediaResource());
		}

		// POST api/vehicles
		[HttpPost]
		public IActionResult Post(string id, [FromBody] VehicleDto dto) {
			var vehicleModel = db.FindModel(id);
			
			if(vehicleModel == default)
			{
				return NotFound();	
			}

			var exising = db.FindVehicle(dto.Registration);
			
			if(exising != default)
			{
				return Conflict($"Sorry {dto.Registration} already exists in our database and you are not allowed to sell the same car twice");
			}

			var vehicle = new Vehicle {
				Registration = dto.Registration,
				Color = dto.Color,
				Year = dto.Year,
				VehicleModel = vehicleModel
			};
			db.CreateVehicle(vehicle);
			return Created($"/api/vehicles/{vehicle.Registration}", vehicle.ToHypermediaResource());
		}
	}
}