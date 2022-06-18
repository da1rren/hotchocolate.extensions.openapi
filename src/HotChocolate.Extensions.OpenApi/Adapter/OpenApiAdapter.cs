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

            var r = responseTypes.First();
            var rr = _openApiDocument.ResolveReference(r.Reference);
            var rrr = r.Items.Reference;
            var rrrr = _openApiDocument.ResolveReference(rrr);
            
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

    private IEnumerable<OpenApiResponse> ParseResponseTypes(IEnumerable<OpenApiSchema> schemas)
    {
        return Enumerable.Empty<OpenApiResponse>();
    }

    // private string ParseOpenApiSchemaType(OpenApiSchema rootSchema)
    // {
    //     var types = new List<string>();
    //     var stack = new Stack<OpenApiSchema>();
    //     stack.Push(rootSchema);
    //
    //     while (stack.TryPop(out var schema))
    //     {
    //          types.Add(schema.Type);
    //          stack.Push(schema.GetEffective());
    //     }
    // }
}
