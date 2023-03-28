namespace BingChatApiLibs.Models;

public record BingRequest(string request)
{
    public ConversationSession Session { get; set; }
};