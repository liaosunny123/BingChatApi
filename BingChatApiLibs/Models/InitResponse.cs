namespace BingChatApiLibs.Models;

public record InitResponse
{
    public string conversationId { get; set; }
    
    public string clientId { get; set; }
    
    public string conversationSignature { get; set; }

    public InitResponseResult result { get; set; }
    
    public record InitResponseResult
    {
        public string value { get; set; }
        
        public string message { get; set; }
    }
};