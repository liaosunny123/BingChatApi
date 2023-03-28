using System.Net;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using BingChatApiLibs.Models;
using RestSharp;

namespace BingChatApiLibs;

//Some methods are modified from Mirror.ChatGpt NuGet
public class BingChatClient
{
    private readonly BingChatSettings _chatSettings;

    private bool isStart = true;

    public NetworkResponse NetworkResponse;
    
    public event EventHandler<ChatProcess> MessageReceiveProcess;
    
    public BingChatClient(BingChatSettings settings)
    {
        _chatSettings = settings;
    }
    
    public async Task<BingResponse> ChatAsync(BingRequest request, CancellationToken cancellationToken)
    {
        if (isStart)
        {
            var converRes = await CreateNewConversation(cancellationToken);
            if (converRes?.result.value != "Success")
            {
                throw new($"Cannot create NewBing Chat conversation with error message:{converRes?.result.message}");
            }
            request.Session = new(0, converRes.conversationId, converRes.clientId, converRes.conversationSignature);
        }

        var bingRequest = new NetworkRequest()
        {
            invocationId = request.Session.InvocationId.ToString(),
            arguments = new ()
            {
                new()
                {
                    conversationId = request.Session.ConversationId,
                    optionsSets = GetDefaultOptions(),
                    allowedMessageTypes = GetDefaultResponseType(),
                    sliceIds = GetDefaultSlids(),
                    isStartOfSession = isStart,
                    Message = new NetworkMessage()
                    {
                        text = request.request,
                        timestamp = DateTimeOffset.Now
                    },
                    participant = new ParticipantModel()
                    {
                        id = request.Session.ClientId
                    },
                    conversationSignature = request.Session.Signature
                }
            }
        };

        using var ws = await CreateNewChatHub(cancellationToken);
        await Handshake(ws, cancellationToken);
        await SendMessageAsync(ws, bingRequest, cancellationToken);
        var res = await Receive(ws, cancellationToken);
        var invocationId = res == null ? 0 : request.Session.InvocationId + 1;
        isStart = false;
        return new(res, request.Session with { InvocationId = invocationId });
    }
    
    private async Task Handshake(WebSocket ws, CancellationToken cancellationToken)
    {
        await SendMessageAsync(ws, new
        {
            protocol = "json",
            version = 1
        }, cancellationToken);
        var res = await Receive(ws, cancellationToken);
        if (res == "{}")
        {
            await SendMessageAsync(ws, new
            {
                type = 6
            }, cancellationToken);
            return;
        }

        throw new($"handshake error:{res}");
    }
    
    public NetworkResponse? GetFUllResponse()
    {
        return NetworkResponse;
    }
    
    private async Task<string> Receive(WebSocket ws, CancellationToken cancellationToken)
    {
        var lastText = "";
        var i = 0;
        while (ws.State == WebSocketState.Open && !cancellationToken.IsCancellationRequested)
        {
            WebSocketReceiveResult result;
            var messages = new StringBuilder();
            do
            {
                var buffer = new byte[1024];
                result = await ws.ReceiveAsync(buffer, cancellationToken);
                var messageJson = Encoding.UTF8.GetString(buffer.Take(result.Count).ToArray());
                messages.Append(messageJson);
            } while (result is { EndOfMessage: false, MessageType: WebSocketMessageType.Text } && !cancellationToken.IsCancellationRequested);
            var objects = messages.ToString().Split("\u001e", StringSplitOptions.RemoveEmptyEntries);
            if (objects.Length <= 0)
                continue;
            var responseMsg = objects[0];
            var response = JsonSerializer.Deserialize<NetworkFixedResponse>(responseMsg)!;
            switch (response.Type)
            {
                case 1:
                    if (response is not { Arguments.Count: > 0 } || response.Arguments[0] is not { Messages.Count: > 0 })
                        continue;
                    var message = response.Arguments[0].Messages[0];
                    if (message.Author != "bot" || string.IsNullOrEmpty(message.Text))
                        continue;
                    message.Text = Regex
                        .Replace(message.Text, @"\[[^\]]+\]", "", RegexOptions.Compiled|RegexOptions.Multiline);
                    var thisText = message.Text.Length >= lastText.Length
                        ? message.Text[lastText.Length..]
                        : message.Text;
                    lastText = message.Text;
                    var init = i++ <= 0;
                    var end = thisText == "";
                    MessageReceiveProcess?.Invoke(this, new(init, end, thisText));
                    if (end)
                    {
                        return string.IsNullOrEmpty(message.Text) ? lastText : message.Text;
                    }
                    break;
                case 0:
                    return responseMsg;
                case 2:
                    NetworkResponse = JsonSerializer.Deserialize<NetworkResponse>(responseMsg)!;
                    return null;
                case 6:
                    await SendMessageAsync(ws, new
                    {
                        type = 6
                    }, cancellationToken);
                    continue;
                default:
                    continue;
            }
        }

        return null;
    }
    
