using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using IBuildObjects;

// Release 1.0 - 10/15/2013
// All initial planned functionality to satisfy the IObjectBuilder interface.

namespace IBuildObjects
{
    /// <summary>
    /// All of the main functionality of IBuildObjects. Implements IObjectBuilder and the configuration automatically registers ObjectBoss as a singleton IObjectBuilder
    /// so it can be injected as a dependency across the application.
    /// </summary>
    /// <summary>
    /// All of the main functionality of IBuildObjects. Implements IObjectBuilder and the configuration automatically registers ObjectBoss as a singleton IObjectBuilder
    /// so it can be injected as a dependency across the application.
    /// </summary>
    public class ObjectBoss : IObjectBuilder, IDisposable
    {
        private readonly IDictionary<Type, List<IConfigurableType>> _configuration = new Dictionary<Type, List<IConfigurableType>>();
        private readonly IDictionary<Type, IConfigurableType> _defaultTypes = new Dictionary<Type, IConfigurableType>();
        private IDictionary<IConfigurableType, object> _singletons = new Dictionary<IConfigurableType, object>();
        private readonly IMessenger _messenger = new Messenger();
        private readonly object _lock = new object();

        public ObjectBoss()
        {
            
        }

        /// <summary>
        /// sends a message to any types registered ForMessaging()
        /// </summary>
        /// <param name="message">the iMessage to send. IBuildObjects automatically routes this to methods in registered objects that match the method signature via reflection.</param>
        public void SendMessage(IMessage message)
        {
            _messenger.SendMessage(message);
        }

        /// <summary>
        /// unregisters a type that was previously registered for messaging.
        /// </summary>
        /// <param name="type">the type to unregister</param>
        public void UnregisterTypeForMessaging(Type type)
        {
            _messenger.Unregister(type);
        }

        /// <summary>
        /// builds a new configuration.
        /// </summary>
        /// <param name="configuration">fluently built IConfiguration</param>
        public void Configure(Action<IConfiguration> configuration)
        {
            lock (_lock)
            {
                var config = new Configuration(_configuration, _defaultTypes);

                config.AddUsing<IObjectBuilder, ObjectBoss>().Singleton();

                var type = typeof(IObjectBuilder);
                var configurableType = _configuration.Keys.SingleOrDefault(x => x == type);

                if (configurableType == null)
                    throw new Exception("Something went terribly wrong! IBuildObjects should add itself to the container.");

                var configTypeForObjectBoss = _configuration[type][0];
                _singletons.Add(configTypeForObjectBoss, this);

                configuration(config);
            }
        }

        /// <summary>
        /// gets an instance by Type
        /// </summary>
        /// <param name="type">the Type of the instance to be retrieved.</param>
        /// <returns>instance of the object requested</returns>
        public object GetInstance(Type type)
        {
            lock (_lock)
            {
                var configurableType = _configuration.Keys.SingleOrDefault(x => x == type);
                return configurableType == null ? GetInstance(new StandardConfigurableType() { Type = type, Key = "" }) : GetInstance(_defaultTypes.ContainsKey(type) ? _defaultTypes[type] : _configuration[configurableType][0]);
            }
        }

        /// <summary>
        /// gets an instance by generic type T
        /// </summary>
        /// <typeparam name="T">the generic type to be retrieved.</typeparam>
        /// <returns>instance of the generic type T requested</returns>
        public T GetInstance<T>()
        {
            lock (_lock)
            {
                var configurableType = _configuration.Keys.SingleOrDefault(x => x == typeof(T));
                if (configurableType == null)
                    return (T)GetInstance(new StandardConfigurableType() { Type = typeof(T), Key = "" });

                if (_defaultTypes.ContainsKey(typeof(T)))
                    return (T)GetInstance(_defaultTypes[typeof(T)]);
                return (T)GetInstance(_configuration[configurableType][0]);
            }
        }

        /// <summary>
        /// gets an instance by generic type T and a key.
        /// </summary>
        /// <typeparam name="T">the generic type to be retrieved.</typeparam>
        /// <param name="key">the key used to retrieve the type</param>
        /// <returns>instance of the generic type T requested</returns>
        public T GetInstance<T>(string key)
        {
            lock (_lock)
            {
                var configurableType = _configuration.Keys.SingleOrDefault(x => x == typeof(T));
                if (configurableType == null)
                    return (T)GetInstance(new StandardConfigurableType() { Type = typeof(T), Key = "" });
                return (T)GetInstance(_configuration[configurableType].SingleOrDefault(x => x.Key == key));
            }
        }

        private object GetInstance(IConfigurableType type)
        {
            var constructors = type.Type.GetConstructors();
            if (constructors.Count() > 1)
                throw new Exception("IHandleObjects does not support multiple constructors on type " + type.Type);

            if (type.IsSingleton)
            {
                if (_singletons.ContainsKey(type))
                    return _singletons[type];
            }

