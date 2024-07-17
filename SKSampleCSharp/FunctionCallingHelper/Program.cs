﻿using System.Text.Json;
using Azure.AI.OpenAI;
using FunctionCallingHelper.Plugins;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;

namespace FunctionCallingHelper
{
#pragma warning  disable SKEXP0001
    internal class Program
    {
        static async Task Main(string[] args)
        {
            Console.ForegroundColor = ConsoleColor.White;

            Console.WriteLine("Hello,Microsoft Semantic Kernel - FunctionCalling");

            var kernel = CreateKernelBuilder();

            var chatService = kernel.GetRequiredService<IChatCompletionService>();

            OpenAIPromptExecutionSettings settings = new()
            {
                ToolCallBehavior = ToolCallBehavior.EnableKernelFunctions
            };

            var chatHistory = new ChatHistory();

            chatHistory.AddUserMessage("Could you provide me with the history of Austria and the current exchange rate of its currency to INR?");

            var modelResult = await chatService.GetChatMessageContentAsync(chatHistory, settings, kernel);

            Console.WriteLine(modelResult.Content);

            chatHistory.Add(modelResult);

            var functionCalls = FunctionCallContent.GetFunctionCalls(modelResult);

            foreach (var functionCallContent in functionCalls)
            {
                var functionResultContent = await functionCallContent.InvokeAsync(kernel);

                chatHistory.Add(functionResultContent.ToChatMessage());

                var modelToolResult = await chatService.GetChatMessageContentAsync(chatHistory, settings, kernel);

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine(modelToolResult.Content);
            }

            Console.WriteLine("\n\nPress any key to exit...");
            Console.Read();
        }


        private static Kernel CreateKernelBuilder()
        {
            //Create a kernel builder and add the Azure OpenAI Chat Completion service
            var builder = Kernel.CreateBuilder()
                .AddAzureOpenAIChatCompletion(Config.DeploymentOrModelId, Config.Endpoint, Config.ApiKey);

            builder.Plugins.AddFromType<CurrencyConverterPlugin>();
            
            return builder.Build()!;
        }
    }
}
