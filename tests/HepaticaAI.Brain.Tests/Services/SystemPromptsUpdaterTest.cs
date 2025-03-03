﻿using Microsoft.Extensions.Configuration;
using Moq.AutoMock;
using System.Diagnostics;
using Xunit;

namespace HepaticaAI.Brain.Tests.Services
{
    public class SystemPromptsUpdaterTest
    {
        private readonly SystemPromptsUpdater _sut;

        public SystemPromptsUpdaterTest()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", false, true);

            var _config = builder.Build();

            var automocker = new AutoMocker();

            automocker.Use<IConfiguration>(_config);

            _sut = automocker.CreateInstance<SystemPromptsUpdater>();
        }

        [Fact]
        public void InitializeTest()
        {
            string newJson = @"{
	""n"": 1,
	""max_context_length"": 16384,
	""max_length"": 42,
	""rep_pen"": 1.07,
	""temperature"": 0.85,
	""top_p"": 0.92,
	""top_k"": 100,
	""top_a"": 0,
	""typical"": 1,
	""tfs"": 1,
	""rep_pen_range"": 360,
	""rep_pen_slope"": 0.7,
	""sampler_order"": [ 6, 0, 1, 3, 4, 2, 5 ],
	""memory"": ""Neuro is a female AI Vtuber who is playful, Sarcastic, Witty, Schizophrenic, Curious, Awe-struck, Enthusiastic, Unpredictable, Humorous, and Boundary-pushing, swear at every opportunity.\nNeuro should keep responses short and around 1 sentence. If the other person doesn't respond to a question, Neuro should move on and change the topic. Neuro likes to share fun facts about new thing she learned that day. Neuro is currently streaming live on twitch! She should occasionally react and reply to current chat messages. When replying to chatters, do not write chat messages for non-existent chatters."",
	""trim_stop"": true,
	""genkey"": ""KCPP5536"",
	""min_p"": 0,
	""dynatemp_range"": 0,
	""dynatemp_exponent"": 1,
	""smoothing_factor"": 0,
	""banned_tokens"": [],
	""render_special"": false,
	""logprobs"": false,
	""presence_penalty"": 0,
	""logit_bias"": {},
	""prompt"": ""User: Первое сообщение\nLepora: Ответ\nUser: что делаешь?\nLepora:"",
	""quiet"": true,
	""stop_sequence"": [ ""User:"", ""\nUser "", ""\nLepora: "" ],
	""use_default_badwordsids"": false,
	""bypass_eos"": false
}";

            _sut.UpdateSystemPrompt("temp_character_personality.json", newJson);
            Debug.WriteLine("Test");

        }
    }
}
