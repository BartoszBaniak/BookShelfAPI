using BookShelfAPI.Application.Abstractions;
using BookShelfAPI.Application.Books;
using BookShelfAPI.Application.Books.Create;
using BookShelfAPI.Application.Books.GetById;
using BookShelfAPI.Contracts.Books;
using Microsoft.AspNetCore.Mvc;

namespace BookShelfAPI.Controllers;

[ApiController]
[Route("books")]
public class BooksController(
    ICommandHandler<CreateBookCommand, Guid> createBookHandler,
    IQueryHandler<GetBookByIdQuery, BookDto> getBookByIdHandler) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] CreateBookRequest request,
        CancellationToken cancellationToken)
    {
        var command = new CreateBookCommand(
            request.Title,
            request.Author,
            request.Isbn,
            request.PublicationYear,
            request.Description,
            request.Status,
            request.Rating,
            request.StartedReadingAt,
            request.FinishedReadingAt);

        var id = await createBookHandler.HandleAsync(command, cancellationToken);

        return CreatedAtAction(nameof(GetById), new { id }, new CreateBookResponse(id));
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<BookDto>> GetById(
        [FromRoute] Guid id,
        CancellationToken cancellationToken)
    {
        var dto = await getBookByIdHandler.HandleAsync(new GetBookByIdQuery(id), cancellationToken);
        return Ok(dto);
    }
}
