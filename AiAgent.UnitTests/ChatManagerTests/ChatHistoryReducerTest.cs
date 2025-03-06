using AiAgent.API.Service.Abstract;
using AiAgent.API.Service.ChatManager;
using AiAgent.UnitTests.MockUpData;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace AiAgent.UnitTests.ChatManagerTests
{
    public class ChatHistoryReducerTest
    {
        private readonly Mock<ITextCompletionService> _mockTextCompletionService;
        private readonly ChatHistoryReducer _chatHistoryReducer;

        public ChatHistoryReducerTest()
        {
            _mockTextCompletionService = new Mock<ITextCompletionService>();
            _chatHistoryReducer = new ChatHistoryReducer(_mockTextCompletionService.Object);
        }

        [Fact]
        public async Task ChatHistoryReducer_WhenHistoryIsTooLong_ReturnReducedChatHistory()
        {
            _mockTextCompletionService
                .Setup(service => service.GetChatMessageContentAsync(It.IsAny<ChatHistory>()))
                .ReturnsAsync(new ChatMessageContent(AuthorRole.Assistant, MockChatHistory.Summarization));

            var chatHistory = new MockChatHistory().chatHistory;

            var result = new ChatHistory(await _chatHistoryReducer.ReduceAsync(chatHistory));

            var mockReducedChatHistory = MockChatHistory.BuildReducedChatHistory();

            Assert.Equal(ContentGetter(result), ContentGetter(MockChatHistory.BuildReducedChatHistory()));
        }

        private string ContentGetter(ChatHistory chatHistory)
        {
            var builder = new StringBuilder();
            foreach (var message in chatHistory) builder.AppendLine(message.ToString());
            return builder.ToString();
        }
    }
}
