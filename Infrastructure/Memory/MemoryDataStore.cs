using AppCore.Models.Contacts;

namespace Infrastructure.Memory;

public class MemoryDataStore
{
    public Dictionary<Guid, Contact> Contacts { get; } = new();
    public Dictionary<Guid, Person> Persons { get; } = new();
    public Dictionary<Guid, Company> Companies { get; } = new();
    public Dictionary<Guid, Organization> Organizations { get; } = new();
}