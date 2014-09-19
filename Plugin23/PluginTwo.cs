using PluginsCore;

namespace Plugin23
{
    public class PluginTwo : PluginBase
    {
        #region PluginBase

        protected override void Init()
        {
            LogMessage("PluginTwo initialized");
        }

        public override void Close()
        {
            LogMessage("PluginTwo closed");
        }

        #endregion
    }
}
