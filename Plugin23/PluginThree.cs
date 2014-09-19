using PluginsCore;

namespace Plugin23
{
    class PluginThree : PluginBase
    {
        #region PluginBase

        protected override void Init()
        {
            LogMessage("PluginThree initialized");
        }

        public override void Close()
        {
            LogMessage("PluginThree closed");
        }

        #endregion
    }
}