            if (!constructors.Any())
            {
                var newObject = Activator.CreateInstance(type.Type);

                if (type.IsForMessaging)
                    _messenger.Register(newObject);

                return newObject;
            }
            else
            {
                var args = constructors[0].GetParameters();

                var argumentInstances = new List<object>();
                foreach (var arg in args)
                {
                    if (type.Arguments.ContainsKey(arg.Name))
                    {
                        argumentInstances.Add(GetCustomConstructorArgument(type, arg.Name));
                        continue;
                    }

                    var argtype = arg.ParameterType;
                    var getInstanceMethod = GetType().GetMethodExt("GetInstance", new Type[] { });
                    var argObject = getInstanceMethod.MakeGenericMethod(argtype).Invoke(this, null);
                    argumentInstances.Add(argObject);
                }

                var newObject = Activator.CreateInstance(type.Type, argumentInstances.ToArray());
                
                if (type.IsSingleton)
                    _singletons.Add(type, newObject);
                
                if (type.IsForMessaging)
                    _messenger.Register(newObject);

                return newObject;
            }
        }

        private object GetCustomConstructorArgument(IConfigurableType configurableType, string argumentName)
        {
            return configurableType.Arguments.ContainsKey(argumentName) ? configurableType.Arguments[argumentName] : null;
        }

        /// <summary>
        /// gets all instances by Type.
        /// </summary>
        /// <param name="type">the Type of the instance to be retrieved.</param>
        /// <returns>Enumerable of objects representing all instances of the Type to be retrieved</returns>
        public IEnumerable<object> GetAllInstances(Type type)
        {
            lock (_lock)
            {
                var configurableType = _configuration.Keys.SingleOrDefault(x => x == type);
                if (configurableType == null)
                    return new List<object>() { GetInstance(new StandardConfigurableType() { Type = type, Key = "" }) };

                var types = _configuration[type];
                var instances = types.Select(theType => GetInstance(type)).ToList();

                return instances;
            }
        }

        /// <summary>
        /// gets all instances by generic type T
        /// </summary>
        /// <typeparam name="T">the generic type to be retrieved.</typeparam>
        /// <returns>Enumerable of objects representing all instances of the generic type T</returns>
        public IEnumerable<T> GetAllInstances<T>()
        {
            lock (_lock)
            {
                var configurableType = _configuration.Keys.SingleOrDefault(x => x == typeof(T));
                if (configurableType == null)
                    return new List<T>() { (T)GetInstance(new StandardConfigurableType() { Type = typeof(T), Key = "" }) };

                var types = _configuration[typeof(T)];
                var instances = types.Select(type => (T)GetInstance(type)).ToList();

                return instances;
            }
        }

        /// <summary>
        /// gets all instances by generic type T and a key.
        /// </summary>
        /// <typeparam name="T">the generic type to be retrieved.</typeparam>
        /// <param name="key">the key used to retrieve the type</param>
        /// <returns>Enumerable of objects representing all instances of the generic type T requested</returns>
        public IEnumerable<T> GetAllInstances<T>(string key)
        {
            lock (_lock)
            {
                var configurableType = _configuration.Keys.SingleOrDefault(x => x == typeof(T));
                if (configurableType == null)
                    return new List<T>() { (T)GetInstance(new StandardConfigurableType() { Type = typeof(T), Key = key }) };

                var types = _configuration[typeof(T)];
                var instances = types.Where(x => x.Key == key).Select(type => (T)GetInstance(type)).ToList();
                return instances;
            }

        }

        /// <summary>
        /// checks to see if a generic type T exists in the configuration.
        /// </summary>
        /// <typeparam name="T">generic type T to check</typeparam>
        /// <returns>bool representing the existence of generic type T in the configuration</returns>
        public bool Contains<T>()
        {
            lock (_lock)
                return _configuration.ContainsKey(typeof(T));
        }

        /// <summary>
        /// checks to see if an implementation of a base class or interface exists in the configuration.
        /// </summary>
        /// <typeparam name="T">interface or base class</typeparam>
        /// <typeparam name="TT">implementing class</typeparam>
        /// <returns>bool representing the existence of a class that implements a base class or interface</returns>
        public bool ContainsUsing<T, TT>()
        {
            lock (_lock)
            {
                if (!_configuration.ContainsKey(typeof(T)))
                    return false;
                var configuration = _configuration[typeof(T)];
                return configuration.Any(config => config.Type == typeof(TT));
            }
        }

        /// <summary>
        /// returns the count of classes in the configuration.
        /// </summary>
        /// <returns>count of singletons</returns>
        public int GetSingletonCount()
        {
            lock (_lock)
                return _singletons.Count;
        }

        /// <summary>
        /// returns the count of classes currently registered in the configuration.
        /// </summary>
        /// <returns>count of classes registered in the config</returns>
        public int GetRegisteredClassCount()
        {
            lock (_lock)
                return _configuration.Keys.Count;
        }

        /// <summary>
        /// disposes all of the singletons when IBuildObjects gets destroyed.
        /// </summary>
        public void Dispose()
        {
            foreach (var type in _singletons)
            {
                var value = type.Value;
                if (!(value is IDisposable)) continue;
                var disposable = value as IDisposable;
                disposable.Dispose();
            }
            _singletons = null;
        }
    }
}
