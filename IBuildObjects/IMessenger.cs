using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IHandleObjects
{
    public interface IMessenger
    {
        void SendMessage(IMessage message);
        void Register(object instance);
    }
}
