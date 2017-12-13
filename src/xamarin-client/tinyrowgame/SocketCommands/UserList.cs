using System.Collections.Generic;
using Newtonsoft.Json;
using TinyWebSockets;

namespace tinyrowgame.SocketCommands
{
    [Message("userlist")]
    public class UserList
    {
        [JsonProperty("users")]
        public IList<int> Users { get; set; }
    }
}
