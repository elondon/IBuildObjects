#region usings

using System;

#endregion

namespace IBuildObjects
{
    public interface IRegistry
    {
        Action<IConfiguration> GetConfiguration();
    }
}
