using AppCore.Dto;
using AppCore.Models.Contacts;

namespace AppCore.Mappings;

public static class PersonMappings
{
    public static PersonDto ToDto(this Person person)
    {
        return new PersonDto
        {
            Id = person.Id,
            FirstName = person.FirstName,
            LastName = person.LastName,
            Email = person.Email,
            Phone = person.Phone,
            Position = person.Position,
            BirthDate = person.BirthDate,
            Gender = person.Gender,
            EmployerId = person.Employer?.Id,
            Status = person.Status,
            CreatedAt = person.CreatedAt,
            Address = person.Address is null
                ? null
                : new AddressDto(
                    person.Address.Street,
                    person.Address.City,
                    person.Address.PostalCode,
                    person.Address.Country,
                    person.Address.Type),
            Tags = person.Tags.Select(t => t.Name).ToList(),
            Notes = person.Notes.Select(n => new NoteDto
            {
                Id = n.Id,
                Content = n.Content,
                CreatedAt = n.CreatedAt
            }).ToList()
        };
    }

    public static Person ToEntity(this CreatePersonDto dto)
    {
        return new Person
        {
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            Email = dto.Email,
            Phone = dto.Phone,
            Position = dto.Position,
            BirthDate = dto.BirthDate,
            Gender = dto.Gender,
            Address = dto.Address is null
                ? null
                : new Address
                {
                    Street = dto.Address.Street,
                    City = dto.Address.City,
                    PostalCode = dto.Address.PostalCode,
                    Country = dto.Address.Country,
                    Type = dto.Address.Type
                }
        };
    }

    public static void ApplyUpdate(this Person person, UpdatePersonDto dto)
    {
        if (dto.FirstName is not null) person.FirstName = dto.FirstName;
        if (dto.LastName is not null) person.LastName = dto.LastName;
        if (dto.Email is not null) person.Email = dto.Email;
        if (dto.Phone is not null) person.Phone = dto.Phone;
        if (dto.Position is not null) person.Position = dto.Position;
        if (dto.BirthDate is not null) person.BirthDate = dto.BirthDate;
        if (dto.Gender is not null) person.Gender = dto.Gender.Value;
        if (dto.Status is not null) person.Status = dto.Status.Value;

        if (dto.Address is not null)
        {
            person.Address = new Address
            {
                Street = dto.Address.Street,
                City = dto.Address.City,
                PostalCode = dto.Address.PostalCode,
                Country = dto.Address.Country,
                Type = dto.Address.Type
            };
        }

        person.UpdatedAt = DateTime.UtcNow;
    }
}