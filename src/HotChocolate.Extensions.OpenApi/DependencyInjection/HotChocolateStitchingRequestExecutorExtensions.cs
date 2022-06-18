namespace HotChocolate.Extensions.OpenApi.DependencyInjection;

using Adapter;
using Execution.Configuration;
using Language;
using Microsoft.Extensions.DependencyInjection;
using Stitching;

public static class HotChocolateOpenApiStitchingRequestExecutorExtensions
{
    public static IRequestExecutorBuilder AddOpenApiSchema(
        this IRequestExecutorBuilder builder,
        NameString schemaName,
        Uri openApiEndpoint,
        bool ignoreRootTypes = false)
    {
        if (builder == null)
        {
            throw new ArgumentNullException(nameof(builder));
        }

        schemaName.EnsureNotEmpty(nameof(schemaName));

        return builder.AddRemoteSchema(schemaName, async (sp, cancellationToken) =>
        {
            var clientFactory = sp.GetRequiredService<IHttpClientFactory>();
            var httpClient = clientFactory.CreateClient(schemaName);
            var adapter = new SwaggerAdapter(httpClient);
            var adaptedSchema = await adapter.ToSchemaSdl(openApiEndpoint);
            var sdl = adaptedSchema.ToString();
            var remoteSchema = Utf8GraphQLParser.Parse(sdl);
            return new RemoteSchemaDefinition(schemaName, remoteSchema);
        }, ignoreRootTypes);
    }
    
    public static IRequestExecutorBuilder AddOpenApiSchemaFromString(
        this IRequestExecutorBuilder builder,
        NameString schemaName,
        string openApiDocument,
        bool ignoreRootTypes = false)
    {
        if (builder == null)
        {
            throw new ArgumentNullException(nameof(builder));
        }

        schemaName.EnsureNotEmpty(nameof(schemaName));

        return builder.AddRemoteSchemaFromString(schemaName, openApiDocument, ignoreRootTypes);
    }
    
    
}