using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Lifetime;
using PluginsCore;
using PluginsCore.Utils;

namespace PluginsHost
{
    public class PluginsCollection : IEnumerable<PluginBase>
    {
        #region CONST PROPERTIES

        private const string ASSEMBLY_EXTENSION = ".dll";
        private const string PLUGINS_DIR_NAME = "Plugins";
        private const string PLUGINS_CACHE_DIR_NAME = "PluginsShadowCache";

        private static readonly string PATH_TO_PLUGINS_DIR = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
            PLUGINS_DIR_NAME);

        // Factory (In PluginsCore - shared between this and each Plugin) for creating Plugins in other AppDomain
        private static readonly string FACTORY_ASSEMBLY_NAME = typeof (PluginFactory).Assembly.FullName;
        private static readonly string FACTORY_FULLNAME = typeof (PluginFactory).FullName;

        #endregion

        private readonly ISponsor _sponsor = new DefaultSponsor();
        private readonly IDictionary<PluginBase, AppDomain> _loadedPlugins =
            new Dictionary<PluginBase, AppDomain>();

        private AppDomainSetup _appDomainSetup;


        public PluginsCollection()
        {
            initializeAppDomainSetup();
        }

        public IEnumerable<PluginBase> this[string fullname]
        {
            get
            {
                return
                    _loadedPlugins.Where(x => x.Key.TypeFullName.Equals(fullname))
                        .Select(x => x.Key);
            }
        }

        public IEnumerable<PluginBase> LoadPlugin(string assemblyName)
        {
            check(assemblyName);
            return loadPluginsInAssembly(assemblyName);
        }

        public PluginBase LoadPlugin(string assemblyName, string fullname)
        {
            check(assemblyName, fullname);
            return loadSpecificPlugin(assemblyName, fullname);
        }

        /// <summary>
        /// It removes server plugin from collection and unloads its AppDomain
        /// </summary>
        /// <param name="plugin"></param>
        public void RemovePlugin(PluginBase plugin)
        {
            if (_loadedPlugins.Any(x => x.Key == plugin))
            {
                var plugins = _loadedPlugins.Where(x => x.Key == plugin);
                removeSelected(plugins);
            }
        }

        public void StopAll()
        {
            foreach (var plugin in _loadedPlugins)
            {
                plugin.Key.Close();
            }
        }

        public void StopAndRemoveAll()
        {
            StopAll();
            removeSelected(_loadedPlugins);
        }

        public bool Contains(string fullname)
        {
            return _loadedPlugins.Any(x => x.Key.TypeFullName.Equals(fullname));
        }

        #region PRIVATE METHODS

        private void removeSelected(IEnumerable<KeyValuePair<PluginBase, AppDomain>> plugins)
        {
            foreach (var plug in plugins.ToArray())
            {
                AppDomain domain = plug.Value;
                _loadedPlugins.Remove(plug);
                unregisterSponsor(plug.Key);

                if (_loadedPlugins.All(x => x.Value != domain))
                    AppDomain.Unload(domain);
            }
        }

        private void initializeAppDomainSetup()
        {
            _appDomainSetup = new AppDomainSetup
            {
                ApplicationName = "",
                //with MSDN: If the ApplicationName property is not set, the CachePath property is ignored and the download cache is used. No exception is thrown.
                ShadowCopyFiles = "true", //Enabling ShadowCopy - yes, it's string value
                ApplicationBase = PATH_TO_PLUGINS_DIR, //Base path for new app domain - our plugins folder
                CachePath = PLUGINS_CACHE_DIR_NAME //Path, where we want to have our copied dlls store. 
            };
        }


        private IEnumerable<PluginBase> loadPluginsInAssembly(string assemblyName)
        {
            assemblyName = Path.GetFileNameWithoutExtension(assemblyName);

            AppDomain domain = createDomain(assemblyName);
            PluginFactory factory = createFactory(domain);
            List<PluginBase> plugins = factory.CreatePlugin(assemblyName).ToList();
            var result = new List<PluginBase>();
            foreach (PluginBase plugin in plugins)
            {
                registerSponsor(plugin);
                _loadedPlugins.Add(plugin, domain);
                result.Add(plugin);
            }
            return result;
        }

        private PluginBase loadSpecificPlugin(string assemblyName, string fullname)
        {
            string asmName = Path.GetFileNameWithoutExtension(assemblyName);

            AppDomain domain = createDomain(asmName, fullname);
            PluginFactory factory = createFactory(domain);
            PluginBase plugin = factory.CreatePlugin(asmName, fullname);
            registerSponsor(plugin);
            _loadedPlugins.Add(plugin, domain);
            return plugin;
        }


        private AppDomain createDomain(string assemblyName)
        {
            return AppDomain.CreateDomain("AppDomain for Plugins in asm: " + assemblyName, null, _appDomainSetup);
        }

        private AppDomain createDomain(string assemblyName, string fullname)
        {
            return createDomain("AppDomain for Plugin: " + assemblyName + " - " + fullname);
        }

        private PluginFactory createFactory(AppDomain inDomain)
        {
            return (PluginFactory)inDomain.CreateInstance(FACTORY_ASSEMBLY_NAME, FACTORY_FULLNAME).Unwrap();
        }

        private void check(string assembly, string fullname)
        {
            if (string.IsNullOrEmpty(fullname))
                throw new PluginsException("Fullname name is null");

            check(assembly);
        }

        private void check(string assembly)
        {
            if (string.IsNullOrEmpty(assembly))
                throw new PluginsException("Assembly name is null");

            checkPluginsDir();
            checkPluginAssembly(assembly);
        }

        private void checkPluginsDir()
        {
            if (!Directory.Exists(PATH_TO_PLUGINS_DIR))
                throw new PluginsException(
                    String.Format("Directory '{0}' not exists in executing directory. Create dir at path: {1}",
                        PLUGINS_DIR_NAME, PATH_TO_PLUGINS_DIR));
        }

        private void checkPluginAssembly(string assembly)
        {
            if (!assembly.ToLower().EndsWith(ASSEMBLY_EXTENSION))
                assembly = assembly + ASSEMBLY_EXTENSION;

            if (!File.Exists(Path.Combine(PATH_TO_PLUGINS_DIR, assembly)))
                throw new PluginsException(String.Format("File '{0}' not exists in '{1}' directory: {2}", assembly,
                    PLUGINS_DIR_NAME, PATH_TO_PLUGINS_DIR));
        }

        private void registerSponsor(PluginBase plugin)
        {
            object lifetimeService = RemotingServices.GetLifetimeService(plugin);
            if (lifetimeService is ILease)
            {
                var lease = (ILease)lifetimeService;
                lease.Register(_sponsor);
            }
        }

        private void unregisterSponsor(PluginBase plugin)
        {
            object lifetimeService = RemotingServices.GetLifetimeService(plugin);
            if (lifetimeService is ILease)
            {
                var lease = (ILease)lifetimeService;
                lease.Unregister(_sponsor);
            }
        }


        #endregion

        #region ENUMERATOR

        public IEnumerator<PluginBase> GetEnumerator()
        {
            return _loadedPlugins.Keys.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion  
    }  
}