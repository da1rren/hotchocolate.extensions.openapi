using HotChocolate.Extensions.OpenApi.DependencyInjection;
using Polly;

var builder = WebApplication.CreateBuilder(args);
var swaggerUri = new Uri("https://localhost:7038/swagger/v1/swagger.json");
const string swaggerSchemaName = "weather";

builder.Services.AddHttpClient(swaggerSchemaName, config =>
{
    config.BaseAddress = new Uri(swaggerUri.GetLeftPart(UriPartial.Authority | UriPartial.Scheme));
})
.AddTransientHttpErrorPolicy(builder => builder.WaitAndRetryForeverAsync(
    _ => TimeSpan.FromSeconds(1)));

builder.Services
    .AddGraphQLServer()
    .AddOpenApiSchema(swaggerSchemaName, swaggerUri);
    // .AddQueryType<Query>();

var app = builder.Build();

app.UseHttpLogging();

app.MapGraphQL();

app.Run();

public class Query
{
    public Book GetBook() =>
        new Book
        {
            Title = "C# in depth.",
            Tags = new []{ "programming" },
            Author = new Author
            {
                Name = "Jon Skeet"
            }
        };
}

public class Book
{
    public string? Title { get; set; }

    public Author? Author { get; set; }

    public IEnumerable<string> Tags { get; set; }
}

public class Author
{
    public string? Name { get; set; }
}