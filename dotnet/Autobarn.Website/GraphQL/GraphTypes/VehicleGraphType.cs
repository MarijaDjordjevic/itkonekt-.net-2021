using Autobarn.Data.Entities;
using GraphQL.Types;

namespace Autobarn.Website.GraphQL.GraphTypes
{
    public sealed class VehicleGraphType : ObjectGraphType<Vehicle>
    {
        public VehicleGraphType()
        {
            Name = "vehicle";
            Field(v => v.Registration);
            Field(v => v.Year);
            Field(v => v.Color);
            //TODO: add model
            Field(c => c.VehicleModel, nullable:false, type: typeof(VehicleModelGraphType))
                .Description("The model of this particular vehicle");
        }
    }
}
