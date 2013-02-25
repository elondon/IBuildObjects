using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBuildObjects
{
    public interface IConfigureTypes
    {
        IConfigureTypes Singleton();
    }

    public interface IConfigurableType
    {
        Type Type { get; set; }
        string Key { get; set; }
        bool IsSingleton { get; }
    }

    public class StandardConfigurableType : IConfigurableType, IConfigureTypes
    {
        public Type Type { get; set; }
        public string Key { get; set; }
        public bool IsSingleton { get; private set; }

        public StandardConfigurableType()
        {
            
        }

        public IConfigureTypes Singleton()
        {
            IsSingleton = true;
            return this;
        }
    }
}
