using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HandleObjects;

namespace IHandleObjects
{
    public interface IConfiguration
    {
        IConfiguration Add<T>();
        IConfiguration Add<T>(string key);
        IConfiguration AddUsing<T, TT>();
        IConfiguration AddUsing<T, TT>(string key);
        IConfiguration AddUsingDefaultType<T, TT>();
    }

    public class Configuration : IConfiguration
    {
        private readonly IDictionary<Type, List<ConfigurableType>> ModuleConfiguration = new Dictionary<Type, List<ConfigurableType>>();
        private readonly IDictionary<Type, ConfigurableType> ModuleDefaultTypes = new Dictionary<Type, ConfigurableType>();

        public Configuration(IDictionary<Type, List<ConfigurableType>> config, IDictionary<Type, ConfigurableType> defaults)
        {
            ModuleConfiguration = config;
            ModuleDefaultTypes = defaults;
        }

        public IConfiguration Add<T>()
        {
            var configurableType = new ConfigurableType() { Type = typeof(T), Key = "" };
            AddToConfiguration(typeof(T), configurableType);
            return this;
        }

        public IConfiguration Add<T>(string key)
        {
            var configurableType = new ConfigurableType() { Type = typeof(T), Key = key };
            AddToConfiguration(typeof(T), configurableType);
            return this;
        }

        public IConfiguration AddUsing<T, TT>()
        {
            var configurableType = new ConfigurableType() { Type = typeof(TT), Key = "" };
            AddToConfiguration(typeof(T), configurableType);
            return this;
        }

        public IConfiguration AddUsing<T, TT>(string key)
        {
            var configurableType = new ConfigurableType() { Type = typeof(TT), Key = key };
            AddToConfiguration(typeof(T), configurableType);
            return this;
        }

        public IConfiguration AddUsingDefaultType<T, TT>()
        {
            var configurableType = new ConfigurableType() { Type = typeof(TT), Key = "" };
            AddToConfiguration(typeof(T), configurableType);
            ModuleDefaultTypes.Add(typeof(T), configurableType);
            return this;
        }

        private void AddToConfiguration(Type usingType, ConfigurableType configureType)
        {
            if (ModuleConfiguration.ContainsKey(usingType))
            {
                ModuleConfiguration[usingType].Add(configureType);
            }
            else
            {
                ModuleConfiguration.Add(usingType, new List<ConfigurableType>() { configureType });
            }
        }
    }
}
