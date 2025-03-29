using Microsoft.Extensions.Options;
using Movies.Api.Configuration;
using Movies.Api.Models;
using System.Text.Json;

namespace Movies.Api.Clients;

public class OmdbApiClient(HttpClient httpClient, IOptions<OmdbApiSettings> options)
{
    private readonly OmdbApiSettings _settings = options.Value;

    public async Task<Movie> GetMovieByImdbIdAsync(string imdbId, CancellationToken ct)
    {
        // Build query with API key, IMDb id, and full plot.
        var requestUri = $"?apikey={_settings.ApiKey}&i={imdbId}&plot=full";

        var response = await httpClient.GetAsync(requestUri, ct);
        if (!response.IsSuccessStatusCode)
        {
            return null!;
        }

        var content = await response.Content.ReadAsStringAsync(ct);

        // Deserialize JSON response into Movie
        var movie = JsonSerializer.Deserialize<Movie>(content, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        // OMDb returns Response:"False" when not found.
        if (movie == null || 
            movie.Response.Equals("False",
            StringComparison.InvariantCultureIgnoreCase))
        {
            return null!;
        }

        // Since OMDb doesn't provide an internal ID, assign one
        movie = movie with { Id = Guid.NewGuid() };

        return movie;
    }
}