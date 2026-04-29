using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;

namespace BookShelfAPI.IntegrationTests;

public class BooksApiTests : BookShelfApiTestBase
{
    [Fact]
    public async Task Crud_lifecycle_creates_reads_and_deletes_book()
    {
        var createResponse = await Client.PostAsJsonAsync("/api/books", new
        {
            title = "Clean Code",
            author = "Robert C. Martin",
            isbn = "9780132350884"
        });

        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);
        var location = createResponse.Headers.Location;
        Assert.NotNull(location);

        var getResponse = await Client.GetAsync(location);
        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);

        var book = await getResponse.Content.ReadFromJsonAsync<JsonElement>();
        Assert.Equal("Clean Code", book.GetProperty("title").GetString());
        Assert.Equal("Robert C. Martin", book.GetProperty("author").GetString());
        Assert.Equal("9780132350884", book.GetProperty("isbn").GetString());

        var deleteResponse = await Client.DeleteAsync(location);
        Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);

        var afterDeleteResponse = await Client.GetAsync(location);
        Assert.Equal(HttpStatusCode.NotFound, afterDeleteResponse.StatusCode);
    }

    [Fact]
    public async Task Post_with_duplicate_isbn_returns_409_with_problem_details()
    {
        var firstResponse = await Client.PostAsJsonAsync("/api/books", new
        {
            title = "First",
            author = "Author A",
            isbn = "9780132350884"
        });
        firstResponse.EnsureSuccessStatusCode();

        var duplicateResponse = await Client.PostAsJsonAsync("/api/books", new
        {
            title = "Second",
            author = "Author B",
            isbn = "9780132350884"
        });

        Assert.Equal(HttpStatusCode.Conflict, duplicateResponse.StatusCode);

        var problem = await duplicateResponse.Content.ReadFromJsonAsync<ProblemDetails>();
        Assert.NotNull(problem);
        Assert.Equal("Conflict", problem.Title);
        Assert.Equal(409, problem.Status);
        Assert.Contains("ISBN", problem.Detail);
    }

    [Fact]
    public async Task GetStatistics_returns_correct_aggregations_for_seeded_data()
    {
        var ids = new List<Guid>();
        var seeds = new[]
        {
            new { title = "Clean Code", author = "R. Martin", isbn = "9780132350884" },
            new { title = "Refactoring", author = "M. Fowler", isbn = "9780201485677" },
            new { title = "DDD", author = "E. Evans", isbn = "9780321125217" }
        };

        foreach (var seed in seeds)
        {
            var response = await Client.PostAsJsonAsync("/api/books", seed);
            response.EnsureSuccessStatusCode();
            var id = Guid.Parse(response.Headers.Location!.Segments.Last());
            ids.Add(id);
        }

        await PatchAsJson($"/api/books/{ids[0]}/status", new { status = 2 });
        await PatchAsJson($"/api/books/{ids[0]}/rating", new { rating = 5 });
        await PatchAsJson($"/api/books/{ids[1]}/status", new { status = 2 });
        await PatchAsJson($"/api/books/{ids[1]}/rating", new { rating = 3 });

        var stats = await Client.GetFromJsonAsync<JsonElement>("/api/books/statistics");

        Assert.Equal(3, stats.GetProperty("totalCount").GetInt32());
        Assert.Equal(1, stats.GetProperty("unreadCount").GetInt32());
        Assert.Equal(0, stats.GetProperty("readingCount").GetInt32());
        Assert.Equal(2, stats.GetProperty("finishedCount").GetInt32());
        Assert.Equal(4.0, stats.GetProperty("averageRating").GetDouble());
        Assert.Equal(2, stats.GetProperty("finishedThisYear").GetInt32());

        var topAuthors = stats.GetProperty("topAuthors");
        Assert.Equal(3, topAuthors.GetArrayLength());
        Assert.Equal("E. Evans", topAuthors[0].GetProperty("author").GetString());
        Assert.Equal(1, topAuthors[0].GetProperty("count").GetInt32());
    }

    private async Task PatchAsJson<T>(string url, T body)
    {
        var request = new HttpRequestMessage(HttpMethod.Patch, url)
        {
            Content = JsonContent.Create(body)
        };
        var response = await Client.SendAsync(request);
        response.EnsureSuccessStatusCode();
    }
}
