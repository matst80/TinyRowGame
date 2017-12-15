using Newtonsoft.Json;
using TinyWebSockets;

namespace TinyRowMessages.SocketCommands
{
    public class User : BaseMessage
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("nr")]
        public int Nr { get; set; }
    }
}
