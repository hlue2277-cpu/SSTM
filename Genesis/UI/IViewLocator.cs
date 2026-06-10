
namespace Genesis.UI
{
    public interface IViewLocator
    {
        object Locate(object view);

        object Locate(IViewModel viewModel);
    }
}
