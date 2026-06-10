using Genesis.Logging;

namespace Genesis
{
    public interface IServiceWithoutInterceptor : IDependency
    {
        ILogger Logger { get; set; }
    }
}
