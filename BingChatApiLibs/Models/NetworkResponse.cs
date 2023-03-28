namespace BingChatApiLibs.Models;

public class NetworkResponse
{
    public int type { get; set; }
    
    public string target { get; set; }
    
    public NetworkResponseItem item { get; set; }
}