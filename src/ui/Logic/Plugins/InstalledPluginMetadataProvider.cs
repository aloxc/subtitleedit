using Nikse.SubtitleEdit.Core.Common;
using Nikse.SubtitleEdit.Forms;
using System.Collections.Generic;
using System.Globalization;

namespace Nikse.SubtitleEdit.Logic.Plugins
{
    public class InstalledPluginMetadataProvider : IPluginMetadataProvider
    {
        public IReadOnlyCollection<PluginInfoItem> GetPlugins()
        {
            var installedPlugins = new List<PluginInfoItem>();
            foreach (var pluginFileName in Configuration.GetPlugins())
            {
               
            }

            return installedPlugins;
        }
    }
}