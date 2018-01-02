using TinyWebSockets.Interfaces;

namespace TinyWebSockets
{
    public abstract class BaseMessage : IMessage
    {
        [Newtonsoft.Json.JsonProperty("type")]
        public string Type { get; set; }
    }
}
