using Autobarn.Data;
using Autobarn.Data.Entities;
using Autobarn.Messages;
using Autobarn.Website.Models;
using EasyNetQ;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Autobarn.Website.Controllers.api {
	[Route("api/[controller]")]
	[ApiController]
	public class VehiclesController : ControllerBase {
		private readonly IAutobarnDatabase db;
        private readonly IBus bus;

        public VehiclesController(IAutobarnDatabase db, IBus bus) {
			this.db = db;
			this.bus = bus;
		}


		const int page_size = 10;

		// GET: api/vehicles
		[HttpGet]
		[Produces("application/hal+json")]
		public IActionResult Get(int index = 0, int count = page_size, string expand = "") {
			//return db.ListVehicles().Skip(index).Take(page_size).ToList();
			/*var result = new{
				message = new {
					hello = "ITKonekt"
				}
			};*/
			//var items = db.ListVehicles().Skip(index).Take(page_size);
			var items = db.ListVehicles().Skip(index).Take(page_size).Select(vehicle => vehicle.ToHypermediaResource(expand));
			

			/*dynamic _links = new ExpandoObject();
			_links.self = new {href = "/api/vehicles?index={index}"};
			_links.next = new {href = $"/api/vehicles?index={index + page_size}"};
			if(index > 0)
			{
				_links.previous = new {href = $"/api/vehicles?index={index - page_size}"};
			}*/
			/*
			var result = new {
				_links = new {
					href = "/api/vehicles?index={index}"
				},
				next = new {
					href = $"/api/vehicles?index={index + page_size}"
				},
				previous = new {
					href = $"/api/vehicles?index={index - page_size}"
				},
				items
			}*/

			var total = db.CountVehicles();
			var _links = Hal.Paginate("/api/vehicles", index, count, total);
			var result = new{
				_links,
				index,
				count,
				total,
				items
			};

			return Ok(result);
		}
		// GET: api/vehicles
		[HttpGet("search")]
		[Produces("application/hal+json")]
		public IActionResult GetByCharacter(char character, int index, int count = page_size) {
			var items = db.ListVehicles().Where(x => x.Registration.StartsWith(character)).Skip(index).Take(page_size).ToList();
			var total = db.ListVehicles().Where(x => x.Registration.StartsWith(character)).Count();
			var _links = Hal.Paginate("/api/vehicles", index, count, total);
			var result = new{
				total,
				index,
				count,
				items,
				_links
			};

			return Ok(result);
		}

		// GET api/vehicles/ABC123
		[HttpGet("{id}")]
		[Produces("application/hal+json")]
		public IActionResult Get(string id) {
			var vehicle = db.FindVehicle(id);
			if (vehicle == default) return NotFound();
			var result = vehicle.ToHypermediaResource();

			/*
			var result = vehicle.ToDynamic();

			result._links = new 
			{
				self = new {
					href = $"/api/vehicles/{id}"
				},
				model = new {
					href = $"/api/models/{vehicle.ModelCode}"
				}
			};
			*/
			return Ok(result);
		}

		// POST api/vehicles
		[HttpPost]
		public IActionResult Post([FromBody] VehicleDto dto) {
			var exising = db.FindVehicle(dto.Registration);
			
			if(exising != default)
			{
				return Conflict($"Sorry {dto.Registration} already exists in our database and you are not allowed to sell the same car twice");
			}

			var vehicleModel = db.FindModel(dto.ModelCode);

			if (vehicleModel == default)
			{
				return BadRequest($"Sorry we don't know what kind of car you want {dto.ModelCode}");
			}

			var vehicle = new Vehicle {
				Registration = dto.Registration,
				Color = dto.Color,
				Year = dto.Year,
				VehicleModel = vehicleModel
			};
			db.CreateVehicle(vehicle);
			PublishNewVehicleMessage(vehicle);
			return Created($"/api/vehicles/{vehicle.Registration}", dto);
		}

        private void PublishNewVehicleMessage(Vehicle vehicle)
        {
			NewVehicleMessage message = new NewVehicleMessage
			{
				Registration = vehicle.Registration,
				Color = vehicle.Color,
				Year = vehicle.Year,
				Manufacturer = vehicle?.VehicleModel?.Manufacturer.Name,
				ModelName = vehicle?.VehicleModel?.Name,
				ListedAt = DateTimeOffset.UtcNow
			};
			bus.PubSub.Publish(message);
        }

        // PUT api/vehicles/ABC123
        [HttpPut("{id}")]
		public IActionResult Put(string id, [FromBody] VehicleDto dto) {
			var vehicleModel = db.FindModel(dto.ModelCode);
			var vehicle = new Vehicle {
				Registration = dto.Registration,
				Color = dto.Color,
				Year = dto.Year,
				ModelCode = vehicleModel.Code
			};
			db.UpdateVehicle(vehicle);
			return Ok(dto);
		}

		// DELETE api/vehicles/ABC123
		[HttpDelete("{id}")]
		public IActionResult Delete(string id) {
			var vehicle = db.FindVehicle(id);
			if (vehicle == default) return NotFound();
			db.DeleteVehicle(vehicle);
			return NoContent();
		}
	}
}
