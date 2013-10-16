using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBuildObjects
{
    /// <summary>
    /// main interface used to build objects. This interface defines the contract of functionality for the entire library. 
    /// This is the API accessible to the outside and can be referred to for all the features currently built into IBuildObjects.
    /// </summary>
    public interface IObjectBuilder
    {
        void Configure(Action<IConfiguration> configuration);
        int GetSingletonCount();

        T GetInstance<T>();
        T GetInstance<T>(string key);
        object GetInstance(Type type);

        IEnumerable<T> GetAllInstances<T>();
        IEnumerable<T> GetAllInstances<T>(string key);
        IEnumerable<object> GetAllInstances(Type type);

        void SendMessage(IMessage message);
    }
}
