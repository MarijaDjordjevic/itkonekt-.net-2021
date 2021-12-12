using Autobarn.Data.Entities;
using GraphQL.Types;

namespace Autobarn.Website.GraphQL.GraphTypes
{
    public sealed class VehicleModelGraphType : ObjectGraphType<Model>
    {
        public VehicleModelGraphType()
        {
            Name = "model";
            Field(v => v.Name);
            Field(v => v.Code);
            //TODO: add manufacturer

            Field(c => c.Manufacturer, type: typeof(ManufacturerGraphType))
                .Description("The  manufacturer");
        }
    }
}
