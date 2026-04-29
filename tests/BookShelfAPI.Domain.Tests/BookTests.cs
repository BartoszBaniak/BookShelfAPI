using BookShelfAPI.Domain.Entities;
using BookShelfAPI.Domain.Enums;
using BookShelfAPI.Domain.Exceptions;
using BookShelfAPI.Domain.Services;

namespace BookShelfAPI.Domain.Tests;

public class BookTests
{
    private static readonly IBookIsbnUniquenessChecker AlwaysUnique = new StubIsbnChecker(isUnique: true);

    [Fact]
    public async Task SetTitle_throws_DomainException_when_title_is_empty()
    {
        var book = await Book.CreateAsync("Initial", "Author", AlwaysUnique);

        var ex = Assert.Throws<DomainException>(() => book.SetTitle(""));
        Assert.Equal("Title is required.", ex.Message);
    }

    [Fact]
    public async Task SetRating_throws_DomainException_when_status_is_not_Finished()
    {
        var book = await Book.CreateAsync("Title", "Author", AlwaysUnique);
        book.SetStatus(ReadingStatus.Reading);

        var ex = Assert.Throws<DomainException>(() => book.SetRating(5));
        Assert.Equal("Rating can only be set for books with status Finished.", ex.Message);
    }

    [Fact]
    public async Task SetStatus_throws_DomainException_when_reverting_from_Finished_to_Unread()
    {
        var book = await Book.CreateAsync("Title", "Author", AlwaysUnique);
        book.SetStatus(ReadingStatus.Finished);

        var ex = Assert.Throws<DomainException>(() => book.SetStatus(ReadingStatus.Unread));
        Assert.Equal("Cannot revert status from Finished to Unread.", ex.Message);
    }

    [Fact]
    public async Task SetStatus_to_Reading_auto_sets_StartedReadingAt_when_previously_null()
    {
        var book = await Book.CreateAsync("Title", "Author", AlwaysUnique);
        Assert.Null(book.StartedReadingAt);

        book.SetStatus(ReadingStatus.Reading);

        Assert.Equal(DateOnly.FromDateTime(DateTime.UtcNow), book.StartedReadingAt);
    }

    [Fact]
    public async Task CreateAsync_throws_ConflictException_when_isbn_is_not_unique()
    {
        var checker = new StubIsbnChecker(isUnique: false);

        var ex = await Assert.ThrowsAsync<ConflictException>(() =>
            Book.CreateAsync("Title", "Author", checker, isbn: "9780132350884"));

        Assert.Equal("ISBN must be unique across the library.", ex.Message);
    }

    private sealed class StubIsbnChecker(bool isUnique) : IBookIsbnUniquenessChecker
    {
        public Task<bool> IsUniqueAsync(string isbn, CancellationToken cancellationToken = default)
            => Task.FromResult(isUnique);
    }
}
