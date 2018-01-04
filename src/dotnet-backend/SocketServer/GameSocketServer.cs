using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using dotnetbackend.Logic;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json.Linq;
using TinyWebSockets;
using TinyWebSockets.Interfaces;

namespace dotnetbackend
{
    public class GameSocketServer : WebSocketServerService
    {
        
        public GameSocketServer(WebSocket webSocket) : base(webSocket)
        {
            
        }


    }


}
