using AppCore.Dto;
using AppCore.Interfaces;
using AppCore.Mappings;
using AppCore.Models.Contacts;
using AppCore.Services;

namespace Infrastructure.Services;

public class MemoryPersonService(IContactUnitOfWork unitOfWork) : IPersonService
{
    public async Task<PagedResult<PersonDto>> FindAllPeoplePaged(int page, int size)
    {
        var people = await unitOfWork.Persons.FindPagedAsync(page, size);

        var items = people.Items
            .Select(p => p.ToDto())
            .ToList();

        return new PagedResult<PersonDto>(
            items,
            people.TotalCount,
            people.Page,
            people.PageSize
        );
    }

    public async Task<PersonDto?> GetById(Guid id)
    {
        var person = await unitOfWork.Persons.FindByIdAsync(id);
        return person?.ToDto();
    }

    public async Task<Person> AddPerson(CreatePersonDto personDto)
    {
        var entity = personDto.ToEntity();

        if (personDto.EmployerId.HasValue)
        {
            var employer = await unitOfWork.Companies.FindByIdAsync(personDto.EmployerId.Value);
            entity.Employer = employer;
        }

        entity = await unitOfWork.Persons.AddAsync(entity);
        await unitOfWork.SaveChangesAsync();

        return entity;
    }

    public async Task<Person?> UpdatePerson(Guid id, UpdatePersonDto personDto)
    {
        var entity = await unitOfWork.Persons.FindByIdAsync(id);
        if (entity is null)
            return null;

        if (personDto.EmployerId.HasValue)
        {
            var employer = await unitOfWork.Companies.FindByIdAsync(personDto.EmployerId.Value);
            entity.Employer = employer;
        }

        entity.ApplyUpdate(personDto);
        entity = await unitOfWork.Persons.UpdateAsync(entity);
        await unitOfWork.SaveChangesAsync();

        return entity;
    }

    public async Task<bool> DeletePerson(Guid id)
    {
        var entity = await unitOfWork.Persons.FindByIdAsync(id);
        if (entity is null)
            return false;

        await unitOfWork.Persons.RemoveByIdAsync(id);
        await unitOfWork.SaveChangesAsync();
        return true;
    }

    public async Task<IAsyncEnumerable<PersonDto>> FindPeopleFromCompany(Guid companyId)
    {
        var people = await unitOfWork.Persons.FindByEmployerAsync(companyId);

        async IAsyncEnumerable<PersonDto> MapAsync()
        {
            foreach (var person in people)
            {
                yield return person.ToDto();
                await Task.Yield();
            }
        }

        return MapAsync();
    }
}