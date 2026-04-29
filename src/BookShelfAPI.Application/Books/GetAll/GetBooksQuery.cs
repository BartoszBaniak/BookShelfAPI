using BookShelfAPI.Domain.Enums;

namespace BookShelfAPI.Application.Books.GetAll;

public record GetBooksQuery(int Page, int PageSize, ReadingStatus? Status);
