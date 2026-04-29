using BookShelfAPI.Domain.Enums;

namespace BookShelfAPI.Application.Books.Create;

public record CreateBookCommand(
    string Title,
    string Author,
    string? Isbn = null,
    int? PublicationYear = null,
    string? Description = null,
    ReadingStatus Status = ReadingStatus.Unread,
    int? Rating = null,
    DateOnly? StartedReadingAt = null,
    DateOnly? FinishedReadingAt = null);
