using System.Linq;
using AppCore.Dto;
using AppCore.Exceptions;
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

    public async Task<IAsyncEnumerable<PersonDto>> FindPeopleFromCompany(Guid companyId)
    {
        var people = await unitOfWork.Persons.FindByEmployerAsync(companyId);

        async IAsyncEnumerable<PersonDto> GetPeople()
        {
            foreach (var person in people)
            {
                yield return person.ToDto();
                await Task.Yield();
            }
        }

        return GetPeople();
    }

    public async Task<Person> AddPerson(CreatePersonDto personDto)
    {
        var entity = personDto.ToEntity();

        if (personDto.EmployerId.HasValue)
        {
            var employer = await unitOfWork.Companies.FindByIdAsync(personDto.EmployerId.Value);
            AssignEmployer(entity, employer);
        }

        SyncOrganizationMembership(entity);

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
            if (entity.Employer?.Id != personDto.EmployerId.Value)
            {
                RemovePersonFromCurrentEmployer(entity);

                var employer = await unitOfWork.Companies.FindByIdAsync(personDto.EmployerId.Value);
                AssignEmployer(entity, employer);
            }
        }
        else if (entity.Employer is not null)
        {
            RemovePersonFromCurrentEmployer(entity);
            entity.Employer = null;
        }

        entity.ApplyUpdate(personDto);
        SyncOrganizationMembership(entity);

        entity = await unitOfWork.Persons.UpdateAsync(entity);
        await unitOfWork.SaveChangesAsync();

        return entity;
    }

    public async Task<PersonDto?> GetById(Guid id)
    {
        var entity = await unitOfWork.Persons.FindByIdAsync(id);
        return entity?.ToDto();
    }

    public async Task<PersonDto> GetPerson(Guid personId)
    {
        var entity = await unitOfWork.Persons.FindByIdAsync(personId);

        if (entity is null)
            throw new ContactNotFoundException($"Person with id={personId} not found!");

        return entity.ToDto();
    }

    public async Task<bool> DeletePerson(Guid id)
    {
        var entity = await unitOfWork.Persons.FindByIdAsync(id);

        if (entity is null)
            return false;

        RemovePersonFromCurrentEmployer(entity);

        await unitOfWork.Persons.RemoveByIdAsync(id);
        await unitOfWork.SaveChangesAsync();

        return true;
    }

    public async Task<Note> AddNoteToPerson(Guid personId, CreateNoteDto noteDto)
    {
        var person = await unitOfWork.Persons.FindByIdAsync(personId);

        if (person is null)
            throw new ContactNotFoundException($"Person with id={personId} not found!");

        if (person.Notes is null)
            person.Notes = new List<Note>();

        var note = new Note
        {
            Id = Guid.NewGuid(),
            Content = noteDto.Content,
            CreatedAt = DateTime.UtcNow
        };

        person.Notes.Add(note);

        await unitOfWork.Persons.UpdateAsync(person);
        await unitOfWork.SaveChangesAsync();

        return note;
    }

    public async Task DeleteNoteFromPerson(Guid personId, Guid noteId)
    {
        var person = await unitOfWork.Persons.FindByIdAsync(personId);

        if (person is null)
            throw new ContactNotFoundException($"Person with id={personId} not found!");

        var note = person.Notes.FirstOrDefault(n => n.Id == noteId);

        if (note is null)
            throw new ContactNotFoundException($"Note with id={noteId} not found!");

        person.Notes.Remove(note);

        await unitOfWork.Persons.UpdateAsync(person);
        await unitOfWork.SaveChangesAsync();
    }

    public async Task AddTag(Guid id, string tag)
    {
        var person = await unitOfWork.Persons.FindByIdAsync(id);

        if (person is null)
            return;

        if (person.Tags.Any(t => t.Name == tag))
            return;

        person.Tags.Add(new Tag
        {
            Id = Guid.NewGuid(),
            Name = tag
        });

        await unitOfWork.Persons.UpdateAsync(person);
        await unitOfWork.SaveChangesAsync();
    }

    public async Task AddNote(Guid id, string content)
    {
        var person = await unitOfWork.Persons.FindByIdAsync(id);

        if (person is null)
            return;

        person.Notes.Add(new Note
        {
            Id = Guid.NewGuid(),
            Content = content,
            CreatedAt = DateTime.UtcNow
        });

        await unitOfWork.Persons.UpdateAsync(person);
        await unitOfWork.SaveChangesAsync();
    }

    private void RemovePersonFromCurrentEmployer(Person person)
    {
        if (person.Employer is null)
            return;

        person.Employer.Employees.Remove(person);
    }

    private void AssignEmployer(Person person, Company? employer)
    {
        person.Employer = employer;

        if (employer is not null && !employer.Employees.Contains(person))
        {
            employer.Employees.Add(person);
        }
    }

    private void SyncOrganizationMembership(Person person)
    {
        if (person.Organization is not null && !person.Organization.Members.Contains(person))
        {
            person.Organization.Members.Add(person);
        }
    }
}