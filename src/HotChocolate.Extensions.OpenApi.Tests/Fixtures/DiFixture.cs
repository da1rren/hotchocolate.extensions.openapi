namespace HotChocolate.Extensions.OpenApi.Tests.Fixtures;

using Microsoft.Extensions.DependencyInjection;

public class DiFixture
{
    public ServiceProvider ServiceProvider { get; }
    
    public DiFixture()
    {
        ServiceProvider = new ServiceCollection()
            .AddHttpClient()
            .BuildServiceProvider();
    }
}