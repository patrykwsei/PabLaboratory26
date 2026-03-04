using AppCore.Dto;
using AppCore.Models.Contacts;
using AppCore.Repositories;

namespace Infrastructure.Memory.Repositories;

public class MemoryContactRepository : MemoryGenericRepository<Contact>, IContactRepository
{
    public Task<PagedResult<Contact>> SearchAsync(ContactSearchDto search)
    {
        var q = _data.Values.AsEnumerable();

        if (!string.IsNullOrWhiteSpace(search.Query))
        {
            var s = search.Query.Trim();
            q = q.Where(c =>
                (c.Email?.Contains(s, StringComparison.OrdinalIgnoreCase) ?? false) ||
                (c.Phone?.Contains(s, StringComparison.OrdinalIgnoreCase) ?? false) ||
                (c.GetDisplayName()?.Contains(s, StringComparison.OrdinalIgnoreCase) ?? false));
        }

        if (search.Status is not null)
            q = q.Where(c => c.Status == search.Status);

        if (!string.IsNullOrWhiteSpace(search.Tag))
        {
            var tag = search.Tag.Trim();
            q = q.Where(c => c.Tags.Any(t => string.Equals(t.Name, tag, StringComparison.OrdinalIgnoreCase)));
        }

        if (!string.IsNullOrWhiteSpace(search.ContactType))
        {
            var type = search.ContactType.Trim();
            q = q.Where(c => string.Equals(c.GetType().Name, type, StringComparison.OrdinalIgnoreCase));
        }

        var total = q.Count();

        var page = search.Page <= 0 ? 1 : search.Page;
        var pageSize = search.PageSize <= 0 ? 20 : search.PageSize;

        var items = q
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        return Task.FromResult(new PagedResult<Contact>(items, total, page, pageSize));
    }

    public Task<IEnumerable<Contact>> FindByTagAsync(string tag)
    {
        tag = tag?.Trim() ?? "";
        var result = _data.Values
            .Where(c => c.Tags.Any(t => string.Equals(t.Name, tag, StringComparison.OrdinalIgnoreCase)))
            .AsEnumerable();

        return Task.FromResult(result);
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

        tag = tag?.Trim() ?? "";
        if (string.IsNullOrWhiteSpace(tag)) return Task.CompletedTask;

        // żeby nie dublować tagów
        if (!contact.Tags.Any(t => string.Equals(t.Name, tag, StringComparison.OrdinalIgnoreCase)))
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

        tag = tag?.Trim() ?? "";
        contact.Tags.RemoveAll(t => string.Equals(t.Name, tag, StringComparison.OrdinalIgnoreCase));
        contact.UpdatedAt = DateTime.UtcNow;

        return Task.CompletedTask;
    }
}