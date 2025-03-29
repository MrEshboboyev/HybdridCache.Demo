using Movies.Api.Clients;
using Movies.Api.Configuration;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddOpenApi();

builder.Services.Configure<OmdbApiSettings>(builder.Configuration.GetSection("OmdbApi"));

builder.Services.AddHttpClient<OmdbApiClient>((services, client) =>
{
    var settings = builder.Configuration.GetSection("OmdbApi").Get<OmdbApiSettings>();
    client.BaseAddress = new Uri(settings?.BaseUrl ?? 
        throw new InvalidOperationException("OMDB APi Base Url is not configured!"));
});

var app = builder.Build();

app.MapDefaultEndpoints();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.Run();
