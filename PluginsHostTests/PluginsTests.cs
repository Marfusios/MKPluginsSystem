using System;
using System.Collections.Generic;
using System.Data;
using PluginsCore;
using PluginsHost;
using Xunit;

namespace PluginsHostTests
{
    public class PluginsTests
    {
        private static readonly FakeHost _fakeHost = new FakeHost();
        private static readonly PluginsCollection _plugCollection = new PluginsCollection();

        [Fact]
        public void LoadAndInitializePlugin()
        {
            var plugin = _plugCollection.LoadPlugin(PluginsCollectionTests.TEST_PLUGIN1_ASSEMBLY, PluginsCollectionTests.TEST_PLUGIN1_FULLNAME);
            var logsCount = getLogsCount(plugin);
            plugin.Init(_fakeHost);
            Assert.Equal(logsCount + 1, getLogsCount(plugin));
        }

        [Fact]
        public void LoadAndClosePlugin()
        {
            var plugin = _plugCollection.LoadPlugin(PluginsCollectionTests.TEST_PLUGIN1_ASSEMBLY, PluginsCollectionTests.TEST_PLUGIN1_FULLNAME);
            var logsCount = getLogsCount(plugin);
            plugin.Init(_fakeHost);
            Assert.Equal(logsCount + 1, getLogsCount(plugin));
            plugin.Close();
            Assert.Equal(logsCount + 2, getLogsCount(plugin));
        }



        private int getLogsCount(object sender)
        {
            return _fakeHost.LogForSender.ContainsKey(sender) ? _fakeHost.LogForSender[sender].Count : 0;
        }

        private class FakeHost : MarshalByRefObject, IPluginParent
        {
            public readonly Dictionary<object, List<string>> LogForSender = new Dictionary<object, List<string>>();

            public string PluginParentId
            {
                get { return "FakeHost"; }
            }

            public void LogMessage(object sender, object objectToLog)
            {
                if (!LogForSender.ContainsKey(sender))
                    LogForSender.Add(sender, new List<string>());
                LogForSender[sender].Add(objectToLog != null ? objectToLog.ToString() : "null");
            }

            public DataTable GetData(string queryName)
            {
                return new DataTable("Fake table");
            }
        }
    }  
}
