using Celestial.Models;
using Celestial.Services;
using Microsoft.Toolkit.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Celestial.Utils
{
    public class ApodIncrementalSource : IIncrementalSource<Apod>
    {
        private List<Apod> ApodList { get; set; }
        private bool IsGridLoaded { get; set; }
        private DateTimeOffset LastUpdate { get; set; }

        public async Task<IEnumerable<Apod>> GetPagedItemsAsync(int pageIndex, int pageSize, CancellationToken cancellationToken = default)
        {
            if (!IsGridLoaded)
            {
                ApodList = new List<Apod>();
                LastUpdate = DateTimeOffset.UtcNow;
                ApodList = await ApodClient.FetchApodListAsync(DateTimeOffset.Now.AddDays(-20), DateTimeOffset.UtcNow.AddDays(-1)).ConfigureAwait(false);
                LastUpdate = DateTimeOffset.Now.AddDays(-20);
                IsGridLoaded = true;
            }
            else
            {
                ApodList.Clear();
                ApodList = await ApodClient.FetchApodListAsync(LastUpdate.AddDays(-20), LastUpdate.AddDays(-1)).ConfigureAwait(false);
                var updatedDate = LastUpdate;
                LastUpdate = updatedDate.AddDays(-20);
            }
            return ApodList.OrderByDescending(o => o.Date);
        }
    }
}