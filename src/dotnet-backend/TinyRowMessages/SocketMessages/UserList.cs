using System.Collections.Generic;
using Newtonsoft.Json;
using TinyWebSockets;

namespace TinyRowMessages.SocketCommands
{
    [Message("userlist")]
    public class UserList : BaseMessage
    {
        [JsonProperty("users")]
        public IList<int> Users { get; set; }
    }
}
