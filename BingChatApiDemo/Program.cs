// See https://aka.ms/new-console-template for more information

using BingChatApiLibs;
using BingChatApiLibs.Models;

var options = new BingChatSettings()
{
    Style = BingChatSettings.CharStyle.Creative,
    Token  = "",
};
var bing = new BingChatClient(options);

BingResponse response = null;

var chatCts = new CancellationTokenSource();

chatCts.CancelAfter(TimeSpan.FromMinutes(5));

BingRequest request = new BingRequest("Hello");

response = await bing.ChatAsync(request, chatCts.Token);

Console.Write(response.Text);

BingRequest request2 = new BingRequest("What is your name") {Session = response.Session};

response = await bing.ChatAsync(request2, chatCts.Token);

Console.Write(response.Text);