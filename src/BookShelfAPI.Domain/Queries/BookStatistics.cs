namespace BookShelfAPI.Domain.Queries;

public record BookStatistics(
    int TotalCount,
    int UnreadCount,
    int ReadingCount,
    int FinishedCount,
    double? AverageRating,
    int FinishedThisYear,
    IReadOnlyList<AuthorCount> TopAuthors);