    private async Task SendMessageAsync(WebSocket ws, object msg, CancellationToken cancellationToken)
    {
        var msgSend = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(msg) + "\u001e");
        await ws.SendAsync(msgSend, WebSocketMessageType.Text, true, cancellationToken);
    }
    
    private async Task<WebSocket> CreateNewChatHub(CancellationToken token)
    {
        var ws = new ClientWebSocket();
        if (!string.IsNullOrEmpty(_chatSettings.Proxy))
            ws.Options.Proxy = new WebProxy(_chatSettings.Proxy);
        var uri = new Uri("wss://sydney.bing.com/sydney/ChatHub");
        await ws.ConnectAsync(uri, token);
        return ws;
    }

    private async Task<InitResponse?> CreateNewConversation(CancellationToken token)
    {
        var client = new RestClient();
        var req = new RestRequest("https://www.bing.com/turing/conversation/create");
        req.AddHeader("sec-ch-ua", "\"Microsoft Edge\";v=\"111\", \"Not(A:Brand\";v=\"8\", \"Chromium\";v=\"111\"");
        req.AddHeader("sec-ch-ua-mobile", "?0");
        req.AddHeader("user-agent",
            "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/109.0.0.0 Safari/537.36");
        req.AddHeader("content-type", "application/json");
        req.AddHeader("accept", "application/json");
        req.AddHeader("sec-ch-ua-platform-version", "15.0.0");
        req.AddHeader("x-ms-client-request-id", Guid.NewGuid().ToString());
        req.AddHeader("x-ms-useragent", "azsdk-js-api-client-factory/1.0.0-beta.1 core-rest-pipeline/1.10.0 OS/Win32");
        req.AddHeader("referer", "https://www.bing.com/search?q=Bing+AI&qs=n&form=QBRE&sp=-1&lq=0");
        req.AddHeader("x-forwarded-for", "1.1.1.1");
        req.AddHeader("cookie", $"_U={_chatSettings.Token}");
        var res = await client.ExecuteAsync(req, token);
        if (string.IsNullOrEmpty(res.Content))
            return null;
        return JsonSerializer.Deserialize<InitResponse>(res.Content);
    }

    private List<string> GetDefaultOptions()
    {
        return _chatSettings.Style switch
        {
            BingChatSettings.CharStyle.Creative => new List<string>()
            {
                "nlu_direct_response_filter",
                "deepleo",
                "disable_emoji_spoken_text",
                "responsible_ai_policy_235",
                "enablemm",
                "h3imaginative",
                "clgalileo",
                "gencontentv3",
                "cachewriteext",
                "e2ecachewrite",
                "dv3sugg"
            },
            BingChatSettings.CharStyle.Balanced => new List<string>()
            {
                "nlu_direct_response_filter",
                "deepleo",
                "disable_emoji_spoken_text",
                "responsible_ai_policy_235",
                "enablemm",
                "galileo",
                "cachewriteext",
                "e2ecachewrite",
                "dv3sugg",
                "glpromptv3"
            },
            BingChatSettings.CharStyle.Precise => new List<string>()
            {
                "nlu_direct_response_filter",
                "deepleo",
                "disable_emoji_spoken_text",
                "responsible_ai_policy_235",
                "enablemm",
                "h3precise",
                "cachewriteext",
                "e2ecachewrite",
                "dv3sugg",
                "clgalileo"
            }
        };
    }

    private List<string> GetDefaultSlids()
    {
        return new List<string>()
        {
            "anidtestcf",
            "321bic62ups0",
            "sydpayajax",
            "sydperfinput",
            "303hubcancls0",
            "316cache_sss0",
            "323glpromptv3",
            "316e2ecache"
        };
    }

    private List<string> GetDefaultResponseType()
    {
        return new List<string>()
        {
            "Chat",
            "InternalSearchQuery",
            "InternalSearchResult",
            "Disengaged",
            "InternalLoaderMessage",
            "RenderCardRequest",
            "AdsQuery",
            "SemanticSerp",
            "GenerateContentQuery",
            "SearchQuery"
        };
    }
}