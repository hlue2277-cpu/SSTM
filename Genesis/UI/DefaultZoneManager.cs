using System;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Data;

namespace Genesis.UI
{
    public class DefaultZoneManager:IZoneManager
    {
        private readonly IViewLocator _viewLocator;
        private readonly IZoneCollection _zones = new ZoneCollection();

        public DefaultZoneManager(IViewLocator viewLocator)
        {
            _viewLocator = viewLocator;
        }

        public IZoneCollection Zones
        {
            get { return _zones; }
        }

        public IZone RegisterZone(System.Windows.FrameworkElement zoneHost, string zoneName)
        {
            var zone = new Zone(_viewLocator)
            {
                ZoneName = zoneName
            };

            if (zoneHost is ContentControl)
            {
                Adapt(zoneHost as ContentControl, zone);
            }
            else if (zoneHost is ItemsControl)
            {
                Adapt(zoneHost as ItemsControl, zone);
            }
            else
            {
                return null;
            }

            var zones = _zones as ZoneCollection;

            zones[zoneName] = zone;

            return zone;
        }

        private void Adapt(ContentControl contentControl, Zone zone)
        {
            bool contentIsSet = (null != BindingOperations.GetBinding(contentControl, ContentControl.ContentProperty));

            if (contentIsSet)
            {
                throw new InvalidOperationException("ContentControl's Content property is not empty.");
            }

            zone.InternalViews.CollectionChanged += delegate
            {
                //var view = zone.InternalViews.FirstOrDefault();

                //var type = view.GetType().AssemblyQualifiedName;

                //var viewType = Type.GetType(type.Replace("ViewModel", "View"));

                //if (null == viewType)
                //{

                //}

                //var vi = Activator.CreateInstance(viewType);

                //(vi as FrameworkElement).DataContext = view;

                //contentControl.Content = vi;
                contentControl.Content = zone.InternalViews.FirstOrDefault();
            };

            //zone.Views.CollectionChanged +=
            //    (sender, e) =>
            //    {
            //        if (e.Action == NotifyCollectionChangedAction.Add && zone.ActiveViews.Count() == 0)
            //        {
            //            zone.Activate(e.NewItems[0]);
            //        }
            //    };
        }

        private void Adapt(ItemsControl itemsControl, Zone zone)
        {
            var itemsSourceIsSet = (null != BindingOperations.GetBinding(itemsControl, ItemsControl.ItemsSourceProperty));

            if (itemsSourceIsSet)
            {
                throw new InvalidOperationException("ItemsControl's ItemsSource property is not empty.");
            }

            // If control has child items, move them to the region and then bind control to region.
            // Can't set ItemsSource if child items exist.
            if (0 < itemsControl.Items.Count)
            {
                foreach (var childItem in itemsControl.Items)
                {
                    zone.Add(childItem);
                }

                itemsControl.Items.Clear();
            }

            itemsControl.ItemsSource = zone.InternalViews;
        }
    }
}
