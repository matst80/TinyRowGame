using System;
using TinyWebSockets;

namespace TinyWebSockets.Interfaces
{
    public interface IMessageReceiver
    {
        bool IsActive { get; }
        void SetStateService(MessageHandler stateService);
        void HandleAction(IMessage action);
    }
}
