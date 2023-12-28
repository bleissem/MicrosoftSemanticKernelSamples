﻿using Microsoft.SemanticKernel;
using NativePlugin.Plugins.WeatherPlugin;

namespace NativePlugin
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Hello, Microsoft Semantic Kernel - 1.0 - Native Plugin");

            Console.WriteLine("Enter the text with city name");

            var userInput = Console.ReadLine();

            Console.ForegroundColor = ConsoleColor.Green;

            //Create a kernel builder and add the Azure OpenAI Chat Completion service
            var kernelBuilder = Kernel.CreateBuilder()
                .AddAzureOpenAIChatCompletion(Config.DeploymentOrModelId, Config.Endpoint, Config.ApiKey)
                .Build();

            //Import Plugin
            kernelBuilder.ImportPluginFromType<ExternalWeatherPlugin>(nameof(ExternalWeatherPlugin));

            var weatherKernelFunction =
                kernelBuilder.Plugins.GetFunction(nameof(ExternalWeatherPlugin), ExternalWeatherPlugin.GetWeather);

            //KernelArguments 
            var kernelArguments = new KernelArguments
            {
                { "input", userInput }
            };

            //Invoke the kernel function
            var result = await kernelBuilder.InvokeAsync(weatherKernelFunction, kernelArguments);

            //Print the result
            Console.WriteLine(result.GetValue<string>());
            Console.Read();

        }
    }
}
