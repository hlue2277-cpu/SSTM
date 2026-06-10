using Autofac;

namespace Genesis
{
    public interface IShellContainerFactory
    {
        ILifetimeScope CreateScope(string[] features);
    }
}
