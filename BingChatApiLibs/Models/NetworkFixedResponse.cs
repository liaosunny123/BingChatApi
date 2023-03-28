using System.Text.Json.Serialization;

namespace BingChatApiLibs.Models;

internal record NetworkFixedResponse
{
    [JsonPropertyName("type")]
    public int Type { get; set; }
    [JsonPropertyName("target")]
    public string Target { get; set; }
    [JsonPropertyName("arguments")]
    public List<Argument> Arguments { get; set; }

    public class AdaptiveCard
    {
        [JsonPropertyName("type")]
        public string Type { get; set; }
        [JsonPropertyName("version")]
        public string Version { get; set; }
        [JsonPropertyName("body")]
        public List<TextBlock> Body { get; set; }
    }

    public class TextBlock
    {
        [JsonPropertyName("type")]
        public string Type { get; set; }
        [JsonPropertyName("text")]
        public string Text { get; set; }
        [JsonPropertyName("wrap")]
        public bool Wrap { get; set; }
    }

    public class Feedback
    {
        [JsonPropertyName("tag")]
        public object Tag { get; set; }
        [JsonPropertyName("updateOn")]
        public object UpdatedOn { get; set; }
        [JsonPropertyName("type")]
        public string Type { get; set; }
    }

    public class Message
    {
        [JsonPropertyName("text")]
        public string Text { get; set; }
        [JsonPropertyName("author")]
        public string Author { get; set; }
        [JsonPropertyName("createdAt")]
        public DateTimeOffset CreatedAt { get; set; }
        [JsonPropertyName("timestamp")]
        public DateTimeOffset Timestamp { get; set; }
        [JsonPropertyName("messageId")]
        public string MessageId { get; set; }
        [JsonPropertyName("offense")]
        public string Offense { get; set; }
        [JsonPropertyName("adaptiveCards")]
        public List<AdaptiveCard> AdaptiveCards { get; set; }
        [JsonPropertyName("sourceAttributions")]
        public List<object> SourceAttributions { get; set; }
        [JsonPropertyName("feedback")]
        public Feedback Feedback { get; set; }
        [JsonPropertyName("contentOrigin")]
        public string ContentOrigin { get; set; }
        [JsonPropertyName("privacy")]
        public object Privacy { get; set; }
    }

    public class Argument
    {
        [JsonPropertyName("messages")]
        public List<Message> Messages { get; set; }
        [JsonPropertyName("requestId")]
        public string RequestId { get; set; }
    }
    
}