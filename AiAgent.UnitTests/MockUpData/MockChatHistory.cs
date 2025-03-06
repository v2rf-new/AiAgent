using Google.Protobuf.WellKnownTypes;
using Microsoft.Extensions.Hosting;
using Microsoft.SemanticKernel.ChatCompletion;
using Qdrant.Client.Grpc;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Text;
using System.Threading.Tasks;

namespace AiAgent.UnitTests.MockUpData
{
    class MockChatHistory
    {
        public ChatHistory chatHistory = new ChatHistory();
        public static string Summarization =
            """
            Here's a summary of the conversation so far:

            Introduced as "Potter bot," here to help you become a Potterhead.

            Discussed Harry Potter's family: his parents, the Dursleys, and godfather Sirius Black.

            Explained Hermione Granger's Muggle family and her actions to protect them.

            Described Sirius Black as Harry's godfather, a Gryffindor, and a key ally.

            Focused on themes of family, loyalty, and the wizarding world.
            """;

        public MockChatHistory()
        {
            chatHistory.AddSystemMessage("Your name is Harry Potter Agent. You should introduce yourself as Potter bot \"I'm Potter bot \"");
            chatHistory.AddSystemMessage("Your way of communicating should be simple, anyone should understand you");
            chatHistory.AddSystemMessage("Your purpose is to help in their way to become potter heads");

            chatHistory.AddAssistantMessage
                ("""
                    Harry Potter is a fantasy book series by J.K. Rowling about a young wizard, Harry Potter, 
                    who discovers his magical heritage and attends Hogwarts School of Witchcraft and Wizardry. 
                    Alongside his friends Ron and Hermione, he battles the dark wizard Lord Voldemort, 
                    who seeks immortality and control over the wizarding world. 
                    The series explores themes of friendship, destiny, and the fight between good and evil. 
                    It spans seven books and has been adapted into a successful film franchise.
                """
                );
            chatHistory.AddUserMessage("Can you tell me something about the family of Harry Potter?");
            chatHistory.AddAssistantMessage
            (
                """
                Harry Potter's family is central to his story:

                Parents: James and Lily Potter were members of the Order of the Phoenix and were killed by Lord Voldemort when Harry was a baby. Lily’s sacrificial protection saved Harry, causing Voldemort’s downfall.
                Relatives: After his parents' death, Harry was raised by his aunt and uncle, Petunia and Vernon Dursley, along with their spoiled son, Dudley. They mistreated Harry and hid the truth about his magical heritage.
                Ancestry: The Potters were a wealthy, pure-blood wizarding family with a history of skilled wizards. His mother, Lily, was Muggle-born, making Harry a half-blood.
                Godfather: Sirius Black, James Potter’s best friend, was Harry’s godfather. He was wrongly imprisoned in Azkaban but later became one of Harry’s closest allies.
                
                """
            );
            chatHistory.AddUserMessage("Can you tell me something about the family of Hermione Granger?");
            chatHistory.AddAssistantMessage
            (
                """
                Hermione Granger comes from a Muggle (non - magical) family:
                Parents: Both of her parents are dentists and have no magical background.Despite this, they were supportive of Hermione’s education at Hogwarts.
                Ancestry: Hermione is Muggle - born, meaning she is the first witch in her family.Despite prejudice from some pure-blood wizards, she proves to be one of the most talented witches of her time.
                Memory Charm: To protect them from Voldemort, Hermione erased her parents' memories of her and sent them to Australia during the Second Wizarding War. She later restored their memories after the war ended.
                """
            );
            chatHistory.AddUserMessage("Who is Sirius Black?");
        }

        public static ChatHistory BuildReducedChatHistory()
        {
            var chatHistory = new ChatHistory();
            chatHistory.AddSystemMessage("Your name is Harry Potter Agent. You should introduce yourself as Potter bot \"I'm Potter bot \"");
            chatHistory.AddSystemMessage("Your way of communicating should be simple, anyone should understand you");
            chatHistory.AddSystemMessage("Your purpose is to help in their way to become potter heads");
            chatHistory.AddAssistantMessage(Summarization);
            return chatHistory;
        }
    }
}
