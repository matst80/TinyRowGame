using System;
using System.Collections.Generic;
using System.Linq;
using TinyWebSockets.Interfaces;
using Newtonsoft.Json.Linq;
using System.Reflection;

namespace TinyWebSockets
{
    public abstract class BaseMessage : IMessage
    {
        public string Type { get; set; }
    }

    /// <summary>
    /// Keeps track of all state in the application. There should be only one!
    /// </summary>
    public class MessageHandler
    {
        private readonly WebSocketService webSocketService;
		private readonly List<IMessageReceiver> _receivers = new List<IMessageReceiver>();

        private const string TypePropertyName = "type";

        public event EventHandler<Exception> OnError;
        public Dictionary<string, Type> ActionTypes { get; private set; }

        public MessageHandler(WebSocketService webSocketService)
        {
            this.webSocketService = webSocketService;

            this.webSocketService.OnError += (sender, e) => OnError?.Invoke(sender, e);
            this.webSocketService.MessageRecieved += WebSocketService_MessageRecieved;
            PopulateActions();
        }

        public void RegisterActionReceiver(IMessageReceiver receiver) 
        {
            _receivers.Add(receiver);
            receiver.SetStateService(this);
        }

        public Type GetTypeFromJObject(JToken obj) 
        {
            var type = obj?.Value<String>();
            Type ret = null;

            if (type==null || ActionTypes.ContainsKey(type)) 
            {
                ret = ActionTypes[type];
            }

            return ret;
        }

        public void SendAsync(IMessage obj)
        {
            webSocketService.QueueAction(obj);
        }

        /// <summary>
        /// Message received over websockets and converted to IAction here
        /// </summary>
        /// <param name="message">Message.</param>
        private void WebSocketService_MessageRecieved(JToken message)
        {
            // find the node
            var returnType = GetTypeFromJObject(message[TypePropertyName]);
            var action = message.ToObject(returnType) as IMessage;

            // update it
            if (action != null) 
            {
                SendActionToInternalReceivers(action);
            }
        }

        /// <summary>
        /// Send actions to registerd receivers, possibility to exclude some
        /// </summary>
        /// <param name="action">Action.</param>
        /// <param name="excluded">Excluded.</param>
        public void SendActionToInternalReceivers(IMessage action, params IMessageReceiver[] excluded)
        {
            foreach(var rec in _receivers) 
            {
                if (rec != null && rec.IsActive && !excluded.Contains(rec)) 
                {
                    rec.HandleAction(action);
                }
            }
        }

        /// <summary>
        /// Scans this assembly for types that are decorated with ActionAttribute
        /// and stores the result in a lookup of typename and type.
        /// </summary>
        public void PopulateActions(object parent = null)
        {
            var ass = this.GetType().Assembly;
            if (parent == null)
                ass = Assembly.GetCallingAssembly();
            var interfaceType = typeof(IMessage);
            ActionTypes = new Dictionary<string, Type>();
            var types = this.GetType().Assembly.GetExportedTypes();

            foreach (var t in types)
            {
                if (interfaceType.IsAssignableFrom(t))
                {
                    var attr = t.GetCustomAttributes(typeof(MessageAttribute), true).OfType<MessageAttribute>().FirstOrDefault();
                    if (attr != null)
                    {
                        if (!ActionTypes.ContainsKey(attr.TypeName))
                            ActionTypes.Add(attr.TypeName, t);
                    }
                }
            }
        }
    }
}
