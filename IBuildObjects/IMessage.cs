using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IHandleObjects
{
    public interface IMessage
    {
        IMessage InBackground();
    }

    public interface IConfigureMessage
    {
        bool RunInBackground { get; set; }
        bool RunOnDefault { get; set; }
    }

    public abstract class Message : IMessage, IConfigureMessage
    {
        public bool RunInBackground { get; set; }
        public bool RunOnDefault { get; set; }

        protected Message()
        {
            RunOnDefault = true;
            RunInBackground = false;
        }

        public IMessage InBackground()
        {
            RunInBackground = true;
            return this;
        }

        public IMessage OnUIThread()
        {
            RunInBackground = false;
            return this;
        }
    }
}
