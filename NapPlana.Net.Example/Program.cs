using NapPlana.Core.Bot;
using NapPlana.Core.Data;
using NapPlana.Core.Data.API;
using NapPlana.Core.Data.Message;
using NapPlana.Core.Event.Handler;

var bot = BotFactory
    .Create()
    .SetConnectionType(BotConnectionType.WebSocketClient)
    .SetIp("172.17.21.238")
    .SetPort(6100)
    .SetToken("plana-bot")
    .Build();
BotEventHandler.OnLogReceived += (level, message) =>
{
    if (level == LogLevel.Debug)
    {
        return;
    }
    Console.WriteLine($"[{level}] {message}");
};
BotEventHandler.OnMessageSentGroup += (eventData) =>
{
    Console.WriteLine($"消息类型 {eventData.MessageType}, 消息ID: {eventData.MessageId}");
};


var cts = new CancellationTokenSource();
Console.CancelKeyPress += async (s, e) =>
{
    e.Cancel = true;
    await bot.StopAsync();
    cts.Cancel();
};

await bot.StartAsync();

//发送信息测试
var res  = await bot.SendGroupMessageAsync(new GroupMessageSend()
{
    GroupId = "769372512",
    Message =
    [
        new TextMessage()
        {
            MessageType = MessageDataType.Text, MessageData = new TextMessageData() { Text = "bot online" }
        }
    ]
});

Console.WriteLine(res.MessageId);
try
{
    await Task.Delay(Timeout.Infinite, cts.Token);
}
catch (TaskCanceledException)
{
}
