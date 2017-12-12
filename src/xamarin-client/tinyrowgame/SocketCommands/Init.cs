using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using TinyWebSockets;
using TinyWebSockets.Interfaces;

namespace tinyrowgame.SocketCommands
{

    [Message("init")]
    public class Init : BaseMessage
    {
        [JsonProperty("user")]
        public User UserData { get; set; }

        public IList<Pos> Points { get; set; }
    }
}
