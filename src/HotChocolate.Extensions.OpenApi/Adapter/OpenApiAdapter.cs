namespace HotChocolate.Extensions.OpenApi.Adapter;

using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Readers;

public class SwaggerAdapter
{
    private readonly HttpClient _httpClient;

    private OpenApiDocument _openApiDocument;

    public SwaggerAdapter(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }
    
    public async Task<ParsedOpenApiSchema> ToSchemaSdl(Uri swaggerDocumentUri)
    {
        var response = await _httpClient.GetStreamAsync(swaggerDocumentUri);
        
        //todo error handling
        var openApiDocumentResult = await new OpenApiStreamReader()
            .ReadAsync(response);

        _openApiDocument = openApiDocumentResult.OpenApiDocument;
        
        return ProcessSwaggerDocument(_openApiDocument);
    }

    private ParsedOpenApiSchema ProcessSwaggerDocument(OpenApiDocument openApiDocument)
    {
        var types = ProcessSchemas(openApiDocument.Components.Schemas)
            //todo remove after making recursive
            .Where(x => x.Fields.Any())
            .ToArray();
        
        var operations = ProcessPaths(openApiDocument.Paths)
            .ToArray();

        return new ParsedOpenApiSchema(types, operations);
    }

    private IEnumerable<ParsedOpenApiType> ProcessSchemas(IDictionary<string, OpenApiSchema> schemas)
    {
        return schemas.Select(schema => 
            ProcessSchema(schema.Key, schema.Value));
    }

    private ParsedOpenApiType ProcessSchema(string name, OpenApiSchema schema)
    {
        //todo make recursive  
        var schemaProperties = schema.Properties.Select(properties =>
            new ParsedOpenApiTypeField(properties.Key, properties.Value.Type));

        return new ParsedOpenApiType(name, schemaProperties.ToArray());
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
            var responses = operation.Value.Responses
                .Select(x => x.Value);

            var responseTypes = responses
                .SelectMany(x => x.Content.Values)
                .Select(x => x.Schema);

            var queryTypes = ParseResponseTypes(responseTypes);
            var operationName = name.Trim('/').Replace('/', '_');
            switch (operation.Key)
            {
                case OperationType.Get:
                    operations.Add(new QueryOperation(operationName, operation.Key, queryTypes.Single()));
                    break;
                
                case OperationType.Delete:
                case OperationType.Put:
                case OperationType.Post:
                    operations.Add(new MutationOperation(operationName, operation.Key));
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

    private IEnumerable<string> ParseResponseTypes(IEnumerable<OpenApiSchema> schemas)
    {
        return schemas.Select(ResolveFullType)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);
    }

    private string ResolveFullType(OpenApiSchema schema)
    {
        return schema?.Type == "array" ? $"[{schema.Items.Reference.Id}]" : schema.Reference.Id;
    }
}
