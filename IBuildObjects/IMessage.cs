namespace IBuildObjects
{
    /// <summary>
    /// interface for messaging. defines methods that return an IMessage so method chaining can be used.
    /// </summary>
    public interface IMessage
    {
        IMessage InBackground();
    }

    /// <summary>
    /// interface that defines the configuration for a message.
    /// </summary>
    public interface IConfigureMessage
    {
        bool RunInBackground { get; set; }
        bool RunOnDefault { get; set; }
    }

    /// <summary>
    /// base class for message. 
    /// </summary>
    public abstract class Message : IMessage, IConfigureMessage
    {
        public bool RunInBackground { get; set; }
        public bool RunOnDefault { get; set; }

        protected Message()
        {
            RunOnDefault = true;
            RunInBackground = false;
        }

        /// <summary>
        /// sets a flag so that this message is handled on a worker thread.
        /// </summary>
        /// <returns></returns>
        public IMessage InBackground()
        {
            RunInBackground = true;
            return this;
        }

        /// <summary>
        /// sets a flag so that this message is run on the default thread.
        /// </summary>
        /// <returns></returns>
        public IMessage OnDefaultThread()
        {
            RunInBackground = false;
            return this;
        }
    }
}
