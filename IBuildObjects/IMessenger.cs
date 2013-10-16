using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
