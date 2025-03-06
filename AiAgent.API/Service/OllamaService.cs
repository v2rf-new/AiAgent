using Microsoft.SemanticKernel.Connectors.Ollama;
using Microsoft.SemanticKernel.Embeddings;
using OllamaSharp;
using System.Collections.ObjectModel;

namespace AiAgent.API.Service
{
    public class OllamaService
    {
        private readonly string _ollamaModel;
        private readonly string _ollamaUri;

#pragma warning disable SKEXP0001
        private readonly ITextEmbeddingGenerationService _textEmbeddingGenerationService;
        public OllamaService()
        {
            _ollamaModel = Environment.GetEnvironmentVariable("OLLAMA_MODEL") 
                ?? throw new Exception("OLLAMA_MODEL env var not found");

            _ollamaUri = Environment.GetEnvironmentVariable("OLLAMA_URI")
                ?? throw new Exception("OLLAMA_URI env var not found");

            _textEmbeddingGenerationService = 
                new OllamaApiClient(_ollamaUri, _ollamaModel).AsTextEmbeddingGenerationService();
        }

        public async Task<ReadOnlyMemory<float>> GenerateEmbeddingsAsync(string segment)
        {
            try
            {
                return await _textEmbeddingGenerationService.GenerateEmbeddingAsync(segment);
            }
            catch(Exception)
            {
                throw;
            }
        }
    }
}
