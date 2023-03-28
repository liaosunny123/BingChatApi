namespace BingChatApiLibs.Models;

internal class NetworkRequestArgument
{
    public string source { get; set; } = "cib";
    
    public string conversationId { get; set; }

    public List<string> optionsSets { get; set; } = new ();

    public List<string> allowedMessageTypes { get; set; } = new();
    
    public List<string> sliceIds { get; set; } = new();

    public string verbosity { get; set; } = "verbose";

    public bool isStartOfSession { get; set; }
    
    public NetworkMessage Message { get; set; }
    
    public ParticipantModel participant { get; set; }
    
    public string conversationSignature { get; set; }
}

internal class ParticipantModel
{
    public string id { get; set; }
}