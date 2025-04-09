using Microsoft.Extensions.Caching.Hybrid;
using Movies.Api.Models;
using Movies.Api.Services;

namespace Movies.Api;

public sealed record GetMovieRequest(string ImdbId);

public interface IRequestHandler<TRequest, TResponse>
{
    Task<TResponse> Handle(TRequest request, CancellationToken ct = default);
}

internal sealed class GetMovieRequestHandler(
    IMovieService movieService,
    HybridCache hybridCache) : IRequestHandler<GetMovieRequest, Movie?>
{
    public async Task<Movie?> Handle(
        GetMovieRequest request,
        CancellationToken cancellationToken)
    {
        var movie = await hybridCache.GetOrCreateAsync($"movies:{request.ImdbId}", async ct =>
        {
            return await movieService.GetMovieByImdbIdAsync(request.ImdbId, ct);
        }, cancellationToken: cancellationToken);

        return movie;
    }
}

internal sealed class LoggingRequestHandler<TRequest, TResponse>(
    IRequestHandler<TRequest, TResponse> innerHandler,
    ILogger<LoggingRequestHandler<TRequest, TResponse>> logger) : IRequestHandler<TRequest, TResponse>
    where TRequest : notnull
{
    public async Task<TResponse> Handle(
        TRequest request,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Begin pipeline behavior {Request}", request.GetType().Name);

        var response = await innerHandler.Handle(request, cancellationToken);

        logger.LogInformation("End pipeline behavior {Request}", request.GetType().Name);

        return response;
    }
}
