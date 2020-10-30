using Celestial.Services;
using Microsoft.Toolkit.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Celestial.Models
{
    public class ApodSource : IIncrementalSource<Apod>
    {
        private List<Apod> FetchList;
        private bool isGridFirstLoaded = false;

        public async Task<IEnumerable<Apod>> GetPagedItemsAsync(int pageIndex, int pageSize, CancellationToken cancellationToken = default)
        {
            if (isGridFirstLoaded == false)
            {
                FetchList = await CacheData.ReadCacheAsync().ConfigureAwait(false);
                isGridFirstLoaded = true;
            }
            else
            {
                var newLastFeedUpdate = AppSettings.Instance.LastFeedUpdate.AddMonths(-1);
                FetchList = await ApodClient.FetchApodListAsync(AppSettings.Instance.LastFeedUpdate.AddMonths(-1), AppSettings.Instance.LastFeedUpdate).ConfigureAwait(false);
                await CacheData.WriteCacheAsync(FetchList).ConfigureAwait(false);
                AppSettings.Instance.LastFeedUpdate = newLastFeedUpdate;
            }
            return FetchList;
        }
    }
}