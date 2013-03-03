using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBuildObjects
{
    public interface IMessenger
    {
        void SendMessage(IMessage message);
        void Register(object instance);
    }
}
