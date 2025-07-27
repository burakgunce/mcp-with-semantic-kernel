﻿using Microsoft.SemanticKernel;
using Serilog;
using System.ComponentModel;
using System.Text.Json;

namespace MCP.Server.SK.Plugins
{
    public sealed class TodoPlugin(IHttpClientFactory httpClientFactory)
    {
        [KernelFunction, Description("Kullanıcılara dair yapılacaklar listesini ve durumlarını getirir.")]
        public async Task<List<object>> GetTodoListAsync()
        {
            Log.Information("Yapılacaklar listesi çekiliyor...");

            HttpClient httpClient = httpClientFactory.CreateClient();
            HttpResponseMessage httpResponseMessage = await httpClient.GetAsync("https://jsonplaceholder.typicode.com/todos");
            string jsonData = await httpResponseMessage.Content.ReadAsStringAsync();

            JsonDocument document = JsonDocument.Parse(jsonData);
            JsonElement root = document.RootElement;

            List<object> usersData = new();

            foreach (JsonElement element in root.EnumerateArray())
            {
                usersData.Add(new
                {
                    UserId = element.GetProperty("userId").GetInt32(),
                    Id = element.GetProperty("id").GetInt32(),
                    Title = element.GetProperty("title").GetString(),
                    Completed = element.GetProperty("completed").GetBoolean()
                });
            }

            Log.Information("Yapılacaklar listesi çekildi...");
            return usersData;
        }
    }
}