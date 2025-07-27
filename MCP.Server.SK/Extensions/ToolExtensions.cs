using Microsoft.Extensions.DependencyInjection;
using Microsoft.SemanticKernel;
using ModelContextProtocol.Server;

namespace MCP.Server.SK.Extensions
{
    public static class ToolExtensions
    {
        public static IMcpServerBuilder WithTools(this IMcpServerBuilder builder, KernelPluginCollection plugins)
        {
            foreach (var plugin in plugins)
            {
                foreach (var function in plugin)
                    builder.Services.AddSingleton(service => McpServerTool.Create(function.AsAIFunction()));
            }

            return builder;
        }
    }
}
