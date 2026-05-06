using MediaBrowser.Controller.Entities;
using MediaBrowser.Controller.Resolvers;
using MediaBrowser.Model.IO;

namespace Jellyfin.Plugin.Template
{
    /// <summary>
    /// A sample ignore rule.
    /// </summary>
    public class MyIgnoreRule : IResolverIgnoreRule
    {
        /// <inheritdoc />
        public bool ShouldIgnore(FileSystemMetadata fileInfo, BaseItem? parent)
        {
            // Ignore files named 'ignore_me.txt'
            return fileInfo.Name == "ignore_me.txt";
        }
    }
}
