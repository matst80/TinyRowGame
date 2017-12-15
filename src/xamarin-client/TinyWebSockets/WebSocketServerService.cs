using System;
using Newtonsoft.Json.Linq;
using TinyWebSockets.Interfaces;

namespace TinyWebSockets
{
    public abstract class WebSocketServerService : ISocketService
    {
        public WebSocketServerService()
        {
        }

        //public abstract 

        public event EventHandler<string> MessageReceived;
        public event EventHandler<Exception> OnError;

        public void QueueAction(string obj)
        {
            
        }
    }
}
