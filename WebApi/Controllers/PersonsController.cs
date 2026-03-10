using AppCore.Dto;
using AppCore.Repositories;
using Microsoft.AspNetCore.Mvc;
using WebApi.Mappers;

namespace WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PersonsController : ControllerBase
{
    private readonly IPersonRepository _personRepository;

    public PersonsController(IPersonRepository personRepository)
    {
        _personRepository = personRepository;
    }

    [HttpGet]
    public async Task<ActionResult<PagedResult<PersonDto>>> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        var result = await _personRepository.FindPagedAsync(page, pageSize);

        var dto = new PagedResult<PersonDto>(
            result.Items.Select(x => x.ToDto()).ToList(),
            result.TotalCount,
            result.Page,
            result.PageSize);

        return Ok(dto);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<PersonDto>> GetById(Guid id)
    {
        var person = await _personRepository.FindByIdAsync(id);
        if (person is null)
            return NotFound();

        return Ok(person.ToDto());
    }

    [HttpPost]
    public async Task<ActionResult<PersonDto>> Create(CreatePersonDto dto)
    {
        var entity = dto.ToEntity();
        var created = await _personRepository.AddAsync(entity);

        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created.ToDto());
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<PersonDto>> Update(Guid id, UpdatePersonDto dto)
    {
        var person = await _personRepository.FindByIdAsync(id);
        if (person is null)
            return NotFound();

        person.ApplyUpdate(dto);
        var updated = await _personRepository.UpdateAsync(person);

        return Ok(updated.ToDto());
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var person = await _personRepository.FindByIdAsync(id);
        if (person is null)
            return NotFound();

        await _personRepository.RemoveByIdAsync(id);
        return NoContent();
    }
}