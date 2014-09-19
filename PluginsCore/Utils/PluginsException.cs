using System;
using System.Runtime.Serialization;

namespace PluginsCore.Utils
{
    [Serializable]
    public class PluginsException : Exception
    {
        public PluginsException()
        {

        }

        public PluginsException(string msg)
            : base(msg)
        {

        }

        public PluginsException(string msg, Exception inner)
            : base(msg, inner)
        {

        }

        // Need for serialization
        protected PluginsException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
