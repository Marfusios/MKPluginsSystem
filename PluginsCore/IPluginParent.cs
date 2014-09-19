using System.Data;

namespace PluginsCore
{
    public interface IPluginParent
    {
        string PluginParentId { get; }
        void LogMessage(object sender, object objectToLog);

        DataTable GetData(string queryName);
    }
}
