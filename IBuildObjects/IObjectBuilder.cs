#region usings

using System;
using System.Collections.Generic;

#endregion

namespace IBuildObjects
{
    /// <summary>
    /// main interface used to build objects. This interface defines the contract of functionality for the entire library. 
    /// This is the API accessible to the outside and can be referred to for all the features currently built into IBuildObjects.
    /// </summary>
    public interface IObjectBuilder
    {
        IObjectBuilder GetChildContainer();

        void Configure(Action<IConfiguration> configuration);
        int GetSingletonCount();
        int GetRegisteredClassCount();

        T GetInstance<T>();
        T GetInstance<T>(string key);
        object GetInstance(Type type);
        object GetInstance(string key, Type type);

        IEnumerable<T> GetAllInstances<T>();
        IEnumerable<T> GetAllInstances<T>(string key);
        IEnumerable<object> GetAllInstances(Type type);
      
        bool Contains<T>();
        bool ContainsUsing<T, TT>();
        bool Contains(string key);
        
        void SendMessage(IMessage message);
    }
}
