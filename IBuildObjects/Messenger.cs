#region usings

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

#endregion

namespace IBuildObjects
{
    /// <summary>
    /// Default Messenger implementation. Allows for messaging between objects registered in IBuildObjects "for messaging."
    /// Similar to the Event Aggregator pattern, except built into the tool and there is no need to manually register each 
    /// instance and there is no need for any kind of IHandle interface.
    /// 
    /// Currently, you register an instance but you're really registering the type of that instance. That is why Unregister
    /// takes a type. In the future, I would like instance-level messaging so that you can send messages to individual instances
    /// of a given type instead of to all instances of that type.
    /// </summary>
    public class Messenger : IMessenger
    {
        private readonly List<MessageHandler> _instances = new List<MessageHandler>();

        private readonly object _lock = new object();

        /// <summary>
        /// registers an object for messaging.
        /// </summary>
        /// <param name="instance">the instance to register</param>
        public void Register(object instance)
        {
            _instances.Add(new MessageHandler(instance));
        }

        /// <summary>
        /// unregisters a type so that it no longer receives messages.
        /// </summary>
        /// <param name="type">the type to unregister</param>
        public void Unregister(Type type)
        {
            lock (_lock)
            {
                foreach (var instance in _instances.Where(instance => instance.Reference.Target.GetType() == type))
                {
                    var messageHandler = instance;
                    messageHandler.Kill();
                }
            }
        }

        /// <summary>
        /// sends a message to objects in the container registered for messaging. The registered class must have a method that takes an IMessage
        /// as a parameter. IBuildObjects will reflectively look for a method that handles that type of message and call it passing the message
        /// as the parameter.
        /// </summary>
        /// <param name="message">the message to send. Can be any class that implements IMessage and will automatically go to objects that have a method that take that class
        /// as a parameter.</param>
        public void SendMessage(IMessage message)
        {
            lock (_lock)
            {
                var configuredMessage = message as IConfigureMessage;
                if (configuredMessage == null) throw new Exception("Message must implement base class Message");

                RemoveDeadHandlers();

                foreach (var instance in _instances)
                {
                    var messageHandler = instance;
                    var messageObject = messageHandler.Reference.Target;
                    if (messageObject == null) continue;

                    var type = messageObject.GetType();
                    foreach (var method in type.GetMethods())
                    {
                        var parameters = method.GetParameters();
                        if (parameters.Count() != 1) continue;
                        if (parameters[0].ParameterType != message.GetType()) continue;
                        if (configuredMessage.RunOnDefault)
                            method.Invoke(messageObject, new object[] { message });
                        if (configuredMessage.RunInBackground)
                        {
                            RunInBackground(method, messageObject, message);
                        }
                    }
                }
            }
        }

        private void RemoveDeadHandlers()
        {
            _instances.RemoveAll(x => x.IsDead());
        }

        private static void RunInBackground(MethodInfo info, object instance, IMessage message)
        {
            Task.Factory.StartNew(() =>
            {
                info.Invoke(instance, new object[] { message });
            });
        }

    }

    /// <summary>
    /// wrapper for a weak reference to the registered instance. This way, the client code still controls the scope and the registered instance can be
    /// garbage collected. If it's garbage collected, the handler that contains the weak reference to it will be removed from the list of handlers.
    /// </summary>
    public class MessageHandler
    {
        public WeakReference Reference { get; protected set; }

        public MessageHandler(object handler)
        {
            Reference = new WeakReference(handler);
        }

        /// <summary>
        /// a way to unsubscribe. This kills the message handler - sets the target of the weak reference to null manually so
        /// it is removed from the list of handlers without destroying the registered instance itself.
        /// </summary>
        public void Kill()
        {
            Reference.Target = null;
        }

        /// <summary>
        /// checks to see if the weak reference is still holding a reference to its object.
        /// </summary>
        /// <returns></returns>
        public bool IsDead()
        {
            return Reference.Target == null;
        }
    }
}
