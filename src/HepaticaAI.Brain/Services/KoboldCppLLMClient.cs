﻿using System.Diagnostics;
using HepaticaAI.Brain.Models;
using HepaticaAI.Core.Interfaces.AI;
using HepaticaAI.Core.Interfaces.Memory;
using Microsoft.Extensions.Configuration;
using System.Text;
using System.Text.Json;
using HepaticaAI.Core.Models.Messages;

namespace HepaticaAI.Brain.Services
{
    internal class KoboldCppLLMClient(KoboldCppRunner koboldCppRunner, IConfiguration configuration, IMemory memory, ISystemPromptsUpdater systemPromptsUpdater) : ILLMClient
    {
        public void Initialize()
        {
            koboldCppRunner.StartKoboldCpp();
        }

        public async Task<string> GenerateAsync(string personality, string prompt)
        {
            try
            {
                memory.AddEntry(personality, prompt);

                var characterPersonality = JsonSerializer.Deserialize<Personality>(await File.ReadAllTextAsync("character_personality.json"));
                var aiUrl = configuration["AiUrl"];
                var aiMainCharacterName = systemPromptsUpdater.GetCharacterName();//Todo TEST IT 
                //var aiMainCharacterName = configuration["AiMainCharacterName"];

                memory.AddEntry(aiMainCharacterName!, string.Empty);

                //string promptToSend = memory.GetFormattedPrompt();
                string promptToSend = $"[{aiMainCharacterName} responds playfully]\n" + memory.GetFormattedPrompt();
                //string promptToSend = $"[LeporaAI responds playfully]\n" + memory.GetFormattedPrompt();

                characterPersonality!.prompt = promptToSend;

                memory.AddUserRoleToStopSequenceIfMissing(personality);

                memory.AddUserRoleToStopSequenceIfMissing(aiMainCharacterName!);

                characterPersonality!.stop_sequence = memory.GetStopSequence();

                var jsonBody = JsonSerializer.Serialize(characterPersonality, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });

                using var client = new HttpClient();

                var request = new HttpRequestMessage(HttpMethod.Post, $"{aiUrl}/api/v1/generate")
                {
                    Content = new StringContent(jsonBody, Encoding.UTF8, "application/json")
                };

                var response = await client.SendAsync(request);

                var responseContentJson = await response.Content.ReadAsStringAsync();

                var result = JsonSerializer.Deserialize<AiMessageResponseRoot>(responseContentJson);

                var responseContentText = result!.results.First().text;

                Debug.WriteLine($"Finish reason: {result.results.First().finish_reason}");

                memory.DeleteLastMessage();

                memory.AddEntry(aiMainCharacterName!, responseContentText);

                return responseContentText;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);

                return String.Empty;
            }
        }

        public void Dispose()
        {
            koboldCppRunner.StopKoboldCpp();
        }
    }
}
