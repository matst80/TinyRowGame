using Newtonsoft.Json;
using TinyWebSockets;

namespace tinyrowgame.SocketCommands
{
    [Message("winner")]
    public class Winner
    {
        [JsonProperty("winner")]
        public int WinnerUserNr { get; set; }
    }
}
