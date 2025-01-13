using Bogus;
using System.Collections.Generic;
using WebApplication1.Models;

namespace WebApplication1.Classes
{
    public static class FakeCarGenerator
    {
        public static List<Car> GenerateCars(int count)
        {
            var faker = new Faker<Car>()
                .RuleFor(c => c.Vin, f => f.Vehicle.Vin())
                .RuleFor(c => c.Manufacturer, f => f.Vehicle.Manufacturer())
                .RuleFor(c => c.Model, f => f.Vehicle.Model())
                .RuleFor(c => c.Type, f => f.Vehicle.Type())
                .RuleFor(c => c.Fuel, f => f.Vehicle.Fuel());

            return faker.Generate(count);
        }
    }
}