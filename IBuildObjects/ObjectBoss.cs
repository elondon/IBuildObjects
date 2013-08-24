using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using IBuildObjects;

namespace IBuildObjects
{
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

        public void SendMessage(Message message)
        {
            _messenger.SendMessage(message);
        }

        public void Configure(Action<IConfiguration> configuration)
        {
            lock (_lock)
            {
                var config = new Configuration(_configuration, _defaultTypes);
                config.AddUsing<IObjectBuilder, ObjectBoss>().Singleton();
                
                var type = typeof (IObjectBuilder);
                var configurableType = _configuration.Keys.SingleOrDefault(x => x == type);

                if(configurableType == null)
                        throw new Exception("Something went terribly wrong! IBuildObjects should add itself to the container.");
                
                var configTypeForObjectBoss = _configuration[type][0];
                _singletons.Add(configTypeForObjectBoss, this);

                configuration(config);
            }
        }

        public object GetInstance(Type type)
        {
            lock (_lock)
            {
                var configurableType = _configuration.Keys.SingleOrDefault(x => x == type);
                return configurableType == null ? GetInstance(new StandardConfigurableType() { Type = type, Key = "" }) : GetInstance(_defaultTypes.ContainsKey(type) ? _defaultTypes[type] : _configuration[configurableType][0]);
            }
        }

        public T GetInstance<T>()
        {
            lock(_lock)
            {
                var configurableType = _configuration.Keys.SingleOrDefault(x => x == typeof(T));
                if (configurableType == null)
                    return (T)GetInstance(new StandardConfigurableType() { Type = typeof(T), Key = "" });

                if (_defaultTypes.ContainsKey(typeof(T)))
                    return (T)GetInstance(_defaultTypes[typeof(T)]);
                return (T)GetInstance(_configuration[configurableType][0]);    
            }
        }

        public T GetInstance<T>(string key)
        {
            lock(_lock)
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
                throw new Exception("IHandleObjects does not support multiple constructors on type " +type.Type);

            if(type.IsSingleton && _singletons.ContainsKey(type))
                return _singletons[type];
            

            if (!constructors.Any())
            {
                object newObject;
                try
                {
                    newObject = Activator.CreateInstance(type.Type);
                }
                catch (Exception err)
                {
                    throw new Exception("Could not instanciate type " + type.Type + ". Please make sure this type is correctly registered.", err);
                }
                
                if (type.IsForMessaging)
                    _messenger.Register(newObject);

                return newObject;
            }
            else
            {
                var args = constructors[0].GetParameters();

                var newObject = Activator.CreateInstance(type.Type, (from arg in args select arg.ParameterType into argtype let getInstanceMethod = GetType().GetMethodExt("GetInstance", new Type[] {}) select getInstanceMethod.MakeGenericMethod(argtype).Invoke(this, null)).ToArray());
                
                if(type.IsSingleton)
                    _singletons.Add(type, newObject);
                
                if(type.IsForMessaging)
                    _messenger.Register(newObject);

                return newObject;
            }
        }

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

        public IEnumerable<T> GetAllInstances<T>()
        {
            lock(_lock)
            {
                var configurableType = _configuration.Keys.SingleOrDefault(x => x == typeof(T));
                if (configurableType == null)
                    return new List<T>() { (T)GetInstance(new StandardConfigurableType() { Type = typeof(T), Key = "" }) };

                var types = _configuration[typeof(T)];
                var instances = types.Select(type => (T)GetInstance(type)).ToList();

                return instances;    
            }
        }

        public IEnumerable<T> GetAllInstances<T>(string key)
        {
            lock(_lock)
            {
                var configurableType = _configuration.Keys.SingleOrDefault(x => x == typeof(T));
                if (configurableType == null)
                    return new List<T>() { (T)GetInstance(new StandardConfigurableType() { Type = typeof(T), Key = key }) };

                var types = _configuration[typeof(T)];
                var instances = types.Where(x => x.Key == key).Select(type => (T)GetInstance(type)).ToList();
                return instances;    
            }
            
        }

        public bool Contains<T>()
        {
            lock(_lock)
                return _configuration.ContainsKey(typeof(T));
        }

        public bool ContainsUsing<T, TT>()
        {
            lock(_lock)
            {
                if (!_configuration.ContainsKey(typeof(T)))
                    return false;
                var configuration = _configuration[typeof(T)];
                return configuration.Any(config => config.Type == typeof (TT));
            }
        }

        public int GetObjectCount()
        {
            lock(_lock)
                return _singletons.Count;
        }

        public void Dispose()
        {
            foreach(var type in _singletons)
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
