using AppCore.Dto;
using AppCore.Models.Contacts;
using AppCore.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ContactsController : ControllerBase
{
    private readonly IContactRepository _contactRepository;

    public ContactsController(IContactRepository contactRepository)
    {
        _contactRepository = contactRepository;
    }

    [HttpGet("search")]
    public async Task<ActionResult<PagedResult<Contact>>> Search([FromQuery] ContactSearchDto dto)
    {
        var result = await _contactRepository.SearchAsync(dto);
        return Ok(result);
    }

    [HttpPost("{id:guid}/tags")]
    public async Task<IActionResult> AddTag(Guid id, [FromBody] string tag)
    {
        await _contactRepository.AddTagAsync(id, tag);
        return NoContent();
    }

    [HttpDelete("{id:guid}/tags/{tag}")]
    public async Task<IActionResult> RemoveTag(Guid id, string tag)
    {
        await _contactRepository.RemoveTagAsync(id, tag);
        return NoContent();
    }

    [HttpPost("{id:guid}/notes")]
    public async Task<IActionResult> AddNote(Guid id, [FromBody] string content)
    {
        await _contactRepository.AddNoteAsync(id, new Note { Content = content });
        return NoContent();
    }

    [HttpGet("{id:guid}/notes")]
    public async Task<ActionResult<IEnumerable<Note>>> GetNotes(Guid id)
    {
        var notes = await _contactRepository.GetNotesAsync(id);
        return Ok(notes);
    }
}