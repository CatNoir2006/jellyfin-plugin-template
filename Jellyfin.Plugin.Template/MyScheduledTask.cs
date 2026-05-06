using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediaBrowser.Model.Tasks;

namespace Jellyfin.Plugin.Template
{
    /// <summary>
    /// A sample scheduled task.
    /// </summary>
    public class MyScheduledTask : IScheduledTask
    {
        /// <inheritdoc />
        public string Name => "My Plugin Task";

        /// <inheritdoc />
        public string Key => "MyPluginTask";

        /// <inheritdoc />
        public string Description => "A sample task for my plugin.";

        /// <inheritdoc />
        public string Category => "My Plugin";

        /// <inheritdoc />
        public Task ExecuteAsync(IProgress<double> progress, CancellationToken cancellationToken)
        {
            progress.Report(0);
            // Do work here
            progress.Report(100);
            return Task.CompletedTask;
        }

        /// <inheritdoc />
        public IEnumerable<TaskTriggerInfo> GetDefaultTriggers()
        {
            return new[] { new TaskTriggerInfo { Type = TaskTriggerInfo.TriggerInterval, IntervalTicks = TimeSpan.FromHours(24).Ticks } };
        }
    }
}
