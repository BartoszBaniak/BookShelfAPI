using System.ComponentModel.DataAnnotations;
using BookShelfAPI.Application.Abstractions;
using BookShelfAPI.Application.Books;
using BookShelfAPI.Application.Books.Create;
using BookShelfAPI.Application.Books.Delete;
using BookShelfAPI.Application.Books.GetAll;
using BookShelfAPI.Application.Books.GetById;
using BookShelfAPI.Application.Common;
using BookShelfAPI.Contracts.Books;
using BookShelfAPI.Domain.Enums;
using Microsoft.AspNetCore.Mvc;

namespace BookShelfAPI.Controllers;

[ApiController]
[Route("api/books")]
public class BooksController(
    ICommandHandler<CreateBookCommand, Guid> createBookHandler,
    ICommandHandler<DeleteBookCommand> deleteBookHandler,
    IQueryHandler<GetBookByIdQuery, BookDto> getBookByIdHandler,
    IQueryHandler<GetBooksQuery, PagedResult<BookDto>> getBooksHandler) : ControllerBase
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

    [HttpGet]
    public async Task<ActionResult<PagedResult<BookDto>>> Get(
        [FromQuery, Range(1, int.MaxValue)] int page = 1,
        [FromQuery, Range(1, 100)] int pageSize = 20,
        [FromQuery] ReadingStatus? status = null,
        CancellationToken cancellationToken = default)
    {
        var result = await getBooksHandler.HandleAsync(
            new GetBooksQuery(page, pageSize, status),
            cancellationToken);

        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<BookDto>> GetById(
        [FromRoute] Guid id,
        CancellationToken cancellationToken)
    {
        var dto = await getBookByIdHandler.HandleAsync(new GetBookByIdQuery(id), cancellationToken);
        return Ok(dto);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(
        [FromRoute] Guid id,
        CancellationToken cancellationToken)
    {
        await deleteBookHandler.HandleAsync(new DeleteBookCommand(id), cancellationToken);
        return NoContent();
    }
}
