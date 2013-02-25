using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace IBuildObjects
{
    public class ObjectBoss : IObjectBuilder, IDisposable
    {
        private readonly IDictionary<Type, List<IConfigurableType>> Configuration = new Dictionary<Type, List<IConfigurableType>>();
        private readonly IDictionary<Type, IConfigurableType> DefaultTypes = new Dictionary<Type, IConfigurableType>();
        private IDictionary<IConfigurableType, object> Objects = new Dictionary<IConfigurableType, object>();

        private readonly object _lock = new object();

        public ObjectBoss()
        {
            
        }

        public void Configure(Action<IConfiguration> configuration)
        {
            lock (_lock)
            {
                var config = new Configuration(Configuration, DefaultTypes);
                configuration(config);    
            }
        }

        public T GetInstance<T>()
        {
            lock(_lock)
            {
                var configurableType = Configuration.Keys.SingleOrDefault(x => x == typeof(T));
                if (configurableType == null)
                    return (T)GetInstance(new StandardConfigurableType() { Type = typeof(T), Key = "" });

                if (DefaultTypes.ContainsKey(typeof(T)))
                    return (T)GetInstance(DefaultTypes[typeof(T)]);
                return (T)GetInstance(Configuration[configurableType][0]);    
            }
        }

        public T GetInstance<T>(string key)
        {
            lock(_lock)
            {
                var configurableType = Configuration.Keys.SingleOrDefault(x => x == typeof(T));
                if (configurableType == null)
                    return (T)GetInstance(new StandardConfigurableType() { Type = typeof(T), Key = "" });
                return (T)GetInstance(Configuration[configurableType].SingleOrDefault(x => x.Key == key));    
            }
        }

        private object GetInstance(IConfigurableType type)
        {
            var constructors = type.Type.GetConstructors();
            if (constructors.Count() > 1)
                throw new Exception("IHandleObjects does not support multiple constructors on type " +type.Type);

            if(type.IsSingleton)
            {
                if (Objects.ContainsKey(type))
                    return Objects[type];
            }

            if (!constructors.Any())
            {
                var newObject = Activator.CreateInstance(type.Type);
                return newObject;
            }
            else
            {
                var args = constructors[0].GetParameters();

                var argumentInstances = new List<object>();
                foreach (var arg in args)
                {
                    var argtype = arg.ParameterType;
                    var getInstanceMethod = GetType().GetMethodExt("GetInstance", new Type[] { });
                    var argObject = getInstanceMethod.MakeGenericMethod(argtype).Invoke(this, null);
                    argumentInstances.Add(argObject);
                }

                var newObject = Activator.CreateInstance(type.Type, argumentInstances.ToArray());
                if(type.IsSingleton)
                {
                    Objects.Add(type, newObject);
                }
                return newObject;
            }
        }

        public IEnumerable<T> GetAllInstances<T>()
        {
            lock(_lock)
            {
                var configurableType = Configuration.Keys.SingleOrDefault(x => x == typeof(T));
                if (configurableType == null)
                    return new List<T>() { (T)GetInstance(new StandardConfigurableType() { Type = typeof(T), Key = "" }) };

                var types = Configuration[typeof(T)];
                var instances = types.Select(type => (T)GetInstance(type)).ToList();

                return instances;    
            }
        }

        public IEnumerable<T> GetAllInstances<T>(string key)
        {
            lock(_lock)
            {
                var configurableType = Configuration.Keys.SingleOrDefault(x => x == typeof(T));
                if (configurableType == null)
                    return new List<T>() { (T)GetInstance(new StandardConfigurableType() { Type = typeof(T), Key = key }) };

                var types = Configuration[typeof(T)];
                var instances = types.Where(x => x.Key == key).Select(type => (T)GetInstance(type)).ToList();
                return instances;    
            }
            
        }

        public bool Contains<T>()
        {
            lock(_lock)
                return Configuration.ContainsKey(typeof(T));
        }

        public bool ContainsUsing<T, TT>()
        {
            lock(_lock)
            {
                if (!Configuration.ContainsKey(typeof(T)))
                    return false;
                var configuration = Configuration[typeof(T)];
                return configuration.Exists(x => x.Type == typeof(TT));    
            }
        }

        public int GetObjectCount()
        {
            lock(_lock)
                return Objects.Count;
        }

        public void Dispose()
        {
            foreach(var type in Objects)
            {
                var value = type.Value;
                if (!(value is IDisposable)) continue;
                var disposable = value as IDisposable;
                disposable.Dispose();
            }
            Objects = null;
        }
    }
}
