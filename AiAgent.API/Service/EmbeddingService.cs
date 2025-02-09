using Microsoft.Extensions.AI;
using Qdrant.Client;
using Microsoft.SemanticKernel.Connectors.Qdrant;
using AiAgent.API.Models;
using Microsoft.Extensions.VectorData;
using System.Collections.ObjectModel;

namespace AiAgent.API.Service
{
    public class EmbeddingService
    {
        private ulong vectorId = 1;
        private IVectorStoreRecordCollection<ulong, VectorData> _vectorCollection;
        private readonly OllamaService _ollamaService;
        public EmbeddingService(OllamaService ollamaService)
        {
            var qdrantHostname = Environment.GetEnvironmentVariable("QDRANT_HOSTNAME")
                ?? throw new Exception("QDRANT_URI variable not found");

            var vector = new QdrantVectorStore(new QdrantClient(qdrantHostname));
            _vectorCollection = vector.GetCollection<ulong, VectorData>("vectordata");
            _ollamaService = ollamaService;
        }
        public async Task LoadChunksAsync(ReadOnlyCollection<string> segments)
        {
            await _vectorCollection.CreateCollectionIfNotExistsAsync();
            try
            {
                foreach (var segment in segments)
                {
                    await _vectorCollection.UpsertAsync(new VectorData
                    {
                        VectorId = vectorId,
                        RawText = segment.Trim(),
                        DescriptionEmbedding = await _ollamaService.GenerateEmbeddingsAsync(segment)
                    });
                    Console.WriteLine(vectorId);
                    vectorId++;
                }
            }
            catch(Exception ex)
            {
                throw;
            }
        }

        public async Task<ReadOnlyCollection<VectorSemanticResult>> SemanticSearch(string prompt)
        {
            var vectors = await _ollamaService.GenerateEmbeddingsAsync(prompt);
            var searchResult = await _vectorCollection.VectorizedSearchAsync(vectors, new VectorSearchOptions { Top = 3});
            var results = searchResult.Results;

            List<VectorSemanticResult> resultObjects = [];

            await foreach(var result in results)
            {
                resultObjects.Add
                    (
                        new VectorSemanticResult
                        {
                            Score = result.Score,
                            RawText = result.Record.RawText
                        }
                    );
            }

            return resultObjects.AsReadOnly();
        }
    }
}
