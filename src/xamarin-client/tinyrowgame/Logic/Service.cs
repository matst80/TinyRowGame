using System;
using System.Threading.Tasks;
using TinyWebSockets;

namespace tinyrowgame
{
    public static class Service
    {
        public static WebSocketService _socketService;
        public static MessageHandler _messageHandler;

        private static Uri serverUrl = new Uri("ws://fw.knatofs.se:8001");

        public static WebSocketService SocketService
        {
            get
            {
                if (_socketService == null)
                {
                    _socketService = new WebSocketService();
                    Task.Run(async () => await _socketService.StartListening(serverUrl));
                }
                return _socketService;
            }
        }

        public static MessageHandler MessageHandler 
        { 
            get 
            { 
                if (_messageHandler==null) {
                    _messageHandler = new MessageHandler(SocketService);
                    //Task.Run(async () => await _socketService.StartReceivingMessages());
                }
                return _messageHandler;
            } 
        }
    }
}
