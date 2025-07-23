using System.Net;
using Ocelot.Middleware;
using Ocelot.Responder;
using Microsoft.Extensions.Primitives;

namespace IdentityService.Middlewares;

// SafeHttpResponder.cs

using System.Net;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using Ocelot.Middleware;
using Ocelot.Responder;

public class SafeHttpResponder : IHttpResponder
{
    public async Task SetErrorResponseOnContext(HttpContext context, DownstreamResponse response)
    {
        SetStatusCode(context, response.StatusCode);

        foreach (var header in response.Headers)
        {
            context.Response.Headers[header.Key] = new StringValues(header.Values.ToArray());
        }

        if (response.Content != null)
        {
            await context.Response.WriteAsync(await response.Content.ReadAsStringAsync());
        }
    }

    public void SetErrorResponseOnContext(HttpContext context, int statusCode)
    {
        SetStatusCode(context, (HttpStatusCode)statusCode);
    }

    public async Task SetResponseOnHttpContext(HttpContext context, DownstreamResponse response)
    {
        SetStatusCode(context, response.StatusCode);

        foreach (var header in response.Headers)
        {
            context.Response.Headers[header.Key] = new StringValues(header.Values.ToArray());
        }

        if (response.Content != null)
        {
            await context.Response.WriteAsync(await response.Content.ReadAsStringAsync());
        }
    }

    private void SetStatusCode(HttpContext context, HttpStatusCode statusCode)
    {
        context.Response.StatusCode = (int)statusCode;
        // Avoid setting ReasonPhrase – unsupported in ASP.NET Core
    }
}


