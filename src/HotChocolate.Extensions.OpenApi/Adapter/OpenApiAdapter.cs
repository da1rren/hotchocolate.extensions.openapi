namespace HotChocolate.Extensions.OpenApi.Adapter;

using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Readers;

public class SwaggerAdapter
{
    private readonly IHttpClientFactory _clientFactory;

    public SwaggerAdapter(IHttpClientFactory clientFactory)
    {
        _clientFactory = clientFactory;
    }
    
    public async Task<OpenApiSchema> ToSchemaSdl(Uri swaggerDocumentUri)
    {
        var client = _clientFactory.CreateClient();
        var response = await client.GetStreamAsync(swaggerDocumentUri);
        
        //todo error handling
        var openApiDocumentResult = await new OpenApiStreamReader()
            .ReadAsync(response);

        return ProcessSwaggerDocument(openApiDocumentResult.OpenApiDocument);
    }

    private OpenApiSchema ProcessSwaggerDocument(OpenApiDocument openApiDocument)
    {
        var types = ProcessSchemas(openApiDocument.Components.Schemas)
            //todo remove after making recursive
            .Where(x => x.Fields.Any())
            .ToArray();
        
        var operations = ProcessPaths(openApiDocument.Paths)
            .ToArray();

        return new OpenApiSchema(types, operations);
    }

    private IEnumerable<OpenApiType> ProcessSchemas(IDictionary<string, Microsoft.OpenApi.Models.OpenApiSchema> schemas)
    {
        return schemas.Select(schema => 
            ProcessSchema(schema.Key, schema.Value));
    }

    private OpenApiType ProcessSchema(string name, Microsoft.OpenApi.Models.OpenApiSchema schema)
    {
        //todo make recursive  
        var schemaProperties = schema.Properties.Select(properties =>
            new OpenApiTypeField(properties.Key, properties.Value.Type));

        return new OpenApiType(name, schemaProperties.ToArray());
    }
    
    private IEnumerable<IOpenApiOperation> ProcessPaths(OpenApiPaths paths)
    {
        return paths.SelectMany(kvp => 
            ProcessPath(kvp.Key, kvp.Value));
    }

    private IEnumerable<IOpenApiOperation> ProcessPath(string name, OpenApiPathItem item)
    {
        var operations = new List<IOpenApiOperation>();
        
        foreach (var operation in item.Operations)
        {
            switch (operation.Key)
            {
                case OperationType.Get:
                    operations.Add(new QueryOperation(name, operation.Key));
                    break;
                
                case OperationType.Delete:
                case OperationType.Put:
                case OperationType.Post:
                    operations.Add(new MutationOperation(name, operation.Key));
                    break;
                
                case OperationType.Options:
                case OperationType.Head:
                case OperationType.Patch:
                case OperationType.Trace:
                default:
                    //todo what to do
                    break;
            }
        }

        return operations;
    }
}
