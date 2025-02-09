using Microsoft.Extensions.VectorData;

namespace AiAgent.API.Models
{
    public class VectorData
    {
        [VectorStoreRecordKey]
        public required ulong VectorId { get; set; }
        [VectorStoreRecordData(IsFullTextSearchable = true)]
        public required string RawText { get; set; }
        [VectorStoreRecordVector(Dimensions: 384, DistanceFunction.CosineSimilarity, IndexKind.Hnsw)]
        public required ReadOnlyMemory<float>? DescriptionEmbedding { get; set; }
    }
}