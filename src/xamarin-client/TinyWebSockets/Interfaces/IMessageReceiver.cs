using System;
using TinyWebSockets.Core.Services;

namespace TinyWebSockets.Interfaces
{
    public interface IActionReceiver
    {
        bool IsActive { get; }
        void SetStateService(MessageHandler stateService);
        void HandleAction(IAction action);
    }
}
