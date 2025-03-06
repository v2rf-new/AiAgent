using Microsoft.AspNetCore.Components.Forms;
using Microsoft.ML.Tokenizers;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using System.Text;

namespace AiAgent.API.Utils
{
    public static partial class AiUtils
    {
        public static bool CheckTokensThreshold(ChatHistory chatHistory, int tokensLimit)
        {
            var content = new StringBuilder();
            foreach(var message in chatHistory)
            {
                content.AppendLine(message.ToString());
            }

            Tokenizer tokenizer = TiktokenTokenizer.CreateForModel("gpt-4");
            var tokensCount = tokenizer.CountTokens(content.ToString());

            return tokensCount > tokensLimit;

        }

        public static ChatMessageContent RemoveReasoning(ChatMessageContent chatMessageContent)
        {
            var content = chatMessageContent.ToString();

            int thinkIndex = content.LastIndexOf("</think>");

            string resultText = content.Substring(thinkIndex + "</think>".Length);

            return new ChatMessageContent
            {
                Role = AuthorRole.Assistant,
                Items = [
                    new TextContent(resultText)
                    ]
            };
        }
    }
}
