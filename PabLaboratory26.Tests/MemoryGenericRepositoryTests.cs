using AppCore.Models.Contacts;
using Infrastructure.Memory.Repositories;
using Xunit;

namespace PabLaboratory26.Tests;

public class MemoryGenericRepositoryTests
{
    [Fact]
    public async Task AddAsync_ShouldAddEntity()
    {
        // Arrange
        var repo = new MemoryGenericRepository<Person>(new Dictionary<Guid, Person>());
        var person = new Person
        {
            FirstName = "Adam",
            LastName = "Nowak",
            Email = "adam@test.pl",
            Phone = "123456789"
        };

        // Act
        var added = await repo.AddAsync(person);
        var actual = await repo.FindByIdAsync(added.Id);

        // Assert
        Assert.NotNull(actual);
        Assert.Equal(added.Id, actual!.Id);
        Assert.Equal("Adam", actual.FirstName);
        Assert.Equal("Nowak", actual.LastName);
    }

    [Fact]
    public async Task FindByIdAsync_ShouldReturnNull_WhenEntityDoesNotExist()
    {
        // Arrange
        var repo = new MemoryGenericRepository<Person>(new Dictionary<Guid, Person>());
        var id = Guid.NewGuid();

        // Act
        var result = await repo.FindByIdAsync(id);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task FindAllAsync_ShouldReturnAllEntities()
    {
        // Arrange
        var repo = new MemoryGenericRepository<Person>(new Dictionary<Guid, Person>());

        await repo.AddAsync(new Person
        {
            FirstName = "Adam",
            LastName = "Nowak",
            Email = "adam@test.pl",
            Phone = "111"
        });

        await repo.AddAsync(new Person
        {
            FirstName = "Jan",
            LastName = "Kowalski",
            Email = "jan@test.pl",
            Phone = "222"
        });

        // Act
        var result = await repo.FindAllAsync();

        // Assert
        Assert.Equal(2, result.Count());
    }

    [Fact]
    public async Task FindPagedAsync_ShouldReturnCorrectPage()
    {
        // Arrange
        var repo = new MemoryGenericRepository<Person>(new Dictionary<Guid, Person>());

        for (int i = 1; i <= 5; i++)
        {
            await repo.AddAsync(new Person
            {
                FirstName = $"Osoba{i}",
                LastName = "Test",
                Email = $"osoba{i}@test.pl",
                Phone = $"000{i}"
            });
        }

        // Act
        var result = await repo.FindPagedAsync(2, 2);

        // Assert
        Assert.Equal(2, result.Page);
        Assert.Equal(2, result.PageSize);
        Assert.Equal(5, result.TotalCount);
        Assert.Equal(3, result.TotalPages);
        Assert.Equal(2, result.Items.Count);
    }

    [Fact]
    public async Task UpdateAsync_ShouldUpdateEntity_WhenEntityExists()
    {
        // Arrange
        var repo = new MemoryGenericRepository<Person>(new Dictionary<Guid, Person>());
        var person = new Person
        {
            FirstName = "Adam",
            LastName = "Nowak",
            Email = "adam@test.pl",
            Phone = "123"
        };

        var added = await repo.AddAsync(person);

        added.FirstName = "Piotr";
        added.LastName = "Kowalski";

        // Act
        var updated = await repo.UpdateAsync(added);
        var actual = await repo.FindByIdAsync(added.Id);

        // Assert
        Assert.NotNull(actual);
        Assert.Equal("Piotr", actual!.FirstName);
        Assert.Equal("Kowalski", actual.LastName);
        Assert.Equal(updated.Id, actual.Id);
    }

    [Fact]
    public async Task UpdateAsync_ShouldThrow_WhenEntityDoesNotExist()
    {
        // Arrange
        var repo = new MemoryGenericRepository<Person>(new Dictionary<Guid, Person>());
        var person = new Person
        {
            Id = Guid.NewGuid(),
            FirstName = "Ghost",
            LastName = "User",
            Email = "ghost@test.pl",
            Phone = "999"
        };

        // Act + Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() => repo.UpdateAsync(person));
    }

    [Fact]
    public async Task RemoveByIdAsync_ShouldRemoveEntity_WhenEntityExists()
    {
        // Arrange
        var repo = new MemoryGenericRepository<Person>(new Dictionary<Guid, Person>());
        var person = new Person
        {
            FirstName = "Adam",
            LastName = "Nowak",
            Email = "adam@test.pl",
            Phone = "123"
        };

        var added = await repo.AddAsync(person);

        // Act
        await repo.RemoveByIdAsync(added.Id);
        var actual = await repo.FindByIdAsync(added.Id);

        // Assert
        Assert.Null(actual);
    }

    [Fact]
    public async Task RemoveByIdAsync_ShouldThrow_WhenEntityDoesNotExist()
    {
        // Arrange
        var repo = new MemoryGenericRepository<Person>(new Dictionary<Guid, Person>());
        var id = Guid.NewGuid();

        // Act + Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() => repo.RemoveByIdAsync(id));
    }
}