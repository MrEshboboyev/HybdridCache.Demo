using Movies.Api.Clients;
using Movies.Api.Models;

namespace Movies.Api.Services;

public class MovieService(OmdbApiClient omdbApiClient) : IMovieService
{
    public async Task<Movie> GetMovieByImdbIdAsync(
        string imdbId, 
        CancellationToken cancellationToken)
    {
        return await omdbApiClient.GetMovieByImdbIdAsync(imdbId, cancellationToken);
    }
}
