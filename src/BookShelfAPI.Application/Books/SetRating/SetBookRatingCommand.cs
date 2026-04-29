namespace BookShelfAPI.Application.Books.SetRating;

public record SetBookRatingCommand(Guid Id, int Rating);
