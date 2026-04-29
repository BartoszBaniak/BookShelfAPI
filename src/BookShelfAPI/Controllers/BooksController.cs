using System.ComponentModel.DataAnnotations;
using BookShelfAPI.Application.Abstractions;
using BookShelfAPI.Application.Books;
using BookShelfAPI.Application.Books.ChangeStatus;
using BookShelfAPI.Application.Books.Create;
using BookShelfAPI.Application.Books.Delete;
using BookShelfAPI.Application.Books.GetAll;
using BookShelfAPI.Application.Books.GetById;
using BookShelfAPI.Application.Books.SetRating;
using BookShelfAPI.Application.Books.Statistics;
using BookShelfAPI.Application.Books.Update;
using BookShelfAPI.Application.Common;
using BookShelfAPI.Contracts.Books;
using BookShelfAPI.Domain.Enums;
using BookShelfAPI.Domain.Queries;
using Microsoft.AspNetCore.Mvc;

namespace BookShelfAPI.Controllers;

[ApiController]
[Route("api/books")]
public class BooksController(
    ICommandHandler<CreateBookCommand, Guid> createBookHandler,
    ICommandHandler<DeleteBookCommand> deleteBookHandler,
    ICommandHandler<UpdateBookCommand> updateBookHandler,
    ICommandHandler<ChangeBookStatusCommand> changeBookStatusHandler,
    ICommandHandler<SetBookRatingCommand> setBookRatingHandler,
    IQueryHandler<GetBookByIdQuery, BookDto> getBookByIdHandler,
    IQueryHandler<GetBooksQuery, PagedResult<BookDto>> getBooksHandler,
    IQueryHandler<GetStatisticsQuery, BookStatistics> getStatisticsHandler) : ControllerBase
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

    [HttpGet("statistics")]
    public async Task<ActionResult<BookStatistics>> GetStatistics(CancellationToken cancellationToken)
    {
        var stats = await getStatisticsHandler.HandleAsync(new GetStatisticsQuery(), cancellationToken);
        return Ok(stats);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(
        [FromRoute] Guid id,
        CancellationToken cancellationToken)
    {
        await deleteBookHandler.HandleAsync(new DeleteBookCommand(id), cancellationToken);
        return NoContent();
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(
        [FromRoute] Guid id,
        [FromBody] UpdateBookRequest request,
        CancellationToken cancellationToken)
    {
        var command = new UpdateBookCommand(
            id,
            request.Title,
            request.Author,
            request.Isbn,
            request.PublicationYear,
            request.Description);

        await updateBookHandler.HandleAsync(command, cancellationToken);
        return NoContent();
    }

    [HttpPatch("{id:guid}/status")]
    public async Task<IActionResult> ChangeStatus(
        [FromRoute] Guid id,
        [FromBody] ChangeBookStatusRequest request,
        CancellationToken cancellationToken)
    {
        await changeBookStatusHandler.HandleAsync(
            new ChangeBookStatusCommand(id, request.Status),
            cancellationToken);
        return NoContent();
    }

    [HttpPatch("{id:guid}/rating")]
    public async Task<IActionResult> SetRating(
        [FromRoute] Guid id,
        [FromBody] SetBookRatingRequest request,
        CancellationToken cancellationToken)
    {
        await setBookRatingHandler.HandleAsync(
            new SetBookRatingCommand(id, request.Rating),
            cancellationToken);
        return NoContent();
    }
}
