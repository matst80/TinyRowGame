using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
//using Appalizer.Attributes;
//using Appalizer.Core.Actions;
using TinyWebSockets.Interfaces;
using Newtonsoft.Json.Linq;
//using TinyLog;

namespace TinyWebSockets.Core.Services
{
    /// <summary>
    /// Keeps track of all state in the application. There should be only one!
    /// </summary>
    public class StateService
    {
        private readonly WebSocketService webSocketService;
		private readonly List<IActionReceiver> _receivers = new List<IActionReceiver>();

        public event EventHandler<Exception> OnError;
        public Dictionary<string, Type> ActionTypes { get; private set; }

        public StateService(WebSocketService webSocketService)
        {
            Logger.Instance.LogVerbose("StateService initalizing");

            this.webSocketService = webSocketService;
            this.webSocketService.OnError += (sender, e) => OnError?.Invoke(sender, e);
            this.webSocketService.MessageRecieved += WebSocketService_MessageRecieved;
            PopulateActions();

            Logger.Instance.LogVerbose("StateService initialized"); 
        }

        public void RegisterActionReceiver(IActionReceiver receiver) 
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
            else
            {
                Logger.Instance.LogWarning($"No action type for {obj.ToString()}"); 
            }

            return ret;
        }

        public void Pause() => webSocketService.Pause();

        public void Resume() => ConnectAsync();

        public async Task ConnectAsync()
        {
            try
            {
                var uri = "ws://fw.knatofs.se:8001";
                Logger.Instance.LogVerbose($"Connecting to {uri}");
                await webSocketService.StartListening(new Uri(uri)); // TODO Setting?
                Task.Run(async () => await webSocketService.StartReceivingMessages());
            }
            catch (Exception ex)
            {
                Logger.Instance.Log(ex);
                OnError?.Invoke(this, ex);
            }
        }

        public void SendAsync(IAction obj)
        {
            Logger.Instance.LogVerboseEnterMethod();
            webSocketService.QueueAction(obj);
        }

        /// <summary>
        /// Message received over websockets and converted to IAction here
        /// </summary>
        /// <param name="message">Message.</param>
        private void WebSocketService_MessageRecieved(JToken message)
        {
            // find the node
            var returnType = GetTypeFromJObject(message["Type"]);
            var action = message.ToObject(returnType) as IAction;

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
        public void SendActionToInternalReceivers(IAction action, params IActionReceiver[] excluded)
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
        private void PopulateActions()
        {
            var interfaceType = typeof(IAction);
            ActionTypes = new Dictionary<string, Type>();
            var types = this.GetType().Assembly.GetExportedTypes();

            foreach (var t in types)
            {
                if (interfaceType.IsAssignableFrom(t))
                {
                    var attr = t.GetCustomAttributes(typeof(ActionAttribute), true).OfType<ActionAttribute>().FirstOrDefault();
                    if (attr != null)
                    {
                        ActionTypes.Add(attr.TypeName, t);
                    }
                }
            }
        }
    }
}
