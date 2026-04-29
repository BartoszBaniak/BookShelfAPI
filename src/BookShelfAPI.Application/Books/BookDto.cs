using BookShelfAPI.Domain.Entities;
using BookShelfAPI.Domain.Enums;

namespace BookShelfAPI.Application.Books;

public record BookDto(
    Guid Id,
    string Title,
    string Author,
    string? Isbn,
    int? PublicationYear,
    string? Description,
    ReadingStatus Status,
    int? Rating,
    DateOnly? StartedReadingAt,
    DateOnly? FinishedReadingAt)
{
    public static BookDto FromDomain(Book book) =>
        new(
            book.Id,
            book.Title,
            book.Author,
            book.Isbn,
            book.PublicationYear,
            book.Description,
            book.Status,
            book.Rating,
            book.StartedReadingAt,
            book.FinishedReadingAt);
}
