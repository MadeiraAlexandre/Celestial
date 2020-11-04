using Celestial.Shared.Models;
using Celestial.Shared.Services;
using Microsoft.Toolkit.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Celestial.Models
{
    public class ApodSource : IIncrementalSource<Apod>
    {
        private List<Apod> _apod;
        private bool _isGridLoaded;
        private DateTimeOffset _lastUpdate;

        public async Task<IEnumerable<Apod>> GetPagedItemsAsync(int pageIndex, int pageSize, CancellationToken cancellationToken = default)
        {
            if (!_isGridLoaded)
            {
                _apod = new List<Apod>();
                _lastUpdate = DateTimeOffset.UtcNow;
                _apod = await ApodClient.FetchApodListAsync(DateTimeOffset.Now.AddDays(-20), DateTimeOffset.UtcNow.AddDays(-1)).ConfigureAwait(false);
                _lastUpdate = DateTimeOffset.Now.AddDays(-20);
                _isGridLoaded = true;
                return _apod.OrderByDescending(o => o.Date);
            }
            else
            {
                _apod.Clear();
                var updatedLastDate = _lastUpdate;
                _apod = await ApodClient.FetchApodListAsync(_lastUpdate.AddDays(-20), _lastUpdate.AddDays(-1)).ConfigureAwait(false);
                _lastUpdate = updatedLastDate.AddDays(-20);
                return _apod.OrderByDescending(o => o.Date);
            }
        }
    }
}