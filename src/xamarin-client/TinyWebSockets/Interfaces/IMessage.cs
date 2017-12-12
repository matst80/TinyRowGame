using System;

namespace TinyWebSockets.Interfaces
{
    /// <summary>
    /// Base Action that will be used for all events that will be used to replicate throughout the remote nodes
    /// </summary>
    public interface IAction
    {
        string Type { get; }

    }
}
