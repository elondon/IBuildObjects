using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBuildObjects
{
    public interface IObjectBuilder
    {
        void Configure(Action<IConfiguration> configuration);
        int GetObjectCount();
        T GetInstance<T>();
        T GetInstance<T>(string key);
        object GetInstance(Type type);
        IEnumerable<T> GetAllInstances<T>();
        IEnumerable<T> GetAllInstances<T>(string key);
        IEnumerable<object> GetAllInstances(Type type);
    }
}
