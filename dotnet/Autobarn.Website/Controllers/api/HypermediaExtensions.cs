using System.Collections.Generic;
using System;
using System.Dynamic;
using System.ComponentModel;
using System.Linq;
using Autobarn.Data.Entities;

namespace Autobarn.Website.Controllers.api 
{
    public static class Hal 
    {

        public static dynamic ToHypermediaResource(this Vehicle vehicle,string expand = "")
        {
            var result = vehicle.ToDynamic();
            result._links = new 
			{
				self = new {
					href = $"/api/vehicles/{vehicle.Registration}"
				},
				model = new {
					href = $"/api/models/{vehicle.ModelCode}"
				}
			};
            if(expand == "model")
            {
                result._embedded = new {
                    model = vehicle.VehicleModel
                };
            }
            return result;
        }
        public static dynamic ToHypermediaResource(this Model model,string expand = "")
        {
            var result = model.ToDynamic();
            result._links = new 
			{
				self = new {
					href = $"/api/models/{model.Code}"
				},
                info = new {
					href = model.ManufacturerWebsiteUrl
				}
			};

            result._actions = new {
                create = new {
                    href = $"/api/models/{model.Code}",
                    type = "application/json",
                    method = "POST",
                    name = $"Create a new {model.Manufacturer.Name} {model.Name}"
                }
            };

            //if(expand == "manufacturer")
            //{
            //    result._embedded = new {
            //        manufacturer = model.Manufacturer
            //    };
            //}
            return result;
        }


        public static dynamic ToDynamic(this object obj)
        {
            IDictionary<string, object> expando = new ExpandoObject();

            var properties = TypeDescriptor.GetProperties(obj.GetType());

            foreach(PropertyDescriptor prop in properties)
            {
                if(Ignore(prop)) continue;
                expando.Add(prop.Name, prop.GetValue(obj));
            }

            return expando;
        }

        private static bool Ignore(PropertyDescriptor prop)
        {
            return prop.Attributes.OfType<Newtonsoft.Json.JsonIgnoreAttribute>().Any();
        }

        public static Dictionary<string, object> Paginate(string baseUrl, int index, int count, int total)
        {
            var links = new Dictionary<string, object>();
            links.Add("self", new { href = $"{baseUrl}?index={index}&count={count}"});
            if (index + count < total)
            {
                links.Add("next", new { href = $"{baseUrl}?index={index + count}&count={count}"});
            }
            if(index > 0)
            {
                links.Add("prev", new { href = $"{baseUrl}?index={Math.Max(0, index - count)}&count={count}"});
            }

            links.Add("first", new { href = $"{baseUrl}?index=0&count={count}"});
            links.Add("final", new { href = $"{baseUrl}?index={total - (total % count)}&count={count}"});
            
            return links;
        }
    }
}