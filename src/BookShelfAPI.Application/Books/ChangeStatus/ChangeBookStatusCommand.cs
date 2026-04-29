using BookShelfAPI.Domain.Enums;

namespace BookShelfAPI.Application.Books.ChangeStatus;

public record ChangeBookStatusCommand(Guid Id, ReadingStatus Status);
