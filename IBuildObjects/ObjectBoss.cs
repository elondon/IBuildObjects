#region usings

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

#endregion

namespace IBuildObjects
{
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

        public IObjectBuilder _parent { get; }

        public ObjectBoss()
        {
            
        }

        private ObjectBoss(IObjectBuilder parent)
        {
            _parent = parent;
        }

        /// <summary>
        /// Returns a child container. When resolving types, IBuildObjects tries the child container first and bubbles up
        /// through the parent containers until it gets to the root. That means that unless you specifically define type
        /// configurations in the child, the parent's will be used. If a type configuration is defined multiple times, the child
        /// container's configuration will be used to resolve the instance.
        /// 
        /// This is for use cases where a per-request container is useful. http requests, event handlers, request handlers, etc...
        /// 
        /// Note that singletons are still kept at the app domain level. When resolving singletons, IBuildObjects will always bubble
        /// up to the root. Currently, singletons scoped at the client container are not supported.
        /// </summary>
        /// <returns>A child (per request) container.</returns>
        public IObjectBuilder GetChildContainer()
        {
            return new ObjectBoss(this);
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

                if (_parent == null)
                {
                    config.AddUsing<IObjectBuilder, ObjectBoss>().Singleton();

                    var type = typeof(IObjectBuilder);
                    var configurableType = _configuration.Keys.SingleOrDefault(x => x == type);

                    if (configurableType == null)
                        throw new IBuildObjectsException("Something went terribly wrong! IBuildObjects should add itself to the container.");

                    var configTypeForObjectBoss = _configuration[type][0];

                    if (!_singletons.ContainsKey(configTypeForObjectBoss))
                        _singletons.Add(configTypeForObjectBoss, this);
                }
                
                configuration(config);
                ValidateConfiguration();
            }
        }

        private void ValidateConfiguration()
        {
            // there aren't any validation rules on roots.
            if (_parent == null) return;

            // the only validation rule on child containers at the moment is they don't support
            // direct registration of singletons.
            var keys = _configuration.Keys;
            if (keys.Select(key => _configuration[key]).Any(config => config.Any(type => type.IsSingleton)))
                throw new IBuildObjectsException("IBuildObjects currently does not support child container scoped singletons. Singletons should be registered on the root container. All requests from child containers will bubble up and get the singleton scoped at the application level.");
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

                if (configurableType == null & _parent != null)
                    return _parent.GetInstance(type);

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
            return (T)GetInstance(typeof(T));
        }

        /// <summary>
        /// gets an instance by a key returned as an object. If the key exists for more than one type,
        /// the first matched key/type will be returned.
        /// </summary>
        /// <param name="key">the key used to retrieve the type.</param>
        /// <returns></returns>
        public object GetInstance(string key)
        {
            lock (_lock)
            {
                var configurableTypes = _configuration.Keys;
                foreach (var configType in configurableTypes)
                {
                    var type = _configuration[configType].Any(x => x.Key == key);
                    if (!type) continue;
                    var firstTypeForKey = _configuration[configType].First(x => x.Key == key);
                    return GetInstance(firstTypeForKey);
                }

                if (_parent != null)
                    return _parent.GetInstance(key);

                throw new IBuildObjectsException("No class definition was found for the key: " + key);
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
            return (T)GetInstance(key);
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
                if (configurableType == null && _parent != null)
                    return _parent.GetAllInstances(type);

                if (configurableType == null)
                    return type.IsInterface ? new List<object>() : new List<object>() { GetInstance(new StandardConfigurableType() { Type = type, Key = "" }) };
                
                var types = _configuration[type];
                var instances = types.Select(GetInstance).ToList();

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
            return GetAllInstances(typeof(T)).Cast<T>();
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
                if (configurableType == null && _parent != null)
                    return _parent.GetAllInstances<T>(key);

                if (configurableType == null)
                    return new List<T>() { (T)GetInstance(new StandardConfigurableType() { Type = typeof(T), Key = key }) };

                var types = _configuration[typeof(T)];
                var instances = types.Where(x => x.Key == key).Select(type => (T)GetInstance(type)).ToList();
                return instances;
            }
        }

        private object GetInstance(IConfigurableType type)
        {
            if (typeof(IEnumerable).IsAssignableFrom(type.Type) && type.Type.GetGenericArguments().Length > 0)
                return GetEnumerableOfType(type);

            if(type.Type.IsGenericType && type.Type.GetGenericTypeDefinition() == typeof(Func<>))
                return GetLazyFuncFactory(type);

            if (type.BoundInstance != null)
                return type.BoundInstance;

            var constructors = type.Type.GetConstructors();
    
            if (type.IsSingleton)
            {
                while (_parent != null)
                    return _parent.GetInstance(type.Type);

                if (_singletons.ContainsKey(type))
                    return _singletons[type];
            }
            object newObject;
            if (!constructors.Any())
            {
                try
                {
                    newObject = Activator.CreateInstance(type.Type);
                }
                catch (Exception e)
                {
                    var iboException = new IBuildObjectsException("IBuildObjects failed to create an instance of " + type.Type + "." +
                        " Please make sure you have registered the type properly. IBuildObjects can create concrete types using defaults if possible, but interfaces need to be registered.", e);
                    throw iboException;
                }

                if (type.IsForMessaging)
                    _messenger.Register(newObject);

                return newObject;
            }

            var constructor = constructors[0];
            if (constructors.Count() > 1)
                constructor = constructors.OrderByDescending(item => item.GetParameters().Length).First();

            var args = constructor.GetParameters();
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

            try
            {
                newObject = Activator.CreateInstance(type.Type, argumentInstances.ToArray());
            }
            catch (Exception e)
            {
                var iboException = new IBuildObjectsException("IBuildObjects failed to create an instance of " + type.Type + "." +
                    " Please make sure you have registered the type properly. IBuildObjects can create concrete types using defaults if possible, but interfaces need to be registered.", e);
                throw iboException;
            }

            if (type.IsSingleton)
                _singletons.Add(type, newObject);
            
            if (type.IsForMessaging)
                _messenger.Register(newObject);

            return newObject;
        }
        
        private static object GetCustomConstructorArgument(IConfigurableType configurableType, string argumentName)
        {
            return configurableType.Arguments.ContainsKey(argumentName) ? configurableType.Arguments[argumentName] : null;
        }

        private object GetEnumerableOfType(IConfigurableType type)
        {
            var enumOfType = type.Type.GetGenericArguments()[0];
            var listType = typeof(List<>);
            var constructedListType = listType.MakeGenericType(enumOfType);
            var listInstance = (IList)Activator.CreateInstance(constructedListType);
            if (_configuration.All(x => x.Key != enumOfType))
                return listInstance;
            var allTypeInstances = GetAllInstances(enumOfType);
            foreach (var typeInstance in allTypeInstances)
                listInstance.Add(typeInstance);
            return listInstance;
        }

        private object GetLazyFuncFactory(IConfigurableType type)
        {
            var genericArguments = type.Type.GetGenericArguments();
            var returnType = genericArguments[0];
            var resolveMethod = typeof(ObjectBoss).GetMethod("GetInstance", new Type[] { });
            resolveMethod = resolveMethod.MakeGenericMethod(returnType);
            var resolveCall = Expression.Call(Expression.Constant(this), resolveMethod);
            var resolveLambda = Expression.Lambda(resolveCall).Compile();
            return resolveLambda;
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
        /// checks to see if a key exists for 1 or more types.
        /// </summary>
        /// <param name="key">the key used to retrieve the type.</param>
        /// <returns>bool representing the existence of a definition for a given key.</returns>
        public bool Contains(string key)
        {
            lock (_lock)
            {
                var configurableTypes = _configuration.Keys;
                foreach (var configType in configurableTypes)
                {
                    var type = _configuration[configType].Any(x => x.Key == key);
                    if (type)
                        return true;
                }

                return false;
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
