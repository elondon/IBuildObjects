using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBuildObjects
{
    /// <summary>
    /// interface that specifies all configuration options for IBuildObjects. 
    /// </summary>
    public interface IConfiguration
    {
        void AddRegistry<T>();
        IConfigureTypes Add<T>();
        IConfigureTypes Add<T>(string key);
        IConfigureTypes AddUsing<T, TT>();
        IConfigureTypes AddUsing<T, TT>(string key);
        IConfigureTypes AddUsingDefaultType<T, TT>();
    }

    /// <summary>
    /// concrete configuration class that implements IConfiguration. Maps types to their configurations and stores the default implementation for
    /// a given type if there are several interfaces of that type.
    /// </summary>
    public class Configuration : IConfiguration
    {
        private readonly IDictionary<Type, List<IConfigurableType>> _moduleConfiguration;
        private readonly IDictionary<Type, IConfigurableType> _moduleDefaultTypes;

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="config">the built up configuration.</param>
        /// <param name="defaults">the built up mapping of default types. a default type is the type of an instance returned when a call to GetInstance is made on 
        /// an interface that has several implementations.</param>
        public Configuration(IDictionary<Type, List<IConfigurableType>> config, IDictionary<Type, IConfigurableType> defaults)
        {
            _moduleConfiguration = config;
            _moduleDefaultTypes = defaults;
        }

        /// <summary>
        /// implementation of the structuremap 'registry' concept. Allows you to define configurations in separated classes.
        /// useful for definining a configuration per assembly.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public void AddRegistry<T>() 
        {
            var type = typeof(T);
            var registryInstance = Activator.CreateInstance(type);
            if(!(registryInstance is IRegistry))
                throw new Exception("You can only register a class that implements IRegistry");

            var registry = registryInstance as IRegistry;
            registry.GetConfiguration()(this);
        }

        /// <summary>
        /// adds a type the configuration.
        /// </summary>
        /// <typeparam name="T">type to register</typeparam>
        /// <returns></returns>
        public IConfigureTypes Add<T>()
        {
            var configurableType = new StandardConfigurableType() { Type = typeof(T), Key = "" };
            AddToConfiguration(typeof(T), configurableType);
            return configurableType;
        }

        /// <summary>
        /// adds a type to the confgiuration that is retrievable via a key. 
        /// </summary>
        /// <typeparam name="T">type to register</typeparam>
        /// <param name="key">key that will map this instance</param>
        /// <returns></returns>
        public IConfigureTypes Add<T>(string key)
        {
            var configurableType = new StandardConfigurableType() { Type = typeof(T), Key = key };
            AddToConfiguration(typeof(T), configurableType);
            return configurableType;
        }

        /// <summary>
        /// adds a type that will sub-class or implement another class. example: AddUsing IObject MyObject
        /// </summary>
        /// <typeparam name="T">the base class or interface</typeparam>
        /// <typeparam name="TT">the implementing class</typeparam>
        /// <returns></returns>
        public IConfigureTypes AddUsing<T, TT>()
        {
            var configurableType = new StandardConfigurableType() { Type = typeof(TT), Key = "" };
            AddToConfiguration(typeof(T), configurableType);
            return configurableType;
        }

        /// <summary>
        /// adds a type that will sub-class of implement another class retrievable by key. example: AddUsing IObject MyObject ("forThisScenario")
        /// </summary>
        /// <typeparam name="T">the base class or interface</typeparam>
        /// <typeparam name="TT">the implementing class</typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public IConfigureTypes AddUsing<T, TT>(string key)
        {
            var configurableType = new StandardConfigurableType() { Type = typeof(TT), Key = key };
            AddToConfiguration(typeof(T), configurableType);
            return configurableType;
        }

        /// <summary>
        /// adds a type and specifies the default implementation for a base class or interface
        /// </summary>
        /// <typeparam name="T">the base class or interface to add</typeparam>
        /// <typeparam name="TT">the default implementation for that class.</typeparam>
        /// <returns></returns>
        public IConfigureTypes AddUsingDefaultType<T, TT>()
        {
            var configurableType = new StandardConfigurableType() { Type = typeof(TT), Key = "" };
            AddToConfiguration(typeof(T), configurableType);
            _moduleDefaultTypes.Add(typeof(T), configurableType);
            return configurableType;
        }

        private void AddToConfiguration(Type usingType, IConfigurableType configureType)
        {
            if (_moduleConfiguration.ContainsKey(usingType))
            {
                _moduleConfiguration[usingType].Add(configureType);
            }
            else
            {
                _moduleConfiguration.Add(usingType, new List<IConfigurableType>() { configureType });
            }
        }
    }
}
