using Microsoft.Extensions.AI;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using ModelContextProtocol.Client;

var builder = Host.CreateApplicationBuilder(args);

builder.Configuration
    .AddEnvironmentVariables()
    .AddUserSecrets<Program>();



var clientTransport = new StdioClientTransport(new()
{
    Name = "Demo Server",
    Command = "dotnet",
    Arguments = ["run", "--project", "../../01_Calculator/McpCalculatorServer/McpCalculatorServer.csproj"],
});

await using var mcpClient = await McpClient.CreateAsync(clientTransport);

foreach (var tool in await mcpClient.ListToolsAsync())
{
    Console.WriteLine($"{tool.Name} ({tool.Description})");
}

var result = await mcpClient.CallToolAsync(
    "add",
    new Dictionary<string, object?>() { ["a"] = 1, ["b"] = 3  },
    cancellationToken:CancellationToken.None);

Console.WriteLine($"Result of Add: {result.Content.First(c => c.Type == "text")}");
