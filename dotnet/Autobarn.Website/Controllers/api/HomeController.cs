using Autobarn.Data;
using Autobarn.Data.Entities;
using Autobarn.Website.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Autobarn.Website.Controllers.api {
	[Route("api")]
	[ApiController]
	public class HomeController : ControllerBase {
		public IActionResult Get() {
			var result = new {
				_links = new {
					href = "/api/vehicles",
					models = new {
						href = "/api/models"
					}
				},
				_actions = new {
					create = new {
						href = "/api/vehicles",
						type = "application/json",
						method = "POST",
						name = "Add a new vehicle to Autobarn"
					}
				},
				message = "Hello"
			};
			return Ok(result);
		}
	}
}
