using PluginsCore;

namespace Plugin1
{
    public class PluginOne : PluginBase
    {
        #region PluginBase

        protected override void Init()
        {
            LogMessage("PluginOne initialized");
        }

        public override void Close()
        {
            LogMessage("PluginOne closed");
        }

        #endregion

        public string TestField = "default value";

        public void TestMethod()
        {
        }

        private void someMethod()
        {
            var data = Parent.GetData("query1");
        }

    }
}
