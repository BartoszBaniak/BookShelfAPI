using BookShelfAPI.Domain.Enums;
using BookShelfAPI.Domain.Exceptions;
using BookShelfAPI.Domain.Services;

namespace BookShelfAPI.Domain.Entities;

public class Book
{
    public const int TitleMaxLength = 200;
    public const int AuthorMaxLength = 150;
    public const int DescriptionMaxLength = 2000;
    public const int MinRating = 1;
    public const int MaxRating = 5;

    public Guid Id { get; private set; }
    public string Title { get; private set; } = null!;
    public string Author { get; private set; } = null!;
    public string? Isbn { get; private set; }
    public int? PublicationYear { get; private set; }
    public string? Description { get; private set; }
    public ReadingStatus Status { get; private set; }
    public int? Rating { get; private set; }
    public DateOnly? StartedReadingAt { get; private set; }
    public DateOnly? FinishedReadingAt { get; private set; }

    private Book() { }

    public static async Task<Book> CreateAsync(
        string title,
        string author,
        IBookIsbnUniquenessChecker isbnUniquenessChecker,
        string? isbn = null,
        int? publicationYear = null,
        string? description = null,
        ReadingStatus status = ReadingStatus.Unread,
        int? rating = null,
        DateOnly? startedReadingAt = null,
        DateOnly? finishedReadingAt = null,
        CancellationToken cancellationToken = default)
    {
        var book = new Book(title, author, isbn, publicationYear, description, status, rating, startedReadingAt, finishedReadingAt);

        if (book.Isbn is not null)
            await EnsureIsbnIsUniqueAsync(book.Isbn, isbnUniquenessChecker, cancellationToken);

        return book;
    }

    private Book(
        string title,
        string author,
        string? isbn = null,
        int? publicationYear = null,
        string? description = null,
        ReadingStatus status = ReadingStatus.Unread,
        int? rating = null,
        DateOnly? startedReadingAt = null,
        DateOnly? finishedReadingAt = null)
    {
        Id = Guid.NewGuid();
        SetTitle(title);
        SetAuthor(author);
        SetIsbn(isbn);
        SetPublicationYear(publicationYear);
        SetDescription(description);
        SetStartedReadingAt(startedReadingAt);
        SetFinishedReadingAt(finishedReadingAt);
        SetStatus(status);
        SetRating(rating);
    }

    public void SetTitle(string title)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new DomainException("Title is required.");
        if (title.Length > TitleMaxLength)
            throw new DomainException($"Title must not exceed {TitleMaxLength} characters.");

        Title = title.Trim();
    }

    public void SetAuthor(string author)
    {
        if (string.IsNullOrWhiteSpace(author))
            throw new DomainException("Author is required.");
        if (author.Length > AuthorMaxLength)
            throw new DomainException($"Author must not exceed {AuthorMaxLength} characters.");

        Author = author.Trim();
    }

    private void SetIsbn(string? isbn)
    {
        Isbn = NormalizeAndValidateIsbn(isbn);
    }

    public async Task ChangeIsbnAsync(
        string? isbn,
        IBookIsbnUniquenessChecker isbnUniquenessChecker,
        CancellationToken cancellationToken = default)
    {
        var normalized = NormalizeAndValidateIsbn(isbn);

        if (normalized is not null && normalized != Isbn)
            await EnsureIsbnIsUniqueAsync(normalized, isbnUniquenessChecker, cancellationToken);

        Isbn = normalized;
    }

    public async Task UpdateCatalogAsync(
        string title,
        string author,
        string? isbn,
        int? publicationYear,
        string? description,
        IBookIsbnUniquenessChecker isbnUniquenessChecker,
        CancellationToken cancellationToken = default)
    {
        SetTitle(title);
        SetAuthor(author);
        await ChangeIsbnAsync(isbn, isbnUniquenessChecker, cancellationToken);
        SetPublicationYear(publicationYear);
        SetDescription(description);
    }

    public void SetPublicationYear(int? year)
    {
        if (year is null)
        {
            PublicationYear = null;
            return;
        }

        if (year > DateTime.UtcNow.Year)
            throw new DomainException("Publication year cannot be in the future.");

        PublicationYear = year;
    }

    public void SetDescription(string? description)
    {
        if (string.IsNullOrWhiteSpace(description))
        {
            Description = null;
            return;
        }

        if (description.Length > DescriptionMaxLength)
            throw new DomainException($"Description must not exceed {DescriptionMaxLength} characters.");

        Description = description;
    }

    public void SetRating(int? rating)
    {
        if (rating is null)
        {
            Rating = null;
            return;
        }

        if (Status != ReadingStatus.Finished)
            throw new DomainException("Rating can only be set for books with status Finished.");

        if (rating < MinRating || rating > MaxRating)
            throw new DomainException($"Rating must be between {MinRating} and {MaxRating}.");

        Rating = rating;
    }

    public void SetStatus(ReadingStatus status)
    {
        if (Status == ReadingStatus.Finished && status == ReadingStatus.Unread)
            throw new DomainException("Cannot revert status from Finished to Unread.");

        if (status == ReadingStatus.Reading && StartedReadingAt is null)
            SetStartedReadingAt(DateOnly.FromDateTime(DateTime.UtcNow));

        if (status == ReadingStatus.Finished && FinishedReadingAt is null)
            SetFinishedReadingAt(DateOnly.FromDateTime(DateTime.UtcNow));

        Status = status;
    }

    public void SetStartedReadingAt(DateOnly? date)
    {
        if (date is not null && FinishedReadingAt is not null && FinishedReadingAt < date)
            throw new DomainException("Started reading date cannot be later than finished reading date.");

        StartedReadingAt = date;
    }

    public void SetFinishedReadingAt(DateOnly? date)
    {
        if (date is not null && StartedReadingAt is not null && date < StartedReadingAt)
            throw new DomainException("Finished reading date cannot be earlier than started reading date.");

        FinishedReadingAt = date;
    }

    private static string? NormalizeAndValidateIsbn(string? isbn)
    {
        if (string.IsNullOrWhiteSpace(isbn))
            return null;

        var normalized = isbn.Replace("-", "").Replace(" ", "");
        if (!IsValidIsbn13(normalized))
            throw new DomainException("ISBN must be a valid ISBN-13.");

        return normalized;
    }

    private static async Task EnsureIsbnIsUniqueAsync(
        string isbn,
        IBookIsbnUniquenessChecker checker,
        CancellationToken cancellationToken)
    {
        var isUnique = await checker.IsUniqueAsync(isbn, cancellationToken);
        if (!isUnique)
            throw new ConflictException("ISBN must be unique across the library.");
    }

    private static bool IsValidIsbn13(string digits)
    {
        if (digits.Length != 13 || !digits.All(char.IsDigit))
            return false;

        var sum = 0;
        for (var i = 0; i < 13; i++)
        {
            var digit = digits[i] - '0';
            sum += i % 2 == 0 ? digit : digit * 3;
        }

        return sum % 10 == 0;
    }
}
