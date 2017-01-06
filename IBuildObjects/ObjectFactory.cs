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
    /// 
    /// </summary>
    public static class ObjectFactory
    {
        private static ObjectBoss _container;
        public static ObjectBoss Container => _container ?? (_container = new ObjectBoss());

        public static void ClearContainer() { _container = new ObjectBoss(); }

        public static void Configure(Action<IConfiguration> configuration) { Container.Configure(configuration); }
        public static int GetSingletonCount() { return Container.GetSingletonCount(); }
        public static int GetRegisteredClassCount() { return Container.GetRegisteredClassCount(); }

        public static T GetInstance<T>() { return Container.GetInstance<T>(); }
        public static T GetInstance<T>(string key) { return Container.GetInstance<T>(key); }
        public static object GetInstance(Type type) { return Container.GetInstance(type);}
        public static object GetInstance(string key) { return Container.GetInstance(key); }

        public static IEnumerable<T> GetAllInstances<T>() { return Container.GetAllInstances<T>(); }
        public static IEnumerable<T> GetAllInstances<T>(string key) { return Container.GetAllInstances<T>(key); }
        public static IEnumerable<object> GetAllInstances(Type type) {  return Container.GetAllInstances(type); }

        public static bool Contains<T>() { return Container.Contains<T>(); }
        public static bool ContainsUsing<T, TT>() { return Container.ContainsUsing<T, TT>(); }
        public static bool Contains(string key) { return Container.Contains(key); }

        public static void SendMessage(IMessage message) { Container.SendMessage(message); }
    }
}
