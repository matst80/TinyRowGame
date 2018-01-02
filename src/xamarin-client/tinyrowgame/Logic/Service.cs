using System;
using System.Threading.Tasks;
using TinyWebSockets;

namespace tinyrowgame
{
    public static class Service
    {
        public static WebSocketClientService _socketService;
        public static MessageHandler _messageHandler;

        public static WebSocketClientService SocketService
        {
            get
            {
                if (_socketService == null)
                {
                    _socketService = new WebSocketClientService();
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
