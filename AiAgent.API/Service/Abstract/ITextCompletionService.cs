using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;

namespace AiAgent.API.Service.Abstract
{
    public interface ITextCompletionService
    {
        public Task<string> GetChatMessageContentAsync(string prompt);
        public Task<ChatMessageContent> GetChatMessageContentAsync(ChatHistory chatHistory);
    }
}
