using System.IO;
using System.Text.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using RetailsEcosystem.Customer.API.Middleware;

namespace RetailsEcosystem.Customer.Tests.Middleware;

public class GlobalExceptionMiddlewareTests
{
    private static GlobalExceptionMiddleware Build(Func<HttpContext, Task> next, bool dev = false)
    {
        var logger = Mock.Of<ILogger<GlobalExceptionMiddleware>>();
        var env    = new Mock<IWebHostEnvironment>();
        env.Setup(e => e.EnvironmentName).Returns(dev ? "Development" : "Production");
        return new GlobalExceptionMiddleware(new RequestDelegate(next), logger, env.Object);
    }

    private static DefaultHttpContext NewContext()
    {
        var ctx = new DefaultHttpContext();
        ctx.Response.Body = new MemoryStream();
        return ctx;
    }

    private static async Task<(int status, JsonDocument body)> RunAsync(
        GlobalExceptionMiddleware sut, DefaultHttpContext ctx)
    {
        await sut.InvokeAsync(ctx);
        ctx.Response.Body.Seek(0, SeekOrigin.Begin);
        var json = await new StreamReader(ctx.Response.Body).ReadToEndAsync();
        return (ctx.Response.StatusCode, JsonDocument.Parse(json));
    }

    [Fact]
    public async Task Invoke_NoException_CallsNextDelegate()
    {
        var called = false;
        var sut = Build(_ => { called = true; return Task.CompletedTask; });
        await sut.InvokeAsync(NewContext());
        called.Should().BeTrue();
    }

    [Fact]
    public async Task HandleException_KeyNotFoundException_Returns404()
    {
        var sut = Build(_ => throw new KeyNotFoundException("not found"));
        var (status, body) = await RunAsync(sut, NewContext());

        status.Should().Be(404);
        body.RootElement.GetProperty("status").GetInt32().Should().Be(404);
        body.RootElement.GetProperty("title").GetString().Should().Be("Not Found");
    }

    [Fact]
    public async Task HandleException_UnauthorizedAccessException_Returns401()
    {
        var sut = Build(_ => throw new UnauthorizedAccessException("no access"));
        var (status, body) = await RunAsync(sut, NewContext());

        status.Should().Be(401);
        body.RootElement.GetProperty("title").GetString().Should().Be("Unauthorized");
    }

    [Fact]
    public async Task HandleException_InvalidOperationException_Returns400WithDetail()
    {
        var sut = Build(_ => throw new InvalidOperationException("bad state"));
        var (status, body) = await RunAsync(sut, NewContext());

        status.Should().Be(400);
        body.RootElement.GetProperty("detail").GetString().Should().Be("bad state");
    }

    [Fact]
    public async Task HandleException_UnhandledException_Returns500()
    {
        var sut = Build(_ => throw new Exception("crash"));
        var (status, body) = await RunAsync(sut, NewContext());

        status.Should().Be(500);
        body.RootElement.GetProperty("title").GetString().Should().Be("Internal Server Error");
    }

    [Fact]
    public async Task HandleException_SetsContentTypeToApplicationProblemJson()
    {
        var sut = Build(_ => throw new Exception("err"));
        var ctx = NewContext();
        await RunAsync(sut, ctx);
        ctx.Response.ContentType.Should().Be("application/problem+json");
    }

    [Fact]
    public async Task HandleException_Development_IncludesStackTrace()
    {
        var sut = Build(_ => throw new Exception("err"), dev: true);
        var (_, body) = await RunAsync(sut, NewContext());

        body.RootElement.TryGetProperty("stackTrace", out var st).Should().BeTrue();
        st.ValueKind.Should().NotBe(JsonValueKind.Null);
    }

    [Fact]
    public async Task HandleException_Production_StackTraceIsNull()
    {
        var sut = Build(_ => throw new Exception("err"), dev: false);
        var (_, body) = await RunAsync(sut, NewContext());

        body.RootElement.TryGetProperty("stackTrace", out var st).Should().BeTrue();
        st.ValueKind.Should().Be(JsonValueKind.Null);
    }
}
