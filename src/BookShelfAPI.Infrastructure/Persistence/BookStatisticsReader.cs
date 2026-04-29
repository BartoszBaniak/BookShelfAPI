using BookShelfAPI.Domain.Enums;
using BookShelfAPI.Domain.Queries;
using Microsoft.EntityFrameworkCore;

namespace BookShelfAPI.Infrastructure.Persistence;

public class BookStatisticsReader(BookDbContext db) : IBookStatisticsReader
{
    public async Task<BookStatistics> GetStatisticsAsync(CancellationToken cancellationToken = default)
    {
        var byStatus = await db.Books
            .GroupBy(b => b.Status)
            .Select(g => new { Status = g.Key, Count = g.Count() })
            .ToListAsync(cancellationToken);

        var unreadCount = byStatus.FirstOrDefault(x => x.Status == ReadingStatus.Unread)?.Count ?? 0;
        var readingCount = byStatus.FirstOrDefault(x => x.Status == ReadingStatus.Reading)?.Count ?? 0;
        var finishedCount = byStatus.FirstOrDefault(x => x.Status == ReadingStatus.Finished)?.Count ?? 0;
        var totalCount = unreadCount + readingCount + finishedCount;

        var averageRating = await db.Books
            .Where(b => b.Status == ReadingStatus.Finished && b.Rating != null)
            .AverageAsync(b => (double?)b.Rating, cancellationToken);

        var currentYear = DateTime.UtcNow.Year;
        var startOfYear = new DateOnly(currentYear, 1, 1);
        var startOfNextYear = startOfYear.AddYears(1);

        var finishedThisYear = await db.Books
            .Where(b => b.Status == ReadingStatus.Finished
                        && b.FinishedReadingAt >= startOfYear
                        && b.FinishedReadingAt < startOfNextYear)
            .CountAsync(cancellationToken);

        var topAuthorsRaw = await db.Books
            .GroupBy(b => b.Author)
            .Select(g => new { Author = g.Key, Count = g.Count() })
            .OrderByDescending(a => a.Count)
            .ThenBy(a => a.Author)
            .Take(3)
            .ToListAsync(cancellationToken);

        var topAuthors = topAuthorsRaw.Select(a => new AuthorCount(a.Author, a.Count)).ToList();

        return new BookStatistics(
            totalCount,
            unreadCount,
            readingCount,
            finishedCount,
            averageRating,
            finishedThisYear,
            topAuthors);
    }
}
