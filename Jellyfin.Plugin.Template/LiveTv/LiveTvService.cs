using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediaBrowser.Controller.Library;
using MediaBrowser.Controller.LiveTv;
using MediaBrowser.Model.Dto;
using MediaBrowser.Model.Entities;
using MediaBrowser.Model.LiveTv;
using MediaBrowser.Model.MediaInfo;

namespace Jellyfin.Plugin.Template.LiveTv;

/// <summary>
/// A live tv service for the template plugin.
/// </summary>
public class LiveTvService : ITunerHost, IListingsProvider, IConfigurableTunerHost
{
    /// <inheritdoc />
    public string Name => "Template Live TV";

    /// <inheritdoc />
    public string Type => "template_livetv";

    /// <inheritdoc />
    public bool IsSupported => true;

    /// <inheritdoc />
    public Task<List<ChannelInfo>> GetChannels(bool enableCache, CancellationToken cancellationToken)
    {
        var channels = new List<ChannelInfo> { new() { Name = "Big Buck Bunny", Id = "bunny", Number = "1", ChannelType = ChannelType.TV }, new() { Name = "Jellyfish", Id = "jellyfish", Number = "2", ChannelType = ChannelType.TV } };

        return Task.FromResult(channels);
    }

    /// <inheritdoc />
    public Task<ILiveStream> GetChannelStream(string channelId, string streamId, IList<ILiveStream> currentLiveStreams, CancellationToken cancellationToken)
    {
        var url = channelId switch
        {
            "bunny" => "https://test-videos.co.uk/vids/bigbuckbunny/mp4/h264/1080/Big_Buck_Bunny_1080_10s_1MB.mp4",
            "jellyfish" => "https://test-videos.co.uk/vids/jellyfish/mp4/h264/1080/Jellyfish_1080_10s_10MB.mp4",
            _ => throw new System.IO.FileNotFoundException("Channel not found")
        };

        var mediaSource = new MediaSourceInfo
        {
            Path = url,
            Protocol = MediaProtocol.Http,
            Id = channelId,
            IsInfiniteStream = true,
            SupportsDirectPlay = true,
            SupportsDirectStream = true
        };

        return Task.FromResult<ILiveStream>(new SimpleLiveStream(mediaSource));
    }

    /// <inheritdoc />
    public Task<List<MediaSourceInfo>> GetChannelStreamMediaSources(string channelId, CancellationToken cancellationToken)
    {
        return Task.FromResult(new List<MediaSourceInfo>());
    }

    /// <inheritdoc />
    public Task<List<TunerHostInfo>> DiscoverDevices(int discoveryDurationMs, CancellationToken cancellationToken)
    {
        List<TunerHostInfo> tuners =
        [
            new()
            {
                DeviceId = "template_livetv", FriendlyName = "Template Live TV - Device", Source = "Template Live TV", Type = "template_livetv", Id = "template_livetv", Url = "google.de"
            }
        ];
        return Task.FromResult(tuners);
    }

    /// <inheritdoc />
    public Task Validate(TunerHostInfo info)
    {
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public Task<IEnumerable<ProgramInfo>> GetProgramsAsync(ListingsProviderInfo info, string channelId, DateTime startDateUtc, DateTime endDateUtc, CancellationToken cancellationToken)
    {
        var programs = new List<ProgramInfo>();
        var start = startDateUtc;
        while (start < endDateUtc)
        {
            var end = start.AddHours(2);
            programs.Add(new ProgramInfo
            {
                Name = channelId == "bunny" ? "Big Buck Bunny Movie" : "Jellyfish Documentary",
                Overview = "Hardcoded Live Stream Beispiel.",
                StartDate = start,
                EndDate = end,
                Id = channelId + "_" + start.Ticks,
                ChannelId = channelId
            });

            start = end;
        }

        return Task.FromResult<IEnumerable<ProgramInfo>>(programs);
    }

    /// <inheritdoc />
    public Task Validate(ListingsProviderInfo info, bool validateLogin, bool validateListings)
    {
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public Task<List<NameIdPair>> GetLineups(ListingsProviderInfo info, string country, string location)
    {
        return Task.FromResult(new List<NameIdPair> { new NameIdPair { Name = "Template", Id = "template" } });
    }

    /// <inheritdoc />
    public Task<List<ChannelInfo>> GetChannels(ListingsProviderInfo info, CancellationToken cancellationToken)
    {
        return GetChannels(true, cancellationToken);
    }

    private sealed class SimpleLiveStream : ILiveStream
    {
        public SimpleLiveStream(MediaSourceInfo mediaSource)
        {
            MediaSource = mediaSource;
            UniqueId = Guid.NewGuid().ToString("N");
        }

        /// <inheritdoc />
        public int ConsumerCount { get; set; } = 1;

        /// <inheritdoc />
        public string OriginalStreamId { get; set; } = string.Empty;

        /// <inheritdoc />
        public string TunerHostId => "template_livetv";

        /// <inheritdoc />
        public bool EnableStreamSharing => false;

        /// <inheritdoc />
        public MediaSourceInfo MediaSource { get; set; }

        /// <inheritdoc />
        public string UniqueId { get; }

        /// <inheritdoc />
        public Task Open(CancellationToken openCancellationToken) => Task.CompletedTask;

        /// <inheritdoc />
        public Task Close() => Task.CompletedTask;

        /// <inheritdoc />
        public System.IO.Stream GetStream() => throw new NotSupportedException();

        /// <inheritdoc />
        public void Dispose()
        {
        }
    }
}
