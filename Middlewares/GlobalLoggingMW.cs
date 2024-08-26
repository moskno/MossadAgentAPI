using System.Text;
using System.Text.Json;

namespace MossadAgentAPI.Middlewares;

public class GlobalLoggingMW
{

    private readonly RequestDelegate _next;

    public GlobalLoggingMW(RequestDelegate next)
    {
        this._next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var request = context.Request;
        Console.WriteLine($"Got Request to server: {request.Method} {request.Path}\n" +
                          $"From IP: {request.HttpContext.Connection.RemoteIpAddress}");
        await this._next(context);
    }
}
