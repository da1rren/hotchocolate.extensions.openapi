namespace HotChocolate.Extensions.OpenApi.Tests.Adapter;

using Fixtures;
using Microsoft.Extensions.DependencyInjection;
using OpenApi.Adapter;
using Shouldly;

public class OpenApiAdapterTests : IClassFixture<DiFixture>
{
    private readonly DiFixture _fixture;

    public OpenApiAdapterTests(DiFixture fixture)
    {
        _fixture = fixture;
    }
    
    [Fact]
    public async Task Should_Be_Able_To_Covert_PetStore_V2_Yaml_To_Sdl()
    {
        var factory = _fixture.ServiceProvider.GetService<IHttpClientFactory>();
        var client = factory.CreateClient();
        var uri = new Uri("https://raw.githubusercontent.com/OAI/OpenAPI-Specification/main/examples/v2.0/yaml/petstore.yaml");
        var adapter = new SwaggerAdapter(client);
        var adaptedSchema = await adapter.ToSchemaSdl(uri);
        adaptedSchema.Types.ShouldNotBeEmpty();
        adaptedSchema.Operations.ShouldNotBeEmpty();
    }
}