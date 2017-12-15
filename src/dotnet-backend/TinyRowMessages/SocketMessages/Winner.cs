using Newtonsoft.Json;
using TinyWebSockets;

namespace TinyRowMessages.SocketCommands
{
    [Message("winner")]
    public class Winner : BaseMessage
    {
        [JsonProperty("winner")]
        public int WinnerUserNr { get; set; }
    }
}
