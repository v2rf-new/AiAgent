namespace AiAgent.API.Configuration
{
    public class AgentConfiguration
    {
        public required string Name { get; set; }
        public required string Instructions { get; set; }
        public required string[] ExtraSystemConfiguration { get; set; }
    }
}
