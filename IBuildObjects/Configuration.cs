using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBuildObjects
{
    public interface IConfiguration
    {
        void AddRegistry<T>();
        IConfigureTypes Add<T>();
        IConfigureTypes Add<T>(string key);
        IConfigureTypes AddUsing<T, TT>();
        IConfigureTypes AddUsing<T, TT>(string key);
        IConfigureTypes AddUsingDefaultType<T, TT>();
    }

    public class Configuration : IConfiguration
    {
        private readonly IDictionary<Type, List<IConfigurableType>> ModuleConfiguration = new Dictionary<Type, List<IConfigurableType>>();
        private readonly IDictionary<Type, IConfigurableType> ModuleDefaultTypes = new Dictionary<Type, IConfigurableType>();

        public Configuration(IDictionary<Type, List<IConfigurableType>> config, IDictionary<Type, IConfigurableType> defaults)
        {
            ModuleConfiguration = config;
            ModuleDefaultTypes = defaults;
        }

        public void AddRegistry<T>() 
        {
            var type = typeof(T);
            var registryInstance = Activator.CreateInstance(type);
            if(!(registryInstance is IRegistry))
                throw new Exception("You can only register a class that implements IRegistry");

            var registry = registryInstance as IRegistry;
            registry.GetConfiguration()(this);

        }

        public IConfigureTypes Add<T>()
        {
            var configurableType = new StandardConfigurableType() { Type = typeof(T), Key = "" };
            AddToConfiguration(typeof(T), configurableType);
            return configurableType;
        }

        public IConfigureTypes Add<T>(string key)
        {
            var configurableType = new StandardConfigurableType() { Type = typeof(T), Key = key };
            AddToConfiguration(typeof(T), configurableType);
            return configurableType;
        }

        public IConfigureTypes AddUsing<T, TT>()
        {
            var configurableType = new StandardConfigurableType() { Type = typeof(TT), Key = "" };
            AddToConfiguration(typeof(T), configurableType);
            return configurableType;
        }

        public IConfigureTypes AddUsing<T, TT>(string key)
        {
            var configurableType = new StandardConfigurableType() { Type = typeof(TT), Key = key };
            AddToConfiguration(typeof(T), configurableType);
            return configurableType;
        }

        public IConfigureTypes AddUsingDefaultType<T, TT>()
        {
            var configurableType = new StandardConfigurableType() { Type = typeof(TT), Key = "" };
            AddToConfiguration(typeof(T), configurableType);
            ModuleDefaultTypes.Add(typeof(T), configurableType);
            return configurableType;
        }

        private void AddToConfiguration(Type usingType, IConfigurableType configureType)
        {

            if (ModuleConfiguration.ContainsKey(usingType))
            {
                ModuleConfiguration[usingType].Add(configureType);
            }
            else
            {
                ModuleConfiguration.Add(usingType, new List<IConfigurableType>() { configureType });
            }
        }
    }
}
