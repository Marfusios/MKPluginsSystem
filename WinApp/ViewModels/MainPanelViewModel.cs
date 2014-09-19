using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows.Input;
using PluginsCore;
using PluginsHost;
using WinApp.Utils;

namespace WinApp.ViewModels
{
    internal class MainPanelViewModel : ViewModel
    {
        private const string PLUGINS_DIRECTORY = "Plugins";

        #region PROPERTIES

        private static readonly PropertyChangedEventArgs selectedPluginArgs =
            NotifyPropertyChangedHelper.CreateArgs<MainPanelViewModel>(o => o.SelectedPlugin);
        private static readonly PropertyChangedEventArgs loadedPluginsArgs =
            NotifyPropertyChangedHelper.CreateArgs<MainPanelViewModel>(o => o.LoadedPlugins);

        private PluginPanelVM _selectedPlugin;
        public PluginPanelVM SelectedPlugin
        {
            get { return _selectedPlugin; }
            set { SetProperty(ref _selectedPlugin, value, selectedPluginArgs); }
        }

        public IEnumerable<PluginPanelVM> LoadedPlugins
        {
            get { return _pluginsCollection.Select(x => new PluginPanelVM(x)); }
        }

        public PluginParent PluginParent { get; private set; }


        public ICommand LoadAllPluginsCommand
        {
            get { return new RelayCommand(loadAllPlugins); }
        }

        public ICommand InitializeSelectedPluginCommand
        {
            get { return new RelayCommand(() => { if (SelectedPlugin != null) SelectedPlugin.Plugin.Init(PluginParent); }); }
        }

        public ICommand CloseSelectedPluginCommand
        {
            get { return new RelayCommand(() => { if (SelectedPlugin != null) SelectedPlugin.Plugin.Close(); }); }
        }

        public ICommand UnloadSelectedPluginCommand
        {
            get { return new RelayCommand(() =>
            {
                if (SelectedPlugin != null)
                {
                    _pluginsCollection.RemovePlugin(SelectedPlugin.Plugin);
                    informAboutChange();
                }
            }); }
        }

        #endregion


        private PluginsCollection _pluginsCollection = new PluginsCollection();

        public MainPanelViewModel()
        {
            PluginParent = new PluginParent();
        }


        private void loadAllPlugins()
        {
            checkIfPluginsDirExists();
            var assemblies = getAssembliesToLoad();
            foreach (var assembly in assemblies)
            {
                _pluginsCollection.LoadPlugin(assembly);
            }
            informAboutChange();
        }

        private void checkIfPluginsDirExists()
        {
            if (!Directory.Exists(PLUGINS_DIRECTORY))
                throw new InvalidOperationException(string.Format("Directory {0} doesn't exists", PLUGINS_DIRECTORY));
        }

        private IEnumerable<string> getAssembliesToLoad()
        {
            return Directory
                .EnumerateFiles(PLUGINS_DIRECTORY)
                .Where(x => x.ToLower().EndsWith("dll"))
                .Select(Path.GetFileNameWithoutExtension)
                .Where(x => !isPluginsCoreAssembly(x));
        }

        private bool isPluginsCoreAssembly(string assemblyName)
        {
            return typeof (PluginBase).Assembly.FullName.Contains(assemblyName);
        }

        private void informAboutChange()
        {
            OnPropertyChanged(loadedPluginsArgs);
        }

    }
}
