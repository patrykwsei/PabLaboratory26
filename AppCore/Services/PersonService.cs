using AppCore.Dto;
using AppCore.Exceptions;
using AppCore.Interfaces;
using AppCore.Mappings;
using AppCore.Models.Contacts;
using AppCore.Repositories;

namespace AppCore.Services;

public class PersonService : IPersonService
{
    private readonly IContactUnitOfWork _unitOfWork;
    private readonly IContactRepository _contactRepository;

    public PersonService(IContactUnitOfWork unitOfWork, IContactRepository contactRepository)
    {
        _unitOfWork = unitOfWork;
        _contactRepository = contactRepository;
    }

    public async Task<PagedResult<PersonDto>> FindAllPeoplePaged(int page, int size)
    {
        var people = await _unitOfWork.Persons.FindPagedAsync(page, size);

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
        var person = await _unitOfWork.Persons.FindByIdAsync(id);
        return person?.ToDto();
    }

    public async Task<PersonDto> GetPerson(Guid personId)
    {
        var person = await _unitOfWork.Persons.FindByIdAsync(personId);

        if (person is null)
            throw new ContactNotFoundException($"Person with id={personId} not found!");

        return person.ToDto();
    }

    public async Task<Person> AddPerson(CreatePersonDto personDto)
    {
        var entity = personDto.ToEntity();

        entity = await _unitOfWork.Persons.AddAsync(entity);
        await _unitOfWork.SaveChangesAsync();

        return entity;
    }

    public async Task<Person?> UpdatePerson(Guid id, UpdatePersonDto personDto)
    {
        var entity = await _unitOfWork.Persons.FindByIdAsync(id);

        if (entity is null)
            return null;

        entity.ApplyUpdate(personDto);

        entity = await _unitOfWork.Persons.UpdateAsync(entity);
        await _unitOfWork.SaveChangesAsync();

        return entity;
    }

    public async Task<bool> DeletePerson(Guid id)
    {
        var entity = await _unitOfWork.Persons.FindByIdAsync(id);

        if (entity is null)
            return false;

        await _unitOfWork.Persons.RemoveByIdAsync(id);
        await _unitOfWork.SaveChangesAsync();

        return true;
    }

    public async Task<IAsyncEnumerable<PersonDto>> FindPeopleFromCompany(Guid companyId)
    {
        var people = await _unitOfWork.Persons.FindByEmployerAsync(companyId);

        async IAsyncEnumerable<PersonDto> MapAsync(IEnumerable<Person> persons)
        {
            foreach (var person in persons)
            {
                yield return person.ToDto();
                await Task.Yield();
            }
        }

        return MapAsync(people);
    }

    public async Task<Note> AddNoteToPerson(Guid personId, CreateNoteDto noteDto)
    {
        var person = await _unitOfWork.Persons.FindByIdAsync(personId);

        if (person is null)
            throw new ContactNotFoundException($"Person with id={personId} not found!");

        var note = new Note
        {
            Content = noteDto.Content,
            CreatedAt = DateTime.UtcNow,
            ContactId = personId
        };

        await _contactRepository.AddNoteAsync(personId, note);
        await _unitOfWork.SaveChangesAsync();

        return note;
    }

    public async Task DeleteNoteFromPerson(Guid personId, Guid noteId)
    {
        var person = await _unitOfWork.Persons.FindByIdAsync(personId);

        if (person is null)
            throw new ContactNotFoundException($"Person with id={personId} not found!");

        var notes = await _contactRepository.GetNotesAsync(personId);
        var note = notes.FirstOrDefault(n => n.Id == noteId);

        if (note is null)
            throw new ContactNotFoundException($"Note with id={noteId} not found for person with id={personId}!");

        person.Notes.RemoveAll(n => n.Id == noteId);
        await _unitOfWork.SaveChangesAsync();
    }
}