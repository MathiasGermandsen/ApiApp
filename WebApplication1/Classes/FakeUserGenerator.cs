using Bogus;


namespace WebApplication1.Classes 
{
    public static class FakeUserGenerator
    {
        public static List<Models.User> GenerateUsers(int count)
        {
            var faker = new Faker<Models.User>()
                .RuleFor(u => u.Name, f => f.Name.FirstName())
                .RuleFor(u => u.Lastname, f => f.Name.LastName())
                .RuleFor(u => u.Email, f => f.Internet.Email())
                .RuleFor(u => u.Age, f => f.Random.Int(18, 100))
                .RuleFor(u => u.PhoneNumber, f => f.Phone.PhoneNumber());
            
            

            return faker.Generate(count);
        }
    }
}