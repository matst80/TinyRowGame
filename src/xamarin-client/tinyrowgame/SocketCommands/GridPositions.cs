using System.Collections.Generic;
using Newtonsoft.Json;
using TinyWebSockets;

namespace tinyrowgame.SocketCommands
{
    [Message("grid")]
    public class GridPositions : BaseMessage
    {
        [JsonProperty("grid")]
        public IList<Pos> Grid { get; set; }
    }
}
