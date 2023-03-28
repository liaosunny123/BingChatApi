namespace BingChatApiLibs;

public class BingChatSettings
{
    /// <summary>
    /// The value of the cookie whose name is _U.
    /// </summary>
    public string Token { get; set; }
    
    /// <summary>
    /// The web proxy of the BingChat Client.
    /// </summary>
    public string Proxy { get; set; }

    /// <summary>
    /// The default Timeout Span of the BingChar Client Web process.
    /// </summary>
    public double TimeOutSpan { get; set; } = 60.0;

    /// <summary>
    /// Style of the NewBing.
    /// </summary>
    public CharStyle Style { get; set; }
    
    /// <summary>
    /// The extern options for chatting process.
    /// </summary>
    public IEnumerable<string> ChatOptions { get; set; }

    public enum CharStyle
    {
        Creative,
        Balanced,
        Precise,
    }
}