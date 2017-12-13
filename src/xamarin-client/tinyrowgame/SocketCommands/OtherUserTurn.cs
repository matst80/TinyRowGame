using Newtonsoft.Json;
using TinyWebSockets;

namespace tinyrowgame.SocketCommands
{
    [Message("userchange")]
    public class OtherUserTurn
    {
        [JsonProperty("userNr")]
        public int UserNr { get; set; }
    }
}
