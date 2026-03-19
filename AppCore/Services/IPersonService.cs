using AppCore.Dto;
using AppCore.Models.Contacts;

namespace AppCore.Services;

public interface IPersonService
{
    Task<PagedResult<PersonDto>> FindAllPeoplePaged(int page, int size);
    Task<PersonDto?> GetById(Guid id);
    Task<Person> AddPerson(CreatePersonDto personDto);
    Task<Person?> UpdatePerson(Guid id, UpdatePersonDto personDto);
    Task<bool> DeletePerson(Guid id);
    Task<IAsyncEnumerable<PersonDto>> FindPeopleFromCompany(Guid companyId);
}