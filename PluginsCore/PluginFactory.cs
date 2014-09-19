using System;
using System.Collections.Generic;

namespace PluginsCore
{
    /// <summary>
    /// Class for creating instances in current AppDomain
    /// It must be non static!
    /// </summary>
    public class PluginFactory : MarshalByRefObject
    {
        public T CreatePlugin<T>(string assembly, string fullname)
        {
            return (T)Activator.CreateInstance(assembly, fullname).Unwrap();
        }

        public PluginBase CreatePlugin(string assembly, string fullname)
        {
            return CreatePlugin<PluginBase>(assembly, fullname);
        }

        public IEnumerable<T> CreatePlugin<T>(string assembly)
        {
            var result = new List<T>();
            var asm = AppDomain.CurrentDomain.Load(assembly);

            foreach (var type in asm.GetTypes())
            {
                if (typeof(T).IsAssignableFrom(type) && !type.IsAbstract)
                {
                    result.Add((T)Activator.CreateInstance(assembly, type.FullName).Unwrap());
                }
            }
            return result;
        }

        public IEnumerable<PluginBase> CreatePlugin(string assembly)
        {
            return CreatePlugin<PluginBase>(assembly);
        }

    }
}
