using System;
using System.Runtime.Remoting.Lifetime;

namespace PluginsCore
{
    [Serializable]
    public class DefaultSponsor : ISponsor
    {
        public TimeSpan Renewal(ILease lease)
        {
            return TimeSpan.FromMinutes(5);
        }
    }
}
