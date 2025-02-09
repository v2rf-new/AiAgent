using AiAgent.API.Prompts;
using AiAgent.API.Service;
using AiAgent.API.Utils;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace AiAgent.API.Controllers;

[ApiController]
[Route("embedding-generation")]
public class EmbeddingGeneration : ControllerBase
{
    private readonly ILogger<EmbeddingGeneration> _logger;
    private readonly EmbeddingService _embeddingService;

    public EmbeddingGeneration(ILogger<EmbeddingGeneration> logger, EmbeddingService embeddingService)
    {
        _embeddingService = embeddingService;
        _logger = logger;
    }

    [HttpPost]
    [RequestSizeLimit(100 * 1024 * 1024)]
    public async ValueTask<ActionResult> UploadFile(IFormFile file)
    {
        if(file.ContentType == "text/plain")
        {
            var segments = await ChunksParser.SegmentFile(file);
            try
            {
                await _embeddingService.LoadChunksAsync(segments);
                return Ok();
            }
            catch(Exception ex)
            {
                _logger.LogError("Error: {Message}, \nStackTrace: {StackTrace}", ex.Message, ex.StackTrace);
                return Problem("An unexpected error occurred.", statusCode: 500);
            }
        }
        else
        {
            return BadRequest("Incorrect file");
        }
    }

    [HttpPost("semantic-search")]
    public async ValueTask<ActionResult> SemanticSearch([FromBody] SemanticSearchPrompt prompt)
    {
        var results = await _embeddingService.SemanticSearch(prompt.Prompt);
        List<string> output = [];
        foreach(var result in results)
        {
            output.Add
                (
                    JsonSerializer.Serialize(result)
                );
        }
        return Ok(output);
    }
}
