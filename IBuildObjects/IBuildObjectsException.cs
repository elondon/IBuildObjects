using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace IBuildObjects
{
    public class IBuildObjectsException : Exception
    {
        public IBuildObjectsException()
        {
        }

        public IBuildObjectsException(string message) : base(message)
        {
        }

        public IBuildObjectsException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected IBuildObjectsException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
