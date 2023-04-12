# BingChatApi

[![NuGet](https://img.shields.io/nuget/vpre/BingChatApiLibs?style=flat&label=NuGet&color=9866ca)](https://www.nuget.org/packages/BingChatApiLibs/)
[![NuGet Downloads](https://img.shields.io/nuget/dt/BingChatApiLibs?style=flat&label=Downloads&color=42a5f5)](https://www.nuget.org/packages/BingChatApiLibs/) 

The UNOFFICIAL NuGet for Bing Chat Api.

Some codes are modified from [Mirror.ChatGpt](https://github.com/yinanrong/Mirror.ChatGpt). This repo fixed wrong protocols and add more fuctions.

If you think this repo is useful please give this repo a star.

## Use

```csharp
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
```

# License

The codes modified from the original repo are under Apacha License 2.0.

Besides that, this repo is under GPLv3 license.
