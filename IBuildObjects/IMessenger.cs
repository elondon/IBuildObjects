#region usings

using System;

#endregion

namespace IBuildObjects
{
    /// <summary>
    /// defines the messaging interface for sending messages to objects registered for messaging in the object boss.
    /// </summary>
    public interface IMessenger
    {
        void SendMessage(IMessage message);
        void Register(object instance);
        void Unregister(Type type);
    }
}
