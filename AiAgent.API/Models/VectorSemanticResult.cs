namespace AiAgent.API.Models
{
    public class VectorSemanticResult
    {
        public required double? Score { get; set; }
        public required string RawText { get; set; }
    }
}
