namespace Infrastructure.Security;

public interface IDataSeeder
{
    int Order { get; }
    Task SeedAsync();
}