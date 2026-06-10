using System.Collections.Generic;

namespace Genesis.UI
{
    public interface IZoneCollection : IEnumerable<IZone>
    {
        IZone this[string key] { get; }
    }
}
