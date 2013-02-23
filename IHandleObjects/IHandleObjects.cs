using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HandleObjects
{
    public interface IHandleObjects
    {

        int GetObjectCount();
        T GetInstance<T>();
        T GetInstance<T>(string key);
        IEnumerable<T> GetAllInstances<T>();
        IEnumerable<T> GetAllInstances<T>(string key);
    }
}
