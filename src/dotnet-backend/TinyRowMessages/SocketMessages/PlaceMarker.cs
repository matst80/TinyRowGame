using System;
using Newtonsoft.Json;
using TinyWebSockets;

namespace TinyRowMessages.SocketCommands
{
    [Message("place")]
    public class PlaceMarker : BaseMessage
    {
        [JsonProperty("x")]
        public int X { get; set; }

        [JsonProperty("y")]
        public int Y { get; set; }

    }

}