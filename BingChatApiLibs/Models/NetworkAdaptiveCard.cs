namespace BingChatApiLibs.Models;

public class NetworkAdaptiveCard
{
    public string Type { get; set; }
    
    public string Version { get; set; }
    
    public List<NetworkTextblock> Body { get; set; }
}