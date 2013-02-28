using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IHandleObjects
{
    public class Messenger : IMessenger
    {
        private readonly IList<object> _instances = new List<object>(); 

        public void Register(object instance)
        {
            _instances.Add(instance);
        }

        public void SendMessage(IMessage message)
        {
            foreach(var instance in _instances)
            {
                var type = instance.GetType();
                foreach(var method in type.GetMethods())
                {
                    var parameters = method.GetParameters();
                    if (parameters.Count() != 1) continue;
                    if (parameters[0].ParameterType == message.GetType())
                        method.Invoke(instance, new object[] {message});
                }
            }
        }
    }
}
