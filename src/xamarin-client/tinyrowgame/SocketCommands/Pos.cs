using Newtonsoft.Json;

namespace tinyrowgame.SocketCommands
{
    public class Pos
    {
        [JsonProperty("x")]
        public int X { get; set; }

        [JsonProperty("y")]
        public int Y { get; set; }

        [JsonProperty("value")]
        public int Value { get; set; }

    }
}
