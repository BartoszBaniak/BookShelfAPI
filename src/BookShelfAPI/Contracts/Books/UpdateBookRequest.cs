namespace BookShelfAPI.Contracts.Books;

public record UpdateBookRequest(
    string Title,
    string Author,
    string? Isbn = null,
    int? PublicationYear = null,
    string? Description = null);
