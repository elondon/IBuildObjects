using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using IHandleObjects;

namespace HandleObjects
{
    public class ObjectBoss : IHandleObjects
    {
        private readonly IDictionary<Type, List<ConfigurableType>> Configuration = new Dictionary<Type, List<ConfigurableType>>();
        private readonly IDictionary<ConfigurableType, List<object>> Objects = new Dictionary<ConfigurableType, List<object>>();
        private readonly IDictionary<Type, ConfigurableType> DefaultTypes = new Dictionary<Type, ConfigurableType>();

        public ObjectBoss()
        {
            Objects.Add(new ConfigurableType() { Type = typeof(ObjectBoss), Key = "ObjectBoss" }, new List<object>() { this });
        }

        public void Configure(Action<IConfiguration> configuration)
        {
            var config = new Configuration(Configuration, DefaultTypes);
            configuration(config);
        }

        public T GetInstance<T>()
        {
            var configurableType = Configuration.Keys.SingleOrDefault(x => x == typeof (T));
            if (configurableType == null)
                return (T) GetInstance(new ConfigurableType() {Type = typeof (T), Key = ""});

            if (DefaultTypes.ContainsKey(typeof(T)))
                return (T)GetInstance(DefaultTypes[typeof (T)]);
            return (T)GetInstance(Configuration[configurableType][0]);
        }

        public T GetInstance<T>(string key)
        {
            var configurableType = Configuration.Keys.SingleOrDefault(x => x == typeof(T));
            if (configurableType == null)
                return (T)GetInstance(new ConfigurableType() { Type = typeof(T), Key = "" });
            return (T)GetInstance(Configuration[configurableType].SingleOrDefault(x => x.Key == key));
        }

        private object GetInstance(ConfigurableType type)
        {
            var constructors = type.Type.GetConstructors();
            if (constructors.Count() > 1)
                throw new Exception("IHandleObjects does not support multiple constructors on type " +type.Type);

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
                return newObject;
            }
        }

        public IEnumerable<T> GetAllInstances<T>()
        {
            var configurableType = Configuration.Keys.SingleOrDefault(x => x == typeof(T));
            if (configurableType == null)
                return new List<T>() { (T)GetInstance(new ConfigurableType() { Type = typeof(T), Key = ""  })};

            var types = Configuration[typeof (T)];
            var instances = types.Select(type => (T) GetInstance(type)).ToList();

            return instances;
        }

        public IEnumerable<T> GetAllInstances<T>(string key)
        {
            var configurableType = Configuration.Keys.SingleOrDefault(x => x == typeof(T));
            if (configurableType == null)
                return new List<T>() { (T)GetInstance(new ConfigurableType() { Type = typeof(T), Key = key }) };

            var types = Configuration[typeof(T)];
            var instances = types.Where(x => x.Key == key).Select(type => (T)GetInstance(type)).ToList();
            return instances;
        }

        public bool Contains<T>()
        {
            return Configuration.ContainsKey(typeof(T));
        }

        public bool ContainsUsing<T, TT>()
        {
            if (!Configuration.ContainsKey(typeof(T)))
                return false;
            var configuration = Configuration[typeof(T)];
            return configuration.Exists(x => x.Type == typeof(TT));
        }

        public int GetObjectCount()
        {
            return Objects.Count;
        }
    }
}
