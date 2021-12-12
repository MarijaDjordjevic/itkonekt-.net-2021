using Autobarn.Data;
using Autobarn.Data.Entities;
using Autobarn.Website.GraphQL.GraphTypes;
using GraphQL;
using GraphQL.Types;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Autobarn.Website.GraphQL.Queries
{
    public class VehicleQuery : ObjectGraphType
    {
        private readonly IAutobarnDatabase db;

        public VehicleQuery(IAutobarnDatabase db)
        {
            this.db = db;
            Field<ListGraphType<VehicleGraphType>>("Vehicles", "Query to retrive all vehicles", resolve: GetAllVehicles);

            Field<VehicleGraphType>("Vehicle", "Query to retrive all vehicles", resolve: GetVehicle);

            Field<ListGraphType<VehicleGraphType>>("VehiclesByColor", "Retrieve...",
                new QueryArguments(MakeNonNullStringArgument("color", "What color...")),
                resolve: GetVehiclesByColor);

            Field<ListGraphType<VehicleGraphType>>("VehiclesByYear", "Retrieve...",
                new QueryArguments(MakeNonNullStringArgument("year", "What year...")),
                resolve: GetVehiclesByYear);
        }

        private IEnumerable<Vehicle> GetAllVehicles(IResolveFieldContext<object> context) => db.ListVehicles();
        private Vehicle GetVehicle(IResolveFieldContext<object> context)
        {
            var registration = context.GetArgument<string>("registration");
            var vehicle = db.FindVehicle(registration);
            return vehicle;
        }

        private IEnumerable<Vehicle> GetVehiclesByColor(IResolveFieldContext<object> context)
        {
            var color = context.GetArgument<string>("color");
            return db.ListVehicles().Where(x => x.Color.Contains(color, StringComparison.InvariantCultureIgnoreCase));
        }

        private IEnumerable<Vehicle> GetVehiclesByYear(IResolveFieldContext<object> context)
        {
            //For example:  vehiclesByYear(year: "> 2000")
            var yearQuery = context.GetArgument<string>("year");

            var operators = yearQuery.Split(" ")[0];
            var year = int.Parse(yearQuery.Split(" ")[1]);

            return operators switch
            {
                "=" => db.ListVehicles().Where(x => x.Year == year),
                "<=" => db.ListVehicles().Where(x => x.Year <= year),
                ">=" => db.ListVehicles().Where(x => x.Year >= year),
                "<" => db.ListVehicles().Where(x => x.Year < year),
                ">" => db.ListVehicles().Where(x => x.Year > year),
                "!=" => db.ListVehicles().Where(x => x.Year != year),
                _ => throw new Exception("Operator not valid. Use <=, >=, <, >, =, !="),
            };
        }

        private QueryArgument MakeNonNullStringArgument(string name, string description)
        {
            return new QueryArgument<NonNullGraphType<StringGraphType>>
            {
                Name = name,
                Description = description
            };
        }

    }
}
