using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Genesis.Events;
using Autofac;
using Genesis.Logging;
using Genesis.UI;

namespace Genesis
{
    public abstract class LoaderBase : ILoader
    {

        private string currentZoneName;

        public IEventAggregator EventAggregator { get; set; }
        public IZoneManager ZoneManager { get; set; }
        public ILifetimeScope Container { get; set; }
        public ILogger Logger { get; set; }

        public void Initialize()
        {
            SubscribeEvents();
            Setup();
        }

        public void RemoveView(string zoneName, IViewModel viewModel)
        {
            IZone zone = this.ZoneManager.Zones[zoneName];
            if ((zone != null) && zone.Contains(viewModel))
            {
                zone.Remove(viewModel);
            }
        }

        public void AttachOnlyView(string zoneName, IViewModel viewModel)
        {
            if (!string.IsNullOrEmpty(currentZoneName))
            {
                IZone currentZone = this.ZoneManager.Zones[currentZoneName];
                if (currentZone != null)
                {
                    currentZone.RemoveAll();
                }
            }

            IZone zone = this.ZoneManager.Zones[zoneName];
            if (zone != null)
            {
                zone.RemoveAll();
                if (!zone.Contains(viewModel))
                {
                    zone.Add(viewModel);
                }

                currentZoneName = zoneName;
            }
        }

        public void AttachView(string zoneName, IViewModel viewModel)
        {
            IZone zone = this.ZoneManager.Zones[zoneName];
            if ((zone != null) && !zone.Contains(viewModel))
            {
                zone.Add(viewModel);
            }
        }

        protected virtual void SubscribeEvents()
        {
        }

        protected virtual void UnsubscribeEvents()
        {
        }

        protected virtual void Setup()
        {
        }

    }
}
