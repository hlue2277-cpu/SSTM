using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

namespace Genesis.UI
{
    public partial class Zone : IZone
    {
        #region Name
        public static string GetName(DependencyObject obj)
        {
            return (string)obj.GetValue(NameProperty);
        }

        public static void SetName(DependencyObject obj, string value)
        {
            obj.SetValue(NameProperty, value);
        }

        // Using a DependencyProperty as the backing store for Name.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty NameProperty =
            DependencyProperty.RegisterAttached("Name", typeof(string), typeof(Zone), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.None, OnZoneNameChanged));

        private static void OnZoneNameChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            var element = sender as FrameworkElement;

            if (null != element
                && null != args.NewValue
                && null != GetZoneManager(element))
            {
                var zoneName = GetName(element);
                var zoneManager = GetZoneManager(element);
                var zone = zoneManager.RegisterZone(element, zoneName);
            }
        }
        #endregion

        #region ZoneManager
        public static IZoneManager GetZoneManager(DependencyObject obj)
        {
            return (IZoneManager)obj.GetValue(ZoneManagerProperty);
        }
        public static void SetZoneManager(DependencyObject obj, IZoneManager value)
        {
            obj.SetValue(ZoneManagerProperty, value);
        }

        // Using a DependencyProperty as the backing store for ZoneManager.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ZoneManagerProperty =
            DependencyProperty.RegisterAttached("ZoneManager", typeof(IZoneManager), typeof(Zone), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.Inherits, new PropertyChangedCallback(OnZoneManagerChanged)));

        private static void OnZoneManagerChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            FrameworkElement element = sender as FrameworkElement;
            if (null != element && null != args.NewValue && DependencyProperty.UnsetValue != element.ReadLocalValue(NameProperty))
            {
                string zoneName = GetName(element);
                IZoneManager zoneManager = GetZoneManager(element);
                IZone zone = zoneManager.RegisterZone(element, zoneName);
            }
        }
        #endregion

        private readonly System.Collections.Generic.IDictionary<string, object> _named;
        private readonly System.Collections.Generic.IDictionary<object, object> _locatedViews;
        private readonly ObservableCollection<object> _internalViews;
        private readonly IViewLocator _viewLocator;

        public Zone(IViewLocator viewLocator)
        {
            this._viewLocator = viewLocator;
            this._named = new Dictionary<string, object>();
            this._locatedViews = new Dictionary<object, object>();
            this._internalViews = new ObservableCollection<object>();
            this._internalViews.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(_internalViews_CollectionChanged);
        }

        void _internalViews_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (null != this.ViewCollectionChanged)
            {
                this.ViewCollectionChanged(this, e);
            }
        }

        public event EventHandler ViewCollectionChanged;

        public string ZoneName { get; set; }

        public IEnumerable<object> Views
        {
            get
            {
                List<object> views = new List<object>();
                using (IEnumerator<object> enumerator = this._internalViews.GetEnumerator())
                {
                    while (enumerator.MoveNext())
                    {
                        Func<KeyValuePair<object, object>, bool> predicate = null;
                        object item = enumerator.Current;
                        if (predicate == null)
                        {
                            predicate = x => x.Value == item;
                        }
                        KeyValuePair<object, object> pair = this._locatedViews.FirstOrDefault<KeyValuePair<object, object>>(predicate);
                        if (null != pair.Key)
                        {
                            views.Add(pair.Key);
                        }
                    }
                }
                return new ReadOnlyCollection<object>(views);
            }
        }

        internal ObservableCollection<object> InternalViews
        {
            get { return _internalViews; }
        }

        public void Add(object view)
        {
            this.Add(null, view);
        }

        public void Add(string viewName, object view)
        {
            if (null == view)
            {
                throw new System.ArgumentNullException("The view cannot be null.");
            }
            if (null != this._internalViews.FirstOrDefault((object x) => x == view))
            {
                throw new System.InvalidOperationException("The view already exists in zone.");
            }
            if (!string.IsNullOrWhiteSpace(viewName) && this._named.Any((System.Collections.Generic.KeyValuePair<string, object> x) => x.Key == viewName))
            {
                throw new System.InvalidOperationException(string.Format(System.Globalization.CultureInfo.InvariantCulture, "The view with name '{0}' already exists in the zone.", new object[] { viewName }));
            }
            if (!string.IsNullOrWhiteSpace(viewName))
            {
                this._named.Add(viewName, view);
            }
            object located = this._viewLocator.Locate(view);
            if (located is FrameworkElement)
            {
                (located as FrameworkElement).DataContext = view;
            }
            else
            {
                located = view;
            }
            this._locatedViews.Add(view, located);
            this._internalViews.Add(located);
        }

        public bool Contains(object view)
        {
            return this._locatedViews.ContainsKey(view);
        }

        public bool Contains(string viewName)
        {
            return this._named.ContainsKey(viewName);
        }

        public object GetView(string viewName)
        {
            object result;
            if (this._named.ContainsKey(viewName))
            {
                result = this._named[viewName];
            }
            else
            {
                result = null;
            }
            return result;
        }

        public void Remove(object view)
        {
            Func<KeyValuePair<string, object>, bool> predicate = null;
            if (this.Contains(view))
            {
                object item = this._locatedViews[view];
                this._locatedViews.Remove(view);
                this._internalViews.Remove(item);
                if (predicate == null)
                {
                    predicate = x => x.Value == view;
                }
                KeyValuePair<string, object> pair = this._named.SingleOrDefault<KeyValuePair<string, object>>(predicate);
                if (null != pair.Key)
                {
                    this._named.Remove(pair.Key);
                }
            }
        }

        public void Remove(string viewName)
        {
            
            if (this.Contains(viewName))
            {
                object view = this._named[viewName];
                object located = this._locatedViews[view];
                this._locatedViews.Remove(view);
                this._internalViews.Remove(located);
                this._named.Remove(viewName);
            }
        }

        public void RemoveAll()
        {
            this._named.Clear();
            this._locatedViews.Clear();
            _internalViews.Clear();
        }
    }
}
