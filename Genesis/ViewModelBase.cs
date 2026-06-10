using Autofac;
using Genesis.Events;
using Genesis.Logging;
using Genesis.UI;

namespace Genesis
{
    public abstract class ViewModelBase : NotifyObject, IViewModel
    {
        public IEventAggregator EventAggregator { get; set; }

        private ILogger _ILogger;

        public ILogger Logger
        {
            get
            {
                if (_ILogger == null)
                {
                    _ILogger = new GenesisLogger();
                }

                return _ILogger;
            }
            set
            {
                _ILogger = value;
            }
        }

        public ILifetimeScope Container { get; set; }
        public IZoneManager ZoneManager { get; set; }
        public string RegionName { get; set; }

        /// <summary>
        /// 需要Container EventAggregator和Logger
        /// </summary>
        /// <param name="aggregator"></param>
        /// <param name="logger"></param>
        /// <param name="container"></param>
        public ViewModelBase(IEventAggregator aggregator, ILogger logger, ILifetimeScope container)
            : this()
        {
            Container = container;
            EventAggregator = aggregator;
            Logger = logger;
        }

        /// <summary>
        /// 需要Container和Logger
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="container"></param>
        public ViewModelBase(ILogger logger, ILifetimeScope container)
            : this()
        {
            Container = container;
            Logger = logger;
        }

        /// <summary>
        /// 需要EventAggregator和Logger
        /// </summary>
        /// <param name="aggregator"></param>
        /// <param name="logger"></param>
        public ViewModelBase(IEventAggregator aggregator, ILogger logger)
            : this()
        {
            EventAggregator = aggregator;
            Logger = logger;
        }

        /// <summary>
        /// 需要Logger
        /// </summary>
        /// <param name="logger"></param>
        public ViewModelBase(ILogger logger)
            : this()
        {
            Logger = logger;
        }

        public ViewModelBase(IZoneManager zoneManager)
            : this()
        {
            ZoneManager = zoneManager;
        }

        /// <summary>
        /// 默认构造函数
        /// </summary>
        public ViewModelBase()
        {
        }

        public void AttachView(string zoneName, IViewModel viewModel)
        {
            IZone zone = this.ZoneManager.Zones[zoneName];
            if ((zone != null) && !zone.Contains(viewModel))
            {
                zone.Add(viewModel);
            }
        }

        public void RemoveView(string zoneName, IViewModel viewModel)
        {
            IZone zone = this.ZoneManager.Zones[zoneName];
            if ((zone != null) && zone.Contains(viewModel))
            {
                zone.Remove(viewModel);
            }
        }
    }
}