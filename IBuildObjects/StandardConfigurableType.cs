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
        IConfigureTypes ForMessagging();
    }

    public interface IConfigurableType
    {
        Type Type { get; set; }
        string Key { get; set; }
        bool IsSingleton { get; }
        bool IsForMessaging { get; }
    }

    public class StandardConfigurableType : IConfigurableType, IConfigureTypes
    {
        public Type Type { get; set; }
        public string Key { get; set; }
        public bool IsSingleton { get; private set; }
        public bool IsForMessaging { get; private set; }

        public StandardConfigurableType()
        {
            
        }

        public IConfigureTypes Singleton()
        {
            IsSingleton = true;
            return this;
        }

        public IConfigureTypes ForMessagging()
        {
            IsForMessaging = true;
            return this;
        }
    }
}
