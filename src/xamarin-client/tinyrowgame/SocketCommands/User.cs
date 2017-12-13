using Newtonsoft.Json;

namespace tinyrowgame.SocketCommands
{
    public class User
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("nr")]
        public int Nr { get; set; }
    }
}
