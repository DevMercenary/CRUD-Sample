using System.Net;
using System.Net.Http.Json;
using CrudSample.Core.Application;
using FluentAssertions;
using Xunit;

namespace CrudSample.Tests;

public sealed class UsersEndpointTests : IClassFixture<TestWebApplicationFactory>
{
    private readonly HttpClient _client;

    public UsersEndpointTests(TestWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Health_returns_healthy()
    {
        var response = await _client.GetAsync("/health");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadAsStringAsync();
        body.Should().Be("Healthy");
    }

    [Fact]
    public async Task Empty_list_returns_pagination_metadata()
    {
        var response = await _client.GetFromJsonAsync<PagedResult<UserResponse>>("/api/v1/users");
        response.Should().NotBeNull();
        response!.Items.Should().BeEmpty();
        response.Page.Should().Be(1);
        response.PageSize.Should().Be(20);
        response.TotalItems.Should().Be(0);
    }

    [Fact]
    public async Task Create_then_get_round_trip()
    {
        var created = await CreateUser("Ada", "Lovelace", "ada@example.com");

        var fetched = await _client.GetFromJsonAsync<UserResponse>($"/api/v1/users/{created.Id}");
        fetched.Should().BeEquivalentTo(created, opts => opts.Excluding(u => u.CreatedAt));
    }

    [Fact]
    public async Task Get_unknown_id_returns_problem_details()
    {
        var response = await _client.GetAsync("/api/v1/users/999");
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        var body = await response.Content.ReadAsStringAsync();
        body.Should().Contain("\"type\":");
        body.Should().Contain("user-not-found");
    }

    [Fact]
    public async Task Duplicate_email_returns_409_problem_details()
    {
        await CreateUser("Alan", "Turing", "alan@example.com");

        var dup = await _client.PostAsJsonAsync("/api/v1/users", new CreateUserRequest("Alonzo", "Church", "alan@example.com"));
        dup.StatusCode.Should().Be(HttpStatusCode.Conflict);
        var body = await dup.Content.ReadAsStringAsync();
        body.Should().Contain("email-conflict");
    }

    [Fact]
    public async Task Invalid_email_returns_400_validation_problem()
    {
        var response = await _client.PostAsJsonAsync("/api/v1/users",
            new CreateUserRequest("Bad", "User", "not-an-email"));
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Update_modifies_existing_user()
    {
        var user = await CreateUser("Donald", "Knuth", "knuth@example.com");

        var update = await _client.PutAsJsonAsync(
            $"/api/v1/users/{user.Id}",
            new UpdateUserRequest("Donald", "Knuth", "tao@example.com"));
        update.EnsureSuccessStatusCode();

        var fetched = await _client.GetFromJsonAsync<UserResponse>($"/api/v1/users/{user.Id}");
        fetched!.Email.Should().Be("tao@example.com");
    }

    [Fact]
    public async Task Delete_returns_204_then_404_on_repeat()
    {
        var user = await CreateUser("Edsger", "Dijkstra", "ed@example.com");

        var first = await _client.DeleteAsync($"/api/v1/users/{user.Id}");
        first.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var again = await _client.DeleteAsync($"/api/v1/users/{user.Id}");
        again.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Pagination_returns_requested_page_size()
    {
        for (var i = 0; i < 5; i++)
        {
            await CreateUser($"User{i}", "Test", $"user{i}@example.com");
        }

        var page = await _client.GetFromJsonAsync<PagedResult<UserResponse>>("/api/v1/users?page=1&pageSize=2");
        page!.Items.Should().HaveCount(2);
        page.PageSize.Should().Be(2);
        page.TotalItems.Should().BeGreaterOrEqualTo(5);
        page.TotalPages.Should().BeGreaterThan(1);
    }

    [Fact]
    public async Task Minimal_api_count_endpoint_works()
    {
        await CreateUser("Grace", "Hopper", "grace@example.com");

        var response = await _client.GetAsync("/api/v1/users/count");
        response.EnsureSuccessStatusCode();
        var payload = await response.Content.ReadFromJsonAsync<CountResponse>();
        payload!.Count.Should().BeGreaterOrEqualTo(1);
    }

    private async Task<UserResponse> CreateUser(string first, string last, string email)
    {
        var response = await _client.PostAsJsonAsync(
            "/api/v1/users",
            new CreateUserRequest(first, last, email));
        response.EnsureSuccessStatusCode();
        var user = await response.Content.ReadFromJsonAsync<UserResponse>();
        return user!;
    }

    private sealed record CountResponse(int Count);
}
