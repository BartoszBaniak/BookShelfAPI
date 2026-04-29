using BookShelfAPI.Domain.Enums;

namespace BookShelfAPI.Contracts.Books;

public record ChangeBookStatusRequest(ReadingStatus Status);
