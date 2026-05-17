using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TodoList.Application.DTOs;
using TodoList.Application.Interfaces;

namespace TodoList.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class TodoItemsController(ITodoItemService service) : ControllerBase
{
    private readonly ITodoItemService _service = service;

    private Guid GetUserIdFromToken() =>
        Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? throw new UnauthorizedAccessException("Token inválido."));

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<TodoItemResponseDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<TodoItemResponseDto>>> GetAll()
    {
        var items = await _service.GetAllByUserIdAsync(GetUserIdFromToken());
        return Ok(items);
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(TodoItemResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TodoItemResponseDto>> GetById(int id)
    {
        var item = await _service.GetByIdAsync(id, GetUserIdFromToken());

        if (item is null)
            return NotFound(new { message = $"No se encontró la tarea con Id {id}." });

        return Ok(item);
    }

    [HttpPost]
    [ProducesResponseType(typeof(TodoItemResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TodoItemResponseDto>> Create([FromBody] CreateTodoItemDto createDto)
    {
        try
        {
            // El UserId del DTO se sobreescribe con el del token.
            // El cliente no puede enviar un UserId diferente al suyo.
            createDto.UserId = GetUserIdFromToken();

            var createdItem = await _service.CreateAsync(createDto);

            return CreatedAtAction(nameof(GetById),
                new { id = createdItem.Id },
                createdItem);
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(TodoItemResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<TodoItemResponseDto>> Update(
        int id,
        [FromBody] UpdateTodoItemDto updateDto)
    {
        var updatedItem = await _service.UpdateAsync(id, GetUserIdFromToken(), updateDto);

        if (updatedItem is null)
            return NotFound(new { message = $"No se encontró la tarea con Id {id}." });

        return Ok(updatedItem);
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await _service.DeleteAsync(id, GetUserIdFromToken());

        if (!deleted)
            return NotFound(new { message = $"No se encontró la tarea con Id {id}." });

        return NoContent();
    }
}