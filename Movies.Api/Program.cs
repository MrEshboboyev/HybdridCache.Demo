using Microsoft.Extensions.Caching.Hybrid;
using Movies.Api.Clients;
using Movies.Api.Configuration;
using Movies.Api.Services;

var builder = WebApplication.CreateBuilder(args);

// (Assume custom extension to add default services)
builder.AddServiceDefaults();

builder.Services.AddOpenApi();

// Bind OMDb settings from configuration (ensure your appsettings.json has an "OmdbApi" section)
builder.Services.Configure<OmdbApiSettings>(builder.Configuration.GetSection("OmdbApi"));

// Configure HttpClient for OmdbApiClient (note we resolve the settings here too)
builder.Services.AddHttpClient<OmdbApiClient>((services, client) =>
{
    var settings = builder.Configuration.GetSection("OmdbApi").Get<OmdbApiSettings>();
    client.BaseAddress = new Uri(settings?.BaseUrl ??
        throw new InvalidOperationException("OMDB API Base Url is not configured!"));
});

// Register MovieService as an abstraction over OmdbApiClient
builder.Services.AddScoped<IMovieService, MovieService>();

// Configure HybridCache with default options
builder.Services.AddHybridCache(options =>
{
    options.DefaultEntryOptions = new HybridCacheEntryOptions
    {
        LocalCacheExpiration = TimeSpan.FromMinutes(1),
        Expiration = TimeSpan.FromMinutes(5)
    };
});

builder.AddRedisDistributedCache("redis");

var app = builder.Build();

app.MapDefaultEndpoints();

// Endpoint to get a movie by IMDb id. HybridCache wraps the call to the movie service.
app.MapGet("movies/{imdbId}", async (
    string imdbId,
    IMovieService movieService,
    HybridCache hybridCache,
    CancellationToken cancellationToken) =>
{
    var movie = await hybridCache.GetOrCreateAsync($"movies:{imdbId}", async ct =>
    {
        return await movieService.GetMovieByImdbIdAsync(imdbId, ct);
    }, cancellationToken: cancellationToken);

    return movie is not null ? Results.Ok(movie) : Results.NotFound();
})
.WithName("GetMovies")
.WithTags("Movies")
.WithOpenApi();

// Endpoint to remove a movie item from the cache
app.MapDelete("movies/{imdbId}/invalidate", async (
    string imdbId,
    HybridCache hybridCache,
    CancellationToken cancellationToken) =>
{
    await hybridCache.RemoveAsync($"movies:{imdbId}", cancellationToken);
    return Results.NoContent();
})
.WithName("InvalidateMovieCache")
.WithTags("Movies")
.WithOpenApi();

app.MapDelete("invalidate-tags", async (
    string tags,
    HybridCache hybridCache,
    CancellationToken cancellationToken) =>
{
    await hybridCache.RemoveByTagAsync(tags.Split(','), cancellationToken);
    return Results.NoContent();
})
.WithName("InvalidateCacheByTag")
.WithTags("Movies")
.WithOpenApi();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.Run();
