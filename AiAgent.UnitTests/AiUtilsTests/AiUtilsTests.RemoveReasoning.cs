using AiAgent.UnitTests.MockUpData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.SemanticKernel;

namespace AiAgent.UnitTests.AiUtilsTests
{
    public partial class AiUtilsTests
    {

        public string ReasoningOutput =
            """
                <think>
                Okay, so I need to figure out who Hermione Granger's parents are. 
                Let me start by recalling what I know from the Harry Potter books. Hermione is one of the main characters, 
                best friends with Harry and Ron. She's known for being super smart and a witch, but her parents are Muggles, right? 
                Wait, Muggles are non-magical people. So her parents don't have any magical abilities.
                </think>

                <think>
                I think their names are mentioned somewhere. Maybe Mr. and Mrs. Granger? 
                But do they have first names? I'm not sure. In the books, 
                I remember Hermione modifying her parents' memories to protect them during the war with Voldemort. 
                She used a spell to make them forget her and sent them away to Australia. 
                That was in the last book, "The Deathly Hallows
                </think>

                Hermione Granger's parents are Muggles (non-magical people) who play a background but significant role in the Harry Potter series. Here's a concise summary of their details:

                Names and Profession:

                In the books, they are referred to simply as Mr. and Mrs. Granger. Their first names are never explicitly revealed in the original seven novels or supplementary canon material (e.g., Pottermore).

                They are dentists by profession, a detail often highlighted to emphasize their Muggle background.
            """;
        public string ReasoningOutputParsed =
            """
            Hermione Granger's parents are Muggles (non-magical people) who play a background but significant role in the Harry Potter series. Here's a concise summary of their details:
            
            Names and Profession:
            
            In the books, they are referred to simply as Mr. and Mrs. Granger. Their first names are never explicitly revealed in the original seven novels or supplementary canon material (e.g., Pottermore).
            
            They are dentists by profession, a detail often highlighted to emphasize their Muggle background.
            """;

        [Fact]
        public void RemoveReasoning_ContentContainsThinkTag_ReturnParsedContent()
        {
            var chatMessageContent = new ChatMessageContent
            {
                Items = [new TextContent(ReasoningOutput)]
            };
            var chatMessageContentParsed = API.Utils.AiUtils.RemoveReasoning(chatMessageContent)
                .ToString()
                .Trim()
                .Replace("\r", "")
                .Replace("\n", "")
                .Replace(" ", "");
            Assert.Equal(chatMessageContentParsed, ReasoningOutputParsed.Trim().Replace(" ", "").Replace("\r", "").Replace("\n", ""));
        }
    }
}
