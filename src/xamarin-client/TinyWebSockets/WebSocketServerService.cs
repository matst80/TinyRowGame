using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using TinyWebSockets.Interfaces;

namespace TinyWebSockets
{
    public abstract class WebSocketServerService : ISocketService
    {
        private WebSocket _webSocket;
        private IList<string> sendQueue = new List<string>();

        public WebSocketServerService(WebSocket webSocket)
        {
            _webSocket = webSocket;
            Task.Run(async () => await HandleSocketAsync(webSocket));
        }

        public event EventHandler<string> MessageReceived;
        public event EventHandler<Exception> OnError;
        public event EventHandler OnDisconnect;

        private const int BUFFER_SIZE = 1024 * 32;
        private byte[] messageBuffer = new byte[BUFFER_SIZE];

        public async Task HandleSocketAsync(WebSocket webSocket)
        {
            var segment = new ArraySegment<byte>(messageBuffer);
            var result = await webSocket.ReceiveAsync(segment, CancellationToken.None);
            int count = result.Count;

            while (!result.EndOfMessage && !result.CloseStatus.HasValue)
            {
                segment = new ArraySegment<byte>(messageBuffer, count, messageBuffer.Length - count);
                result = await webSocket.ReceiveAsync(segment, CancellationToken.None);
                count += result.Count;
            }

            var message = System.Text.Encoding.UTF8.GetString(messageBuffer, 0, count);
            MessageReceived?.Invoke(this, message);

            while (!result.CloseStatus.HasValue)
            {
                await HandleSocketAsync(webSocket);
            }
            OnDisconnect?.Invoke(this, new EventArgs());
        }

        public void QueueAction(string action)
        {
            var triggerSend = !sendQueue.Any();
            sendQueue.Add(action);

            if (_webSocket.State == WebSocketState.Open && triggerSend)
            {
                Task.Run(async () => await SendFromQueue());
            }
        }

        private bool _isSending = false;

        private async Task SendFromQueue()
        {
            var action = sendQueue.FirstOrDefault();
            if (action != null && !_isSending)
            {
                _isSending = true;

                var sendBuffer = System.Text.Encoding.UTF8.GetBytes(action);
                var sendSegment = new ArraySegment<byte>(sendBuffer);

                try
                {
                    await _webSocket.SendAsync(sendSegment, WebSocketMessageType.Text, true, CancellationToken.None);
                    sendQueue.Remove(action);
                }
                catch (Exception ex)
                {
                    _isSending = false;
                    //Logger.Instance.Log(ex, "SendFromQueue failed"); 
                    OnError?.Invoke(this, ex);
                }
                _isSending = false;

                await Task.Run(() => SendFromQueue());
            }
        }
    }
}
