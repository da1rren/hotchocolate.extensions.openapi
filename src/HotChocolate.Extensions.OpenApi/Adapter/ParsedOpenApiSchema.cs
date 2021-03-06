namespace HotChocolate.Extensions.OpenApi.Adapter;

using System.Text;

public record ParsedOpenApiSchema(ParsedOpenApiType[] Types, IOpenApiOperation[] Operations)
{
    public override string ToString()
    {
        var sdl = Types.Select(x => x.ToString())
            .Append(GenerateQueryRoot());
        
        return string.Join(Environment.NewLine, sdl);
    }

    private string GenerateQueryRoot()
    {
        var queryOperations = Operations.OfType<QueryOperation>()
            .Select(x => x.ToString());

        return 
$@"type Query {{
    {string.Join(Environment.NewLine, queryOperations)}
}}";
    }
}

public record ParsedOpenApiType(string Name, params ParsedOpenApiTypeField[] Fields)
{
    public override string ToString()
    {
        return @$"
type {Name} {{ 
    {string.Join(Environment.NewLine, Fields.Select(x => x.ToString()))}
}}";
    }
}

public record ParsedOpenApiTypeField(string Name, string Type)
{
    public override string ToString()
    {
        var resolvedType = ResolveType(Type);
        return $"{Name}: {resolvedType}";
    }

    // todo this likely already exists with HC
    // Also need to handle nullability 
    private static string ResolveType(string type)
    {
        return type switch
        {
            "string" => "String",
            "bool" => "Boolean",
            "integer" => "Int",
            _ => throw new ArgumentException($"Unknown type {type}")
        };
    }
}
