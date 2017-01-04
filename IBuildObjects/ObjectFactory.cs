using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IBuildObjects
{
    /// <summary>
    /// Wrapper to support static initialization. For some use cases, use of IBuildObjects in a static way is useful.
    /// A use-case would be third party tools that do not support any kind of service location interface for instanced
    /// dependency injection. For example, if a third party tool has to spin up your app - taking control of constructors
    /// away. That tool may require parameterless constructors and have no support for object containers.
    /// 
    /// Refer to ObjectBoss for documentation.
    /// </summary>
    public static class ObjectFactory 
    {
        private static readonly ObjectBoss _objectBoss = new ObjectBoss();

        public static void Configure(Action<IConfiguration> configuration) { _objectBoss.Configure(configuration); }
        public static int GetSingletonCount() { return _objectBoss.GetSingletonCount(); }

        public static T GetInstance<T>() { return _objectBoss.GetInstance<T>(); }
        public static T GetInstance<T>(string key) { return _objectBoss.GetInstance<T>(key); }
        public static object GetInstance(Type type) { return _objectBoss.GetInstance(type);}
        public static object GetInstance(string key) { return _objectBoss.GetInstance(key); }

        public static IEnumerable<T> GetAllInstances<T>() { return _objectBoss.GetAllInstances<T>(); }
        public static IEnumerable<T> GetAllInstances<T>(string key) { return _objectBoss.GetAllInstances<T>(key); }
        public static IEnumerable<object> GetAllInstances(Type type) {  return _objectBoss.GetAllInstances(type); }

        public static bool Contains<T>() { return _objectBoss.Contains<T>(); }
        public static bool ContainsUsing<T, TT>() { return _objectBoss.ContainsUsing<T, TT>(); }
        public static bool Contains(string key) { return _objectBoss.Contains(key); }

        public static void SendMessage(IMessage message) { _objectBoss.SendMessage(message); }
    }
}
