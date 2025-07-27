#define Example1

using MCP.Client;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using ModelContextProtocol.Client;
using OpenAI;
using System.ClientModel;

var builder = WebApplication.CreateBuilder(args);

GoogleGeminiAIMCPClient mcpClient = new("MCP.Server", Path.Combine("..", "MCP.Server", "bin", "Debug", "net9.0", "MCP.Server.exe"));

GoogleGeminiAIMCPClient mcpClient2 = new("MCP.Server.SK", Path.Combine("..", "MCP.Server.SK", "bin", "Debug", "net9.0", "MCP.Server.SK.exe"));

IList<McpClientTool> tools = await mcpClient.GetToolListAsync();

foreach (var tool in await mcpClient2.GetToolListAsync())
    tools.Add(tool);

builder.Services
    .AddKernel()
    .AddOpenAIChatCompletion(
        modelId: "google/gemini-2.0-flash-exp:free",
        openAIClient: new OpenAIClient(
                credential: new ApiKeyCredential("sk-or-v1-****"),
                options: new OpenAIClientOptions
                {
                    Endpoint = new Uri("https://openrouter.ai/api/v1"),
                }
            )
    )
    .Plugins.AddFromFunctions("UserTool", tools.Select(_tool => _tool.AsKernelFunction()));

var app = builder.Build();

#if Example1
app.MapGet("/", async (IChatCompletionService chatCompletionService, Kernel kernel) =>
{
    ChatMessageContent content = await chatCompletionService.GetChatMessageContentAsync(
        prompt: "Tüm kullanýcýlarý ve bu kullanýcýlarýn yapýlacak listelerini istiyorum. bunlarý ekranda [kullanýcý adý - yapýlacak] þeklinde listelemeni istiyorum!",
        executionSettings: new OpenAIPromptExecutionSettings()
        {
            FunctionChoiceBehavior = FunctionChoiceBehavior.Auto(options: new() { }),
        },
        kernel: kernel);

    return content.Content.ToString();
});
#else
app.MapGet("/", async (IChatCompletionService chatCompletionService, Kernel kernel) =>
{
    ChatMessageContent content = await chatCompletionService.GetChatMessageContentAsync(
        prompt: "Tüm kullanýcýlarý getir? Kaç adet olduklarýný, username ve email bilgilerini '[username | email]' formatýnda yazmaný istiyorum. Ayrýca sýralamayý username'e göre alfabetik olarak tersine yapmaný istiyorum. Ardýndan, ---------- çizgi ile sayfayý ayýr ve þu dediklerimi yap: bir þirket senaryosunda gibi davranmaný ve bu kullanýcýlardan 2 tanesini bu þirkette müdür olarak seçmeni ve diðerlerine de kendine göre belirlediðin senaryolarda farklý görevler atayarak bu yöneticiler altýnda paylaþtýrmaný istiyorum. Mýþ gibi yapacak ve bir hikaye çizeceksin anlayacaðýn.",
        executionSettings: new OpenAIPromptExecutionSettings()
        {
            FunctionChoiceBehavior = FunctionChoiceBehavior.Auto(options: new() { }),
        },
        kernel: kernel);

    return content.Content.ToString();
});
#endif

app.Run();
