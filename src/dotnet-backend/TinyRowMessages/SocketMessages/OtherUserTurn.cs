using Newtonsoft.Json;
using TinyWebSockets;

namespace TinyRowMessages.SocketCommands
{
    [Message("userchange")]
    public class OtherUserTurn : BaseMessage
    {
        [JsonProperty("userNr")]
        public int UserNr { get; set; }
    }
}
