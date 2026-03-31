using AppCore.Dto;
using AppCore.Services;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

[ApiController]
[Route("/api/contacts")]
public class ContactsController(IPersonService service) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAllPersons([FromQuery] int page = 1, [FromQuery] int size = 20)
    {
        var result = await service.FindAllPeoplePaged(page, size);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetPerson(Guid id)
    {
        var dto = await service.GetById(id);

        if (dto is null)
            return NotFound();

        return Ok(dto);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreatePersonDto dto)
    {
        var result = await service.AddPerson(dto);
        var createdDto = await service.GetById(result.Id);

        return CreatedAtAction(nameof(GetPerson), new { id = result.Id }, createdDto);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdatePersonDto dto)
    {
        var result = await service.UpdatePerson(id, dto);

        if (result is null)
            return NotFound();

        var updatedDto = await service.GetById(id);
        return Ok(updatedDto);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var deleted = await service.DeletePerson(id);

        if (!deleted)
            return NotFound();

        return NoContent();
    }

    [HttpPost("{contactId:guid}/notes")]
    [ProducesResponseType(typeof(NoteDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AddNote(
        [FromRoute] Guid contactId,
        [FromBody] CreateNoteDto dto)
    {
        var note = await service.AddNoteToPerson(contactId, dto);

        var noteDto = new NoteDto
        {
            Id = note.Id,
            Content = note.Content,
            CreatedAt = note.CreatedAt
        };

        return CreatedAtAction(
            nameof(GetNotes),
            new { contactId },
            noteDto);
    }

    [HttpGet("{contactId:guid}/notes")]
    [ProducesResponseType(typeof(IEnumerable<NoteDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetNotes([FromRoute] Guid contactId)
    {
        var person = await service.GetPerson(contactId);
        return Ok(person.Notes);
    }

    [HttpDelete("{contactId:guid}/notes/{noteId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteNote(
        [FromRoute] Guid contactId,
        [FromRoute] Guid noteId)
    {
        await service.DeleteNoteFromPerson(contactId, noteId);
        return NoContent();
    }
}