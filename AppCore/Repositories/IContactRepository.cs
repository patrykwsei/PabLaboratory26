using AppCore.Dto;
using AppCore.Models.Contacts;

namespace AppCore.Repositories;

public interface IContactRepository : IGenericRepositoryAsync<Contact>
{
    Task<PagedResult<Contact>> SearchAsync(ContactSearchDto search);
    Task<IEnumerable<Contact>> FindByTagAsync(string tag);
    Task AddNoteAsync(Guid contactId, Note note);
    Task<IEnumerable<Note>> GetNotesAsync(Guid contactId);
    Task AddTagAsync(Guid contactId, string tag);
    Task RemoveTagAsync(Guid contactId, string tag);
}