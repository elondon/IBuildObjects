using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IBuildObjects
{
    public class InvalidConfigurationException : Exception
    {
        public IDictionary<Type, List<IConfigurableType>> Configuration { get; private set; }
        public List<IConfigurableType> ConfigurationErrorTypes { get; private set; }

        public InvalidConfigurationException(IDictionary<Type, List<IConfigurableType>> configuration, List<IConfigurableType> configurationErrorTypes)
        {
            Configuration = configuration;
            ConfigurationErrorTypes = configurationErrorTypes;
        }
    }
}
