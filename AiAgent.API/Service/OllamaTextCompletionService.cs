using AiAgent.API.Configuration;
using AiAgent.API.Service.ChatManager;
using Microsoft.Extensions.Options;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Agents;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.Ollama;
using OllamaSharp;
using System.Text.Json;
using AiAgent.API.Service.Abstract;
using AiAgent.API.Utils;


namespace AiAgent.API.Service
{
    public class OllamaTextCompletionService : ITextCompletionService
    {
        private readonly ChatManagerService _chatManager;
        private readonly HttpService _httpService = null!;
        private readonly ILogger<OllamaTextCompletionService> _logger;
        private readonly EmbeddingService _embeddingService;
        private readonly IChatCompletionService _chatCompletionService;

        private readonly string _textCompletionModel;
        private readonly string? _textCompletionApiKey;
        private readonly string _textCompletionModelEndpoint;

        public OllamaTextCompletionService
        (
            IOptions<AgentConfiguration> agentConfiguration, 
            ChatManagerService chatManager,
            HttpService httpService,
            EmbeddingService embeddingService,
            ILogger<OllamaTextCompletionService> logger
        )
        {
            _logger = logger;

            _textCompletionModel = Environment.GetEnvironmentVariable("TEXT_COMPLETION_MODEL")
                    ?? throw new Exception("TEXT_COMPLETION_MODEL not found");
            _textCompletionApiKey = Environment.GetEnvironmentVariable("TEXT_COMPLETION_API_KEY");

            _textCompletionModelEndpoint = Environment.GetEnvironmentVariable("TEXT_COMPLETION_MODEL_ENDPOINT")
                    ?? throw new Exception("TEXT_COMPLETION_MODEL_ENDPOINT not found");

            _chatManager = chatManager;

            _embeddingService = embeddingService;

            var ollamaClient = new OllamaApiClient(
                client: new HttpClient
                {
                    BaseAddress = new Uri(_textCompletionModelEndpoint),
                    Timeout = TimeSpan.FromSeconds(200)
                },
                defaultModel: _textCompletionModel
            );
#pragma warning disable SKEXP0001
            _chatCompletionService = ollamaClient.AsChatCompletionService();

            _httpService = httpService;
        }

        public async Task<string> GetChatMessageContentAsync(string prompt)
        {
            string userIp = _httpService.GetClientIpAddress();

            _logger.LogInformation("Chat completion requested, User ID: {user}, Prompt: {prompt}", userIp, prompt);

            ChatHistory chatHistory = _chatManager.GetChatHistoryFromCache(userIp);

            if (AiUtils.CheckTokensThreshold(chatHistory, 300)) {
                await _chatManager.ReduceChatHistory(userIp);
            }

            _chatManager.AddMessageContentToCache(userIp, new ChatMessageContent
            {
                Role = AuthorRole.User,
                Items = [new TextContent {Text = prompt }]
            });

            await LoadEmbeddings(chatHistory, prompt);

            var response = await _chatCompletionService.GetChatMessageContentAsync(chatHistory);

            var responseParsed = AiUtils.RemoveReasoning(response);

            chatHistory.Add(responseParsed);

            _logger.LogInformation("Chat completion completed, User ID: {user}, Response: {response}", userIp, responseParsed);

            return responseParsed.ToString();
        }

        /// <summary>
        /// Use this method in flows that don't involve user interaction. Such as getting the summarization for 
        /// chat history reducer.
        /// </summary>
        /// <param name="chatHistory"></param>
        /// <returns></returns>
        public async Task<ChatMessageContent> GetChatMessageContentAsync(ChatHistory chatHistory)
        {
            var response = await _chatCompletionService.GetChatMessageContentAsync(chatHistory);
            return response;
        }

        private async Task LoadEmbeddings(ChatHistory chatHistory, string prompt)
        {
            var embeddings = await _embeddingService.SemanticSearch(prompt);
            foreach(var embedding in embeddings)
            {
                chatHistory.AddAssistantMessage(embedding.RawText);
            }
        }
    }
}
