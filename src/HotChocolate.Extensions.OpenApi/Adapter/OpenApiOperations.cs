namespace HotChocolate.Extensions.OpenApi.Adapter;

using Microsoft.OpenApi.Models;

public interface IOpenApiOperation
{
    public string Name { get; }

    public OperationType OperationType { get; }
};

public record QueryOperation(string Name, OperationType OperationType) : IOpenApiOperation;

public record MutationOperation(string Name, OperationType OperationType) : IOpenApiOperation;
