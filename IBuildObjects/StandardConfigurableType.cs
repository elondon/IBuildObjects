﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBuildObjects
{
    /// <summary>
    /// interface used to fluently define configuration options.
    /// </summary>
    public interface IConfigureTypes
    {
        IConfigureTypes Singleton();
        IConfigureTypes ForMessagging();
        IConfigureTypes WithCustomConstructor(Dictionary<string, object> arguments);
    }

    /// <summary>
    /// interface used to store information about the configuration for a given type.
    /// </summary>
    public interface IConfigurableType
    {
        Type Type { get; set; }
        string Key { get; set; }
        bool IsSingleton { get; }
        bool IsForMessaging { get; }
        Dictionary<string, object> Arguments { get; }
    }

    /// <summary>
    /// represents the configuration for a given type and has methods that allow fluent definition of configurable options.
    /// </summary>
    public class StandardConfigurableType : IConfigurableType, IConfigureTypes
    {
        public Type Type { get; set; }
        public string Key { get; set; }
        public bool IsSingleton { get; private set; }
        public bool IsForMessaging { get; private set; }
        public Dictionary<string, object> Arguments { get; private set; }

        public StandardConfigurableType()
        {
            Arguments = new Dictionary<string, object>();
        }

        /// <summary>
        /// defines an object as a singleton.
        /// </summary>
        /// <returns></returns>
        public IConfigureTypes Singleton()
        {
            IsSingleton = true;
            return this;
        }

        /// <summary>
        /// defines an object as one that will receive messages from other objects.
        /// </summary>
        /// <returns></returns>
        public IConfigureTypes ForMessagging()
        {
            IsForMessaging = true;
            return this;
        }

        /// <summary>
        /// allows the client code to specify a custom constructor
        /// </summary>
        /// <param name="arguments">dictionary of arguments where the key is the name of the argument and the object is its value. IBuildObjects
        /// will automatically handle the injection of the arguments when an instance is retreived. The client code can mix custom arguments with
        /// standard injected arguments. If an argument is not found in the argument list, IBuildObjects will inject whatever is defined in the
        /// configuration for that argument type.</param>
        /// <returns></returns>
        public IConfigureTypes WithCustomConstructor(Dictionary<string, object> arguments)
        {
            Arguments = arguments;
            return this;
        }
    }
}
