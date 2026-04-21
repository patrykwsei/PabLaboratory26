using AppCore.Dto;
using AppCore.Models.Contacts;
using AppCore.Repositories;
using Infrastructure.EntityFramework.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.EntityFramework.Repositories;

public class EfContactRepository(ContactsDbContext context)
    : EfGenericRepository<Contact>(context.Set<Contact>()), IContactRepository
{
    public async Task<PagedResult<Contact>> SearchAsync(ContactSearchDto search)
    {
        var query = context.Set<Contact>()
            .Include(c => c.Notes)
            .Include(c => c.Tags)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(search.Query))
        {
            var text = search.Query.Trim();

            query = query.Where(c =>
                c.Email.Contains(text) ||
                c.Phone.Contains(text));
        }

        if (search.Status.HasValue)
        {
            query = query.Where(c => c.Status == search.Status.Value);
        }

        if (!string.IsNullOrWhiteSpace(search.Tag))
        {
            query = query.Where(c => c.Tags.Any(t => t.Name == search.Tag));
        }

        if (!string.IsNullOrWhiteSpace(search.ContactType))
        {
            var type = search.ContactType.Trim().ToLower();

            query = type switch
            {
                "person" => query.Where(c => c is Person),
                "company" => query.Where(c => c is Company),
                "organization" => query.Where(c => c is Organization),
                _ => query
            };
        }

        var page = search.Page <= 0 ? 1 : search.Page;
        var pageSize = search.PageSize <= 0 ? 20 : search.PageSize;

        var totalCount = await query.CountAsync();

        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PagedResult<Contact>(items, totalCount, page, pageSize);
    }

    public async Task<IEnumerable<Contact>> FindByTagAsync(string tag)
    {
        return await context.Set<Contact>()
            .Include(c => c.Tags)
            .Where(c => c.Tags.Any(t => t.Name == tag))
            .ToListAsync();
    }

    public async Task AddNoteAsync(Guid contactId, Note note)
    {
        var exists = await context.Set<Contact>().AnyAsync(c => c.Id == contactId);
        if (!exists)
            throw new KeyNotFoundException($"Contact with Id={contactId} not found.");

        note.ContactId = contactId;

        await context.Notes.AddAsync(note);
    }

    public async Task<IEnumerable<Note>> GetNotesAsync(Guid contactId)
    {
        var exists = await context.Set<Contact>().AnyAsync(c => c.Id == contactId);
        if (!exists)
            throw new KeyNotFoundException($"Contact with Id={contactId} not found.");

        return await context.Notes
            .Where(n => n.ContactId == contactId)
            .OrderBy(n => n.CreatedAt)
            .ToListAsync();
    }

    public async Task AddTagAsync(Guid contactId, string tag)
    {
        var contact = await context.Set<Contact>()
            .Include(c => c.Tags)
            .FirstOrDefaultAsync(c => c.Id == contactId);

        if (contact is null)
            throw new KeyNotFoundException($"Contact with Id={contactId} not found.");

        if (!contact.Tags.Any(t => t.Name == tag))
        {
            contact.Tags.Add(new Tag
            {
                Name = tag
            });

            contact.UpdatedAt = DateTime.UtcNow;
        }
    }

    public async Task RemoveTagAsync(Guid contactId, string tag)
    {
        var contact = await context.Set<Contact>()
            .Include(c => c.Tags)
            .FirstOrDefaultAsync(c => c.Id == contactId);

        if (contact is null)
            throw new KeyNotFoundException($"Contact with Id={contactId} not found.");

        var tagToRemove = contact.Tags.FirstOrDefault(t => t.Name == tag);
        if (tagToRemove is not null)
        {
            contact.Tags.Remove(tagToRemove);
            contact.UpdatedAt = DateTime.UtcNow;
        }
    }
}