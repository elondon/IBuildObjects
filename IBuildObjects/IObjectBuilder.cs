using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBuildObjects
{
    public interface IObjectBuilder
    {

        int GetObjectCount();
        T GetInstance<T>();
        T GetInstance<T>(string key);
        IEnumerable<T> GetAllInstances<T>();
        IEnumerable<T> GetAllInstances<T>(string key);
    }
}
