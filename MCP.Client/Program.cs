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
        prompt: "T�m kullan�c�lar� ve bu kullan�c�lar�n yap�lacak listelerini istiyorum. bunlar� ekranda [kullan�c� ad� - yap�lacak] �eklinde listelemeni istiyorum!",
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
        prompt: "T�m kullan�c�lar� getir? Ka� adet olduklar�n�, username ve email bilgilerini '[username | email]' format�nda yazman� istiyorum. Ayr�ca s�ralamay� username'e g�re alfabetik olarak tersine yapman� istiyorum. Ard�ndan, ---------- �izgi ile sayfay� ay�r ve �u dediklerimi yap: bir �irket senaryosunda gibi davranman� ve bu kullan�c�lardan 2 tanesini bu �irkette m�d�r olarak se�meni ve di�erlerine de kendine g�re belirledi�in senaryolarda farkl� g�revler atayarak bu y�neticiler alt�nda payla�t�rman� istiyorum. M�� gibi yapacak ve bir hikaye �izeceksin anlayaca��n.",
        executionSettings: new OpenAIPromptExecutionSettings()
        {
            FunctionChoiceBehavior = FunctionChoiceBehavior.Auto(options: new() { }),
        },
        kernel: kernel);

    return content.Content.ToString();
});
#endif

app.Run();
