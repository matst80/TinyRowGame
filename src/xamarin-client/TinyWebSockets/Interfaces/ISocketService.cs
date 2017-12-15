using System;
using Newtonsoft.Json.Linq;

namespace TinyWebSockets.Interfaces
{
    public interface ISocketService
    {
        event EventHandler<string> MessageReceived;
        event EventHandler<Exception> OnError;
        void QueueAction(string jsonMessageString);
    }
}
