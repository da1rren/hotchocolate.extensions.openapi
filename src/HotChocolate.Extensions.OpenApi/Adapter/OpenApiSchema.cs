namespace HotChocolate.Extensions.OpenApi.Adapter;

using System.Text;

public record OpenApiSchema(OpenApiType[] Types, IOpenApiOperation[] Operations)
{
    public override string ToString()
    {
        return string.Join(Environment.NewLine, Types.Select(x => x.ToString()));
    }
}

public record OpenApiType(string Name, params OpenApiTypeField[] Fields)
{
    public override string ToString()
    {
        return @$"
type {Name} {{ 
    {string.Join(Environment.NewLine, Fields.Select(x => x.ToString()))}
}}";
    }
}

public record OpenApiTypeField(string Name, string Type)
{
    public override string ToString()
    {
        return $"{Name}: {Type}";
    }
}
