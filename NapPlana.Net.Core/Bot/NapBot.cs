using NapPlana.Core.API;
using NapPlana.Core.Connections;
using NapPlana.Core.Data;
using NapPlana.Core.Data.API;

namespace NapPlana.Core.Bot;

public class NapBot
{
    private ConnectionBase _connection;
    public  long SelfId = 0;
    
    public NapBot()
    {
        // Default to a dummy connection; should be set properly later
        _connection = new ConnectionBase();
    }

    // Added: constructor that accepts a connection
    public NapBot(ConnectionBase connection,long selfId)
    {
        _connection = connection;
        SelfId = selfId;
    }

    // Added: fluent setter for the connection
    public NapBot SetConnection(ConnectionBase connection)
    {
        _connection = connection;
        return this;
    }

    // Added: lifecycle helpers
    public Task StartAsync() => _connection.InitializeAsync();
    public Task StopAsync() => _connection.ShutdownAsync();
    
    /// <summary>
    /// 发送群消息
    /// </summary>
    /// <param name="groupMessage">请求</param>
    /// <returns>响应</returns>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="InvalidOperationException"></exception>
    /// <exception cref="TimeoutException"></exception>
    public async Task<GroupMessageSendResponseData> SendGroupMessageAsync(GroupMessageSend groupMessage)
    {
        if (groupMessage is null) throw new ArgumentNullException(nameof(groupMessage));
        
        var echo = Guid.NewGuid().ToString();
        await _connection.SendMessageAsync(ApiActionType.SendGroupMsg, groupMessage, echo);
        
        var timeout = TimeSpan.FromSeconds(15);
        var start = DateTime.UtcNow;

        while (DateTime.UtcNow - start < timeout)
        {
            if (ApiHandler.TryConsume(echo, out var response))
            {
                if (response.RetCode != 0)
                {
                    throw new InvalidOperationException($"send_group_msg failed: {response.RetCode} - {response.Message}");
                }
                var data = response.GetData<GroupMessageSendResponseData>();
                if (data != null)
                {
                    return data;
                }
                throw new InvalidOperationException("Failed to parse send_group_msg response data.");
            }

            await Task.Delay(50);
        }

        throw new TimeoutException("Timed out waiting for send_group_msg response.");
    }
    
    /// <summary>
    /// 发送戳一戳消息
    /// </summary>
    /// <param name="pokeMessage">信息结构</param>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="InvalidOperationException"></exception>
    /// <exception cref="TimeoutException"></exception>
    public async Task SendPokeAsync(PokeMessageSend pokeMessage)
    {
        if (pokeMessage is null) throw new ArgumentNullException(nameof(pokeMessage));
        
        var echo = Guid.NewGuid().ToString();
        await _connection.SendMessageAsync(ApiActionType.SendPoke, pokeMessage, echo);
        
        var timeout = TimeSpan.FromSeconds(15);
        var start = DateTime.UtcNow;

        while (DateTime.UtcNow - start < timeout)
        {
            if (ApiHandler.TryConsume(echo, out var response))
            {
                if (response.RetCode != 0)
                {
                    throw new InvalidOperationException($"send_poke failed: {response.RetCode} - {response.Message}");
                }
                return;
            }

            await Task.Delay(50);
        }

        throw new TimeoutException("Timed out waiting for send_poke response.");
    }
}