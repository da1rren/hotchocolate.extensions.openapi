namespace HotChocolate.Extensions.OpenApi.DependencyInjection;

using Execution.Configuration;
using Microsoft.Extensions.DependencyInjection;

public static class HotChocolateOpenApiStitchingRequestExecutorExtensions
{
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