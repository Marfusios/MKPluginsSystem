using System;
using System.Collections.ObjectModel;
using System.Data;
using PluginsCore;

namespace WinApp
{
    class PluginParent : MarshalByRefObject, IPluginParent
    {
        public ObservableCollection<string> Logs { get; private set; }

        public PluginParent()
        {
            Logs = new ObservableCollection<string>();
        }

        public string PluginParentId
        {
            get { return "WinApp host"; }
        }

        public void LogMessage(object sender, object objectToLog)
        {
            Logs.Add(string.Format("{0} - {1}  -  {2}", DateTime.Now, sender.ToString(), objectToLog.ToString()));
        }

        public System.Data.DataTable GetData(string queryName)
        {
            return new DataTable("Default table");
        }
    }
}
