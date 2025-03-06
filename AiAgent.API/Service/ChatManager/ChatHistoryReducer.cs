using AiAgent.API.Service.Abstract;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;

namespace AiAgent.API.Service.ChatManager
{
# pragma warning disable SKEXP0001
    public class ChatHistoryReducer : IChatHistoryReducer
    {
        private readonly ITextCompletionService _textCompletionService;

        public ChatHistoryReducer(ITextCompletionService textCompletionService)
        {
            _textCompletionService = textCompletionService;
        }

        public string SummarizationInstruction =
            """
            Provide a concise and complete summarization of the entire dialog that does not exceed 5 sentences.
            this includes both messages of the the user and the assistant.
            Make an overview to give context in the upcoming conversations.
            Don't include this message as part of the summarization!!.
            """;
        public async Task<IEnumerable<ChatMessageContent>?> ReduceAsync(IReadOnlyList<ChatMessageContent> chatHistory, CancellationToken cancellationToken = default)
        {
            var systemMessages = chatHistory.Where(chatMessage => chatMessage.Role == AuthorRole.System).ToList();
            var chatMessages = chatHistory.Where(chatMessage => chatMessage.Role != AuthorRole.System);
            var chatHistoryObject = new ChatHistory(chatMessages);
            chatHistoryObject.AddSystemMessage(SummarizationInstruction);

            var summarization = await _textCompletionService.GetChatMessageContentAsync(chatHistoryObject);
            systemMessages.Add(summarization);
            return systemMessages;
        }
    }
}
