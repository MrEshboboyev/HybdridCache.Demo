using Movies.Api.Models;

namespace Movies.Api.Services;

public interface IMovieService
{
    Task<Movie> GetMovieByImdbIdAsync(string imdbId, CancellationToken cancellationToken);
}
