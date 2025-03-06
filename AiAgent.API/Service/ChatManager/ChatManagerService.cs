using AiAgent.API.Configuration;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.ML.Tokenizers;

namespace AiAgent.API.Service.ChatManager
{
    public class ChatManagerService
    {
        private readonly IMemoryCache _memoryCache;
        private readonly AgentConfiguration _agentConfiguration;
#pragma warning disable SKEXP0001
        private readonly Lazy<IChatHistoryReducer> _historyReducer;

        public ChatManagerService(IOptions<AgentConfiguration> agentConfiguration, Lazy<IChatHistoryReducer> historyReducer)
        {
            var memoryCacheOptions = new MemoryCacheOptions
            {
                CompactionPercentage = 0.2,
                ExpirationScanFrequency = TimeSpan.FromMinutes(2)
            };

            _memoryCache = new MemoryCache(memoryCacheOptions);

            _agentConfiguration = agentConfiguration.Value 
                ?? throw new Exception("AgentConfiguration file not found");

            _historyReducer = historyReducer;
        }

        public void AddMessageContentToCache(string userIp, ChatMessageContent chatMessageContent)
        {
            if (!_memoryCache.TryGetValue(userIp, out ChatHistory? chatHistory))
            {
                chatHistory = new ChatHistory();
                AddSystemMessages(chatHistory);
            }

            chatHistory!.Add(chatMessageContent);

            var cacheEntryOptions = new MemoryCacheEntryOptions()
                .SetSlidingExpiration(TimeSpan.FromMinutes(5));

            _memoryCache.Set(userIp, chatHistory, cacheEntryOptions);
        }

        public ChatHistory GetChatHistoryFromCache(string userIp)
        {
            _memoryCache.TryGetValue(userIp, out ChatHistory? chatHistory);

            if (chatHistory is null) throw new Exception("Cache chat history is null");

            return chatHistory;
        }

        public bool HasInitialized(string userIp)
        {
            if(_memoryCache.TryGetValue(userIp, out var _))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public async Task ReduceChatHistory(string userIp)
        {
            _memoryCache.TryGetValue(userIp, out ChatHistory? chatHistory);
            var reducedChatHistory = _historyReducer.Value.ReduceAsync(chatHistory);

            CancellationTokenSource source = new CancellationTokenSource();
            CancellationToken token = source.Token;

            CancellationTokenSource ctSource = new CancellationTokenSource();
            CancellationToken _ = ctSource.Token;

            var chatHistoryReduced = await chatHistory.ReduceAsync(_historyReducer.Value, _);

            ReduceChatHistory(userIp, chatHistoryReduced);
        }

        private void ReduceChatHistory(string userIp, ChatHistory chatHistoryReduced)
        {
            var cacheEntryOptions = new MemoryCacheEntryOptions()
                .SetSlidingExpiration(TimeSpan.FromMinutes(5));

            _memoryCache.Set(userIp, chatHistoryReduced, cacheEntryOptions);
        }

        private void AddSystemMessages(ChatHistory chatHistory)
        {
            chatHistory.AddSystemMessage(_agentConfiguration.Name);
            chatHistory.AddSystemMessage(_agentConfiguration.Instructions);
            if(_agentConfiguration.ExtraSystemConfiguration.Length > 0)
            {
                foreach(var systemConfiguration in _agentConfiguration.ExtraSystemConfiguration)
                {
                    chatHistory.AddSystemMessage(systemConfiguration);
                }
            }
        }
    }
}
