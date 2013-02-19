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
        private readonly Dictionary<Type, List<Type>> Configuration = new Dictionary<Type, List<Type>>();
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
            if (Configuration.ContainsKey(typeof (T)))
            {
                Configuration[typeof (T)].Add(typeof (T));
            }
            else
            {
                Configuration.Add(typeof (T), new List<Type>() {typeof (T)});
            }
        }

        public void AddUsing<T, TT>()
        {
            if (Configuration.ContainsKey(typeof (T)))
            {
                Configuration[typeof (T)].Add(typeof (TT));
            }
            else
            {
                Configuration.Add(typeof (T), new List<Type>() {typeof (TT)});
            }
        }

        public void AddUsingDefaultType<T, TT>()
        {

        }

        public bool Contains<T>()
        {
            return Configuration.ContainsKey(typeof (T));
        }

        public bool ContainsUsing<T, TT>()
        {
            if (!Configuration.ContainsKey(typeof (T)))
                return false;
            var configuration = Configuration[typeof (T)];
            return configuration.Contains(typeof (TT));
        }

        public void Add<T>(string key)
        {

        }

        public int GetObjectCount()
        {
            return Objects.Count;
        }

        public T GetInstance<T>()
        {
            if (!Configuration.ContainsKey(typeof (T)))
                throw new Exception("IHandleObjects does not contain a configuration for the type " + typeof (T));

            var type = typeof (T);
            var constructors = type.GetConstructors();
            if (constructors.Count() > 1)
                throw new Exception("IHandleObjects does not support multiple constructors on type " + typeof (T));

            if (!constructors.Any())
            {
                var newObject = Activator.CreateInstance<T>();
                return newObject;
            }
            else
            {
                var args = constructors[0].GetParameters();

                var argumentInstances = new List<object>();
                foreach (var arg in args)
                {
                    var argtype = arg.ParameterType;
                    var getInstanceMethod = GetType().GetMethodExt("GetInstance", new Type[] {});
                    var argObject = getInstanceMethod.MakeGenericMethod(argtype).Invoke(this, null);
                    argumentInstances.Add(argObject);
                }

                var newObject = Activator.CreateInstance(type, argumentInstances.ToArray());
                return (T) newObject;
            }
        }

        public T GetInstance<T>(string key)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<T> GetAllInstances<T>()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<T> GetAllInstances<T>(string key)
        {
            throw new NotImplementedException();
        }

        private bool ContainsObjectsFor<T>()
        {
            return Objects.ContainsKey(typeof (T)) && Objects[typeof (T)].Count != 0;
        }
    }
}
