using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using TinyWebSockets.Interfaces;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace TinyWebSockets
{
    public delegate void WebSocketMessageEventArgs(JToken message);

    public delegate void WebSocketConnection(Uri uri);

    public class WebSocketService
    {
        private ClientWebSocket client = new ClientWebSocket();
        private CancellationTokenSource cancelConnectSource = new CancellationTokenSource();
        private CancellationTokenSource cancelMessageSource = new CancellationTokenSource();
        private CancellationTokenSource cancelSendSource = new CancellationTokenSource();
        private Uri _serverUrl;

        private const int BUFFER_SIZE = 1024 * 32;

        public event WebSocketMessageEventArgs MessageRecieved;

        public event WebSocketConnection Connected;

        private byte[] messageBuffer = new byte[BUFFER_SIZE];

        public async Task StartListening(Uri serverUrl = null)
        {
            if(serverUrl != null)
            {
				_serverUrl = serverUrl;
            }
            if (client.State == WebSocketState.Closed) {
                //Logger.Instance.LogError("Socket is closed");
                client = new ClientWebSocket();
                ScheduleNewConnection();
                return;
            }
            if (client.State != WebSocketState.Open && client.State != WebSocketState.Connecting)
            {
                try
                {
                    await client.ConnectAsync(_serverUrl, cancelConnectSource.Token);
                    Connected?.Invoke(_serverUrl);
                    await SendFromQueue();
                }
                catch(Exception ex) 
                {
                    //Logger.Instance.Log(ex);
                    OnError?.Invoke(this,ex);
                }
            }
        }

        internal void Pause()
        {
            cancelSendSource.Cancel();
            cancelConnectSource.Cancel();
            cancelMessageSource.Cancel();
        }

        private SemaphoreSlim sendLock = new SemaphoreSlim(1);

        private List<IMessage> sendQueue = new List<IMessage>();

        public void QueueAction(IMessage action) {
            var triggerSend = !sendQueue.Any();
            sendQueue.Add(action);
            if (client.State == WebSocketState.Open)
            {
                if (triggerSend)
                {

                    Task.Run(async () => await SendFromQueue());
                }
            }
            else {
                //Logger.Instance.LogInfo("Socket not open when sending, connecting...");
                Task.Run(async () =>
                {
                    await StartListening();
                });
            }
        }

        private bool _isSending = false;

        private async Task SendFromQueue()
        {
            var action = sendQueue.FirstOrDefault();
            if (action != null && !_isSending)
            {
                _isSending = true;
                if (string.IsNullOrEmpty(action.Type) && action is BaseMessage basemsg) {
                    PopulateType(basemsg);
                }
                var sendBuffer = System.Text.Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(action));
                var sendSegment = new ArraySegment<byte>(sendBuffer);

                try
                {
                    await client.SendAsync(sendSegment, WebSocketMessageType.Text, true, cancelSendSource.Token);
                    sendQueue.Remove(action);
                }
                catch(Exception ex) 
                {
                    _isSending = false;
                    //Logger.Instance.Log(ex, "SendFromQueue failed"); 
                    OnError?.Invoke(this, ex);
                }
                _isSending = false;
                await Task.Run(()=>SendFromQueue());
            }
        }

        private void PopulateType(BaseMessage action)
        {
            var msgAttr = action.GetType().GetCustomAttributes(typeof(MessageAttribute), true).OfType<MessageAttribute>().FirstOrDefault();
            if (msgAttr != null)
                action.Type = msgAttr.TypeName;
        }

        public event EventHandler<Exception> OnError;

        public async Task StartReceivingMessages()
        {
            try
            {
                //Logger.Instance.LogVerboseEnterMethod();

                var segment = new ArraySegment<byte>(messageBuffer);

                if (client == null)
                {
                    //Logger.Instance.LogWarning("StartReceivingMessage: Client is null");
                    return;
                }

                if (client.State != WebSocketState.Open)
                {
                    //Logger.Instance.LogWarning("StartReceivingMessage: Socket is not open");
                    return;
                }

                var result = await client.ReceiveAsync(segment, cancelMessageSource.Token);

                if (result.MessageType == WebSocketMessageType.Close)
                {
                    //Logger.Instance.LogError("Message type Close not supported");
                    await client.CloseAsync(WebSocketCloseStatus.InvalidMessageType, "Skit in, skit ut", CancellationToken.None);
                    return;
                }

                int count = result.Count;
                while (!result.EndOfMessage)
                {
                    if (count >= messageBuffer.Length)
                    {
                        //Logger.Instance.LogError("Message to long, buffer to small");
                        await client.CloseAsync(WebSocketCloseStatus.InvalidPayloadData, "That's too long", CancellationToken.None);
                        throw new WebSocketException(0, "Buffer to small");
                    }

                    segment = new ArraySegment<byte>(messageBuffer, count, messageBuffer.Length - count);
                    result = await client.ReceiveAsync(segment, cancelMessageSource.Token);
                    count += result.Count;
                }

                var message = System.Text.Encoding.UTF8.GetString(messageBuffer, 0, count);
                try
                {
                    MessageRecieved?.Invoke(JToken.Parse(message));
                }
                catch(Exception ex) {
                    //Logger.Instance.Log(ex,"MessageRecieved delegate error");
                }
                if (client.State == WebSocketState.Open)
                {
					await StartReceivingMessages();
                }
                else
                {
					await StartListening();
                }
            }
            catch (Exception ex)
            {
                //Logger.Instance.Log(ex,"WebSocketError");
                OnError?.Invoke(this, ex);
                ScheduleNewConnection();
                throw;
            }
        }

        private void ScheduleNewConnection() => Task.Delay(TimeSpan.FromSeconds(10)).ContinueWith(async task =>
        {
            //Logger.Instance.LogInfo("Reconnecting after delay");
            await StartListening();
        });
    }
}
