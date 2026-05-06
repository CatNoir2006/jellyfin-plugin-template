using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Jellyfin.Plugin.Template.LiveTv;
using MediaBrowser.Controller;
using MediaBrowser.Controller.LiveTv;
using MediaBrowser.Controller.Plugins;
using Microsoft.Extensions.DependencyInjection;

namespace Jellyfin.Plugin.Template;

/// <summary>
/// Register services for the plugin.
/// </summary>
public class ServiceRegistrator : IPluginServiceRegistrator
{
    /// <inheritdoc />
    public void RegisterServices(IServiceCollection serviceCollection, IServerApplicationHost applicationHost)
    {
        serviceCollection.AddSingleton<LiveTvService>();
        serviceCollection.AddSingleton<ITunerHost>(s => s.GetRequiredService<LiveTvService>());
        serviceCollection.AddSingleton<IListingsProvider>(s => s.GetRequiredService<LiveTvService>());
    }
}
