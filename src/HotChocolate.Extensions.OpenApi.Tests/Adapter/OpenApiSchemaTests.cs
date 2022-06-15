namespace HotChocolate.Extensions.OpenApi.Tests.Adapter;

using OpenApi.Adapter;
using Snapshooter.Xunit;

public class OpenApiSchemaTests
{
    [Fact]
    public void OpenApiField_Should_Produce_Valid_Graphql_Field()
    {
        var field = new OpenApiTypeField("foo", "Int");
        Snapshot.Match(field.ToString());
    }

    [Fact]
    public void OpenApiType_Should_Produce_Valid_Graphql_Type()
    {
        var field = new OpenApiTypeField("foo", "Int");
        var type = new OpenApiType("bar", field);
        Snapshot.Match(type.ToString());
    }

    [Fact]
    public void OpenApiSchema_Should_Produce_Valid_Graphql_Schema()
    {
        var field = new OpenApiTypeField("foo", "Int");
        var barType = new OpenApiType("bar", field);
        var fooType = new OpenApiType("foobar", field);
        var schema = new OpenApiSchema(new[] {barType, fooType}, Array.Empty<IOpenApiOperation>())
            .ToString();

        Snapshot.Match(schema);
    }
}