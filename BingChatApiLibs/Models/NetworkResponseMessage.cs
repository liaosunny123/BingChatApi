namespace BingChatApiLibs.Models;

public class NetworkResponseMessage
{
    public string Text { get; set; }

    public string Author { get; set; }
    
    public DateTimeOffset CreatedAt { get; set; }
    
    public DateTimeOffset Timestamp { get; set; }
    
    public string MessageId { get; set; }

    public string Offense { get; set; } = "None";
    
    public List<NetworkAdaptiveCard> AdaptiveCards { get; set; }
    
    public List<object> SourceAttributions { get; set; }
    
    public NetworkFeedback Feedback { get; set; }
    
    public string ContentOrigin { get; set; }
    
    public object Privacy { get; set; }
}