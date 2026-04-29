namespace BookShelfAPI.Application.Books.Update;

public record UpdateBookCommand(
    Guid Id,
    string Title,
    string Author,
    string? Isbn,
    int? PublicationYear,
    string? Description);
