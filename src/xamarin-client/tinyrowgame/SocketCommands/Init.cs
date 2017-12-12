using System;
using TinyWebSockets;
using TinyWebSockets.Interfaces;

namespace tinyrowgame.SocketCommands
{
    [Message("init")]
    public class Init : BaseMessage
    {
        public Init()
        {
        }


    }
}
