using AppCore.Dto;
using AppCore.Models.Contacts;
using AppCore.Repositories;
using Infrastructure.Memory;

namespace Infrastructure.Memory.Repositories;

public class MemoryContactRepository : MemoryGenericRepository<Contact>, IContactRepository
{
    public MemoryContactRepository(MemoryDataStore store) : base(store.Contacts)
    {
    }

    public Task<PagedResult<Contact>> SearchAsync(ContactSearchDto search)
    {
        var query = _data.Values.AsEnumerable();

        if (!string.IsNullOrWhiteSpace(search.Query))
        {
            var text = search.Query.Trim();

            query = query.Where(c =>
                c.GetDisplayName().Contains(text, StringComparison.OrdinalIgnoreCase) ||
                c.Email.Contains(text, StringComparison.OrdinalIgnoreCase) ||
                c.Phone.Contains(text, StringComparison.OrdinalIgnoreCase));
        }

        if (search.Status.HasValue)
        {
            query = query.Where(c => c.Status == search.Status.Value);
        }

        if (!string.IsNullOrWhiteSpace(search.Tag))
        {
            query = query.Where(c => c.Tags.Any(t =>
                t.Name.Equals(search.Tag, StringComparison.OrdinalIgnoreCase)));
        }

        if (!string.IsNullOrWhiteSpace(search.ContactType))
        {
            query = search.ContactType.Trim().ToLower() switch
            {
                "person" => query.Where(c => c is Person),
                "company" => query.Where(c => c is Company),
                "organization" => query.Where(c => c is Organization),
                _ => query
            };
        }

        var page = search.Page <= 0 ? 1 : search.Page;
        var pageSize = search.PageSize <= 0 ? 20 : search.PageSize;

        var totalCount = query.Count();

        var items = query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        return Task.FromResult(new PagedResult<Contact>(items, totalCount, page, pageSize));
    }

    public Task<IEnumerable<Contact>> FindByTagAsync(string tag)
    {
        var result = _data.Values
            .Where(c => c.Tags.Any(t => t.Name.Equals(tag, StringComparison.OrdinalIgnoreCase)));

        return Task.FromResult(result.AsEnumerable());
    }

    public Task AddNoteAsync(Guid contactId, Note note)
    {
        if (!_data.TryGetValue(contactId, out var contact))
            throw new KeyNotFoundException($"Contact with Id={contactId} not found.");

        contact.Notes.Add(note);
        contact.UpdatedAt = DateTime.UtcNow;
        return Task.CompletedTask;
    }

    public Task<IEnumerable<Note>> GetNotesAsync(Guid contactId)
    {
        if (!_data.TryGetValue(contactId, out var contact))
            throw new KeyNotFoundException($"Contact with Id={contactId} not found.");

        return Task.FromResult(contact.Notes.AsEnumerable());
    }

    public Task AddTagAsync(Guid contactId, string tag)
    {
        if (!_data.TryGetValue(contactId, out var contact))
            throw new KeyNotFoundException($"Contact with Id={contactId} not found.");

        if (!contact.Tags.Any(t => t.Name.Equals(tag, StringComparison.OrdinalIgnoreCase)))
        {
            contact.Tags.Add(new Tag { Name = tag });
            contact.UpdatedAt = DateTime.UtcNow;
        }

        return Task.CompletedTask;
    }

    public Task RemoveTagAsync(Guid contactId, string tag)
    {
        if (!_data.TryGetValue(contactId, out var contact))
            throw new KeyNotFoundException($"Contact with Id={contactId} not found.");

        contact.Tags.RemoveAll(t => t.Name.Equals(tag, StringComparison.OrdinalIgnoreCase));
        contact.UpdatedAt = DateTime.UtcNow;

        return Task.CompletedTask;
    }
}