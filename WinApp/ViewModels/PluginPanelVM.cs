using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Input;
using PluginsCore;
using PluginsCore.Communication;
using WinApp.Utils;

namespace WinApp.ViewModels
{
    internal class PluginPanelVM : ViewModel
    {
        #region PROPERTIES

        private static readonly PropertyChangedEventArgs pluginArgs =
            NotifyPropertyChangedHelper.CreateArgs<PluginPanelVM>(o => o.Plugin);
        private static readonly PropertyChangedEventArgs pluginNameArgs =
            NotifyPropertyChangedHelper.CreateArgs<PluginPanelVM>(o => o.PluginName);
        private static readonly PropertyChangedEventArgs pluginMethodsArgs =
            NotifyPropertyChangedHelper.CreateArgs<PluginPanelVM>(o => o.PluginMethods);
        private static readonly PropertyChangedEventArgs selectedMethodArgs =
            NotifyPropertyChangedHelper.CreateArgs<PluginPanelVM>(o => o.SelectedMethod);
        private static readonly PropertyChangedEventArgs pluginFieldsArgs =
           NotifyPropertyChangedHelper.CreateArgs<PluginPanelVM>(o => o.PluginFields);


        private PluginBase _plugin;
        public PluginBase Plugin
        {
            get { return _plugin; }
            set
            {
                if (value == null)
                    throw new InvalidOperationException("Plugin must be set");
                SetProperty(ref _plugin, value, pluginArgs);
                informAboutPluginChange();
            }
        }

        public string PluginName
        {
            get { return Plugin.TypeFullName; }
        }

        public IEnumerable<Method> PluginMethods
        {
            get { return Plugin.GetMethods(); }
        }

        private Method _selectedMethod;
        public Method SelectedMethod
        {
            get { return _selectedMethod; }
            set { SetProperty(ref _selectedMethod, value, selectedMethodArgs); }
        }

        public IEnumerable<Field> PluginFields
        {
            get { return Plugin.GetFields(); }
        }

        public ICommand RunSelectedMethodCommand
        {
            get
            {
                return new RelayCommand(() =>
                {
                    if (SelectedMethod != null)
                    {
                        try
                        {
                            var metResult = Plugin.ExecuteMethod(SelectedMethod);
                            var result = metResult == null || metResult.Value == null ? string.Empty : metResult.Value;
                            Plugin.LogMessage("Method result: " + result);
                        }
                        catch
                        {
                            Plugin.LogMessage("Error while executing method: " + SelectedMethod.MethodName);
                        }
                    }
                });
            }
        }

        #endregion



        public PluginPanelVM(PluginBase plugin)
        {
            Plugin = plugin;
        }



        private void informAboutPluginChange()
        {
            OnPropertyChanged(pluginNameArgs);
            OnPropertyChanged(pluginMethodsArgs);
            OnPropertyChanged(pluginFieldsArgs);
        }
    }
}
