using System.Windows;

namespace Genesis.UI
{
    public interface IZoneManager
    {
        IZoneCollection Zones { get; }
        IZone RegisterZone(FrameworkElement zoneHost, string zoneName);
    }
}
