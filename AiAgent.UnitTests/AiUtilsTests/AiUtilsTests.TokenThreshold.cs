using AiAgent.API.Utils;
using AiAgent.UnitTests.MockUpData;
using Microsoft.ML.Tokenizers;
using System.Text;

namespace AiAgent.UnitTests.AiUtils
{
    public partial class AiUtilsTests
    {
        [Theory]
        [InlineData(400)]
        public void CheckTokensThreshold_ExceedThreshold_ReturnTrue(int tokenThreshold)
        {
            var mockChatHistory = new MockChatHistory();
            var content = new StringBuilder();
            foreach(var message in mockChatHistory.chatHistory)
            {
                content.AppendLine(message.ToString());
            }
            var check = API.Utils.AiUtils.CheckTokensThreshold(mockChatHistory.chatHistory, tokenThreshold);
            Assert.True(check);
        }

        [Theory]
        [InlineData(500)]
        public void CheckTokensThreshold_NotExceedThreshold_ReturnFalse(int tokenThreshold)
        {
            var mockChatHistory = new MockChatHistory();
            var content = new StringBuilder();
            foreach (var message in mockChatHistory.chatHistory)
            {
                content.AppendLine(message.ToString());
            }
            var check = API.Utils.AiUtils.CheckTokensThreshold(mockChatHistory.chatHistory, tokenThreshold);
            Assert.False(check);
        }
    }
}
