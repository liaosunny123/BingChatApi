namespace BingChatApiLibs.Models;

public class NetworkMessage
{
    public string text { get; set; }

    public string messageType { get; set; } = "Chat";

    public string inputMethod { get; set; } = "Keyboard";

    public string author { get; set; } = "user";

    public DateTimeOffset timestamp { get; set; }

    public string region { get; set; }= "AU";
}