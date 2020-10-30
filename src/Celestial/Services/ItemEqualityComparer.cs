using Celestial.Models;
using System.Collections.Generic;

namespace Celestial.Services
{
    internal class ItemEqualityComparer : IEqualityComparer<Apod>
    {
        public bool Equals(Apod x, Apod y) => x.Url == y.Url;

        public int GetHashCode(Apod obj) => obj.Url.GetHashCode();
    }
}
