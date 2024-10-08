﻿// Copyright (c) Khulnasoft Ltd. All rights reserved.
// Use_Json_Mode.cs

using System.Text.Json;
using System.Text.Json.Serialization;
using DevGpt.Core;
using DevGpt.OpenAI;
using DevGpt.OpenAI.Extension;
using Azure.AI.OpenAI;
using FluentAssertions;

namespace DevGpt.BasicSample;

public class Use_Json_Mode
{
    public static async Task RunAsync()
    {
        #region create_agent
        var apiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY") ?? throw new Exception("Please set OPENAI_API_KEY environment variable.");
        var model = "gpt-3.5-turbo";

        var openAIClient = new OpenAIClient(apiKey);
        var openAIClientAgent = new OpenAIChatAgent(
            openAIClient: openAIClient,
            name: "assistant",
            modelName: model,
            systemMessage: "You are a helpful assistant designed to output JSON.",
            seed: 0, // explicitly set a seed to enable deterministic output
            responseFormat: ChatCompletionsResponseFormat.JsonObject) // set response format to JSON object to enable JSON mode
            .RegisterMessageConnector()
            .RegisterPrintMessage();
        #endregion create_agent

        #region chat_with_agent
        var reply = await openAIClientAgent.SendAsync("My name is John, I am 25 years old, and I live in Seattle.");

        var person = JsonSerializer.Deserialize<Person>(reply.GetContent());
        Console.WriteLine($"Name: {person.Name}");
        Console.WriteLine($"Age: {person.Age}");

        if (!string.IsNullOrEmpty(person.Address))
        {
            Console.WriteLine($"Address: {person.Address}");
        }

        Console.WriteLine("Done.");
        #endregion chat_with_agent

        person.Name.Should().Be("John");
        person.Age.Should().Be(25);
        person.Address.Should().BeNullOrEmpty();
    }
}

#region person_class
public class Person
{
    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("age")]
    public int Age { get; set; }

    [JsonPropertyName("address")]
    public string Address { get; set; }
}
#endregion person_class
