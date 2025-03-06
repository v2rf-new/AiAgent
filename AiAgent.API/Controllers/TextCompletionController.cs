using AiAgent.API.Prompts;
using AiAgent.API.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Razor.TagHelpers;
using AiAgent.API.Service.Abstract;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace AiAgent.API.Controllers
{
    [Route("text-completion")]
    [ApiController]
    public class TextCompletionController : ControllerBase
    {
        private readonly ITextCompletionService _textCompletionService;

        public TextCompletionController(ITextCompletionService textCompletionService)
        {
            _textCompletionService = textCompletionService;
        }

        [HttpPost]
        public async Task<ActionResult> Post([FromBody] UserPrompt value)
        {
            var response =  await _textCompletionService.GetChatMessageContentAsync(value.Prompt);
            return Ok(response);
        }
    }
}
