using BookShelfAPI.Application.Abstractions;
using BookShelfAPI.Application.Books.Create;
using BookShelfAPI.Contracts.Books;
using Microsoft.AspNetCore.Mvc;

namespace BookShelfAPI.Controllers;

[ApiController]
[Route("books")]
public class BooksController(ICommandHandler<CreateBookCommand, Guid> createBookHandler) : ControllerBase
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

        return Created($"/books/{id}", new CreateBookResponse(id));
    }
}
