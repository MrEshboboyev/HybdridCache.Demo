using System.Text.Json.Serialization;

namespace Movies.Api.Models;

public sealed record Movie(
    Guid Id,
    [property: JsonPropertyName("imdbID")] string ImdbId,
    string Title,
    string Year,
    string Rated,
    string Released,
    string Runtime,
    string Genre,
    string Director,
    string Writer,
    string Actors,
    string Plot,
    string Language,
    string Country,
    string Awards,
    string Poster,
    List<Rating> Ratings,
    string Metascore,
    [property: JsonPropertyName("imdbRating")] string ImdbRating,
    [property: JsonPropertyName("imdbVotes")] string ImdbVotes,
    string Type,
    string DVD,
    string BoxOffice,
    string Production,
    string Website,
    string Response
);
