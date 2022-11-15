namespace JacksonVeroneze.NET.Cache.Util.Builders;

[ExcludeFromCodeCoverage]
public class UserBuilder
{
    private UserBuilder()
    {
    }

    public static User BuildSingle()
    {
        return Factory().Generate();
    }

    private static Faker<User> Factory()
    {
        return new Faker<User>("pt_BR")
            .RuleFor(f => f.Id, s => s.Random.Int())
            .RuleFor(f => f.Name, s => s.Person.FullName);
    }
}