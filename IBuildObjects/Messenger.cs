using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
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
            var configuredMessage = message as IConfigureMessage;
            if(configuredMessage == null) throw new Exception("Message must implement base class Message");
            foreach(var instance in _instances)
            {
                var type = instance.GetType();
                foreach(var method in type.GetMethods())
                {
                    var parameters = method.GetParameters();
                    if (parameters.Count() != 1) continue;
                    if (parameters[0].ParameterType != message.GetType()) continue;
                    if(configuredMessage.RunOnDefault)
                        method.Invoke(instance, new object[] { message });
                    if (configuredMessage.RunInBackground)
                    {
                        RunInBackground(method, instance, message);
                    }
                }
            }
        }

        private static void RunInBackground(MethodInfo info, object instance, IMessage message)
        {
            Task.Factory.StartNew(() =>
            {
                info.Invoke(instance, new object[] { message });
            });
        }
       
    }
}
