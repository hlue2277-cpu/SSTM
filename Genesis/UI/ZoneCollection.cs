using System.Collections.Generic;

namespace Genesis.UI
{
    internal class ZoneCollection : IZoneCollection, IEnumerable<IZone>
    {
        private readonly IDictionary<string, IZone> _zones = new Dictionary<string, IZone>();

        public IZone this[string key]
        {
            get
            {
                return this._zones.ContainsKey(key) ? this._zones[key] : null;
            }
            internal set
            {
                this._zones[key] = value;
            }
        }

        public IEnumerator<IZone> GetEnumerator()
        {
            foreach (IZone current in this._zones.Values)
            {
                yield return current;
            }
            yield break;
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}
