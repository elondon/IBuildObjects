using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace HandleObjects
{
    public class ObjectBoss : IHandleObjects
    {
        private readonly Dictionary<Type, List<ConfigurableType>> Configuration = new Dictionary<Type, List<ConfigurableType>>();
        private readonly Dictionary<Type, List<object>> Objects = new Dictionary<Type, List<object>>();
        private readonly Dictionary<Type, Type> DefaultTypes = new Dictionary<Type, Type>();

        public ObjectBoss()
        {
            AddUsing<IHandleObjects, ObjectBoss>();
            DefaultTypes.Add(typeof (IHandleObjects), typeof (ObjectBoss));
            Objects.Add(typeof (IHandleObjects), new List<object>() {this});
        }

        public void Add<T>()
        {
            var configurableType = new ConfigurableType() {Type = typeof (T), Key = ""};
            AddToConfiguration(typeof(T), configurableType);
        }

        public void Add<T>(string key)
        {
            var configurableType = new ConfigurableType() { Type = typeof(T), Key = key };
            AddToConfiguration(typeof(T), configurableType);
        }

        public void AddUsing<T, TT>()
        {
            var configurableType = new ConfigurableType() { Type = typeof(TT), Key = "" };
            AddToConfiguration(typeof(T), configurableType);
        }

        private void AddToConfiguration(Type usingType, ConfigurableType configureType)
        {
            if (Configuration.ContainsKey(usingType))
            {
                Configuration[usingType].Add(configureType);
            }
            else
            {
                Configuration.Add(usingType, new List<ConfigurableType>() { configureType });
            }
        }

        public void AddUsingDefaultType<T, TT>()
        {
            var configurableType = new ConfigurableType() { Type = typeof(TT), Key = "" };
            AddToConfiguration(typeof(T), configurableType);
            DefaultTypes.Add(typeof(T), typeof(TT));
        }

        public T GetInstance<T>()
        {
            var configurableType = Configuration.Keys.SingleOrDefault(x => x == typeof (T));
            if (configurableType == null)
                return (T) GetInstance(new ConfigurableType() {Type = typeof (T), Key = ""});
            return (T)GetInstance(Configuration[configurableType][0]);
        }

        public T GetInstance<T>(string key)
        {
            return (T) new object();
        }

        private object GetInstance(ConfigurableType type)
        {
            if (!Configuration.ContainsKey(type.Type))
                throw new Exception("IHandleObjects does not contain a configuration for the type " + type.Type);

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
            throw new NotImplementedException();
        }

        public IEnumerable<T> GetAllInstances<T>(string key)
        {
            throw new NotImplementedException();
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

        private bool ContainsObjectsFor<T>()
        {
            return Objects.ContainsKey(typeof (T)) && Objects[typeof (T)].Count != 0;
        }
    }
}
