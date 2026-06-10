using System.ComponentModel;

namespace Genesis
{
    public interface IViewModel : INotifyPropertyChanged, IDependency
    {
        string RegionName { get; set; }
    }
}
