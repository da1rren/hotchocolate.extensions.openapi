namespace HotChocolate.Extensions.OpenApi.Adapter;

using Extensions;
using Microsoft.OpenApi.Models;

public interface IOpenApiOperation
{
    public string Name { get; }

    public OperationType OperationType { get; }
};

public record QueryOperation(string Name, OperationType OperationType, string ResultType) : IOpenApiOperation
{
    public override string ToString()
    {
        return $"{Name.FirstCharToLower()}: {ResultType}";
    }
}

public record MutationOperation(string Name, OperationType OperationType) : IOpenApiOperation
{
    public override string ToString()
    {
        return base.ToString();
    }
}
