using MediatR;
using Microsoft.Extensions.Caching.Hybrid;
using Movies.Api.Models;
using Movies.Api.Services;

namespace Movies.Api;

//public sealed record GetMovieRequest(string ImdbId) : IRequest<Movie?>;

//internal sealed class GetMovieRequestHandler(
//    IMovieService movieService,
//    HybridCache hybridCache) : IRequestHandler<GetMovieRequest, Movie?>
//{
//    public async Task<Movie?> Handle(
//        GetMovieRequest request, 
//        CancellationToken cancellationToken)
//    {
//        var movie = await hybridCache.GetOrCreateAsync($"movies:{request.ImdbId}", async ct =>
//        {
//            return await movieService.GetMovieByImdbIdAsync(request.ImdbId, ct);
//        }, cancellationToken: cancellationToken);

//        return movie;
//    }
//}

//internal sealed class LoggingPipelineBehavior<TRequest, TResponse>(
//    ILogger<LoggingPipelineBehavior<TRequest, TResponse>> logger) : IPipelineBehavior<TRequest, TResponse>
//    where TRequest : notnull
//{
//    public async Task<TResponse> Handle(
//        TRequest request,
//        RequestHandlerDelegate<TResponse> next,
//        CancellationToken cancellationToken)
//    {
//        logger.LogInformation("Begin pipeline behavior {Request}", request.GetType().Name);

//        var response = await next(cancellationToken);

//        logger.LogInformation("End pipeline behavior {Request}", request.GetType().Name);

//        return response;
//    }
//}
