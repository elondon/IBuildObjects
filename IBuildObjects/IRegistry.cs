using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IBuildObjects
{
    public interface IRegistry
    {
        Action<IConfiguration> GetConfiguration();
    }
}
