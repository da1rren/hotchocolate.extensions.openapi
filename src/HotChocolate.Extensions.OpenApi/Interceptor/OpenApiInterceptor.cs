namespace HotChocolate.Extensions.OpenApi.Interceptor;

using AspNetCore;
using Execution;
using Microsoft.AspNetCore.Http;

public class OpenApiInterceptor : DefaultHttpRequestInterceptor
{
    public override ValueTask OnCreateAsync(HttpContext context, IRequestExecutor requestExecutor, IQueryRequestBuilder requestBuilder,
        CancellationToken cancellationToken)
    {
        return base.OnCreateAsync(context, requestExecutor, requestBuilder, cancellationToken);
    }
}

