using SSTMTerminal.Enums;
using Genesis.Commands;
using System.Collections;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;

namespace SSTMTerminal.Controls
{
    /// <summary>
    /// Interaction logic for PagableListBox.xaml
    /// </summary>
    public class PagableListBox : ListBox
    {
        private ScrollViewer _scrollViewer;
        private ScrollBar _verticalScrollBar;
        private ScrollBar _horizontalScrollBar;
        private WrapPanel _wrapPanel;

        #region Constuctor(s)

        static PagableListBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(PagableListBox), new FrameworkPropertyMetadata(typeof(PagableListBox)));
        }

        public PagableListBox()
        {
            this.Loaded += OnLoaded;
            this.Unloaded += OnUnloaded;

            PageChangeCommand = new DelegateCommand<object>(OnPageChanged);
        }

        #endregion Constuctor(s)

        #region Properties

        public int CurrentPageIndex
        {
            get { return (int)GetValue(CurrentPageIndexProperty); }
            set { SetValue(CurrentPageIndexProperty, value); }
        }

        public int TotalPage
        {
            get { return (int)GetValue(TotalPageProperty); }
            set { SetValue(TotalPageProperty, value); }
        }

        public ScrollModeEnum ScrollMode
        {
            get { return (ScrollModeEnum)GetValue(ScrollModeProperty); }
            set { SetValue(ScrollModeProperty, value); }
        }

        public ICommand PageChangeCommand
        {
            get { return (ICommand)GetValue(PageChangeCommandProperty); }
            set { SetValue(PageChangeCommandProperty, value); }
        }

        public bool IsPagerNavigatorVisible
        {
            get { return (bool)GetValue(IsPagerNavigatorVisibleProperty); }
            set { SetValue(IsPagerNavigatorVisibleProperty, value); }
        }

        public Thickness PagerNavigatorMargin
        {
            get { return (Thickness)GetValue(PagerNavigatorMarginProperty); }
            set { SetValue(PagerNavigatorMarginProperty, value); }
        }

        public bool IsGoToNextPage
        {
            get { return (bool)GetValue(IsGoToNextPageProperty); }
            set { SetValue(IsGoToNextPageProperty, value); }
        }

        #endregion Properties

        #region Dependency Properties

        /// <summary>
        /// Identifies the <see cref="IsGoToNextPage"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsGoToNextPageProperty =
            DependencyProperty.Register("IsGoToNextPage", typeof(bool), typeof(PagableListBox), new UIPropertyMetadata(false, GoToNextPageChanged));

        /// <summary>
        /// Identifies the <see cref="CurrentPageIndex"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty CurrentPageIndexProperty =
            DependencyProperty.Register("CurrentPageIndex", typeof(int), typeof(PagableListBox), new UIPropertyMetadata(1));

        /// <summary>
        /// Identifies the <see cref="TotalPage"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty TotalPageProperty =
            DependencyProperty.Register("TotalPage", typeof(int), typeof(PagableListBox), new UIPropertyMetadata(1, OnTotalPageChanged));

        // Using a DependencyProperty as the backing store for ScrollMode
        public static readonly DependencyProperty ScrollModeProperty =
            DependencyProperty.Register("ScrollMode", typeof(ScrollModeEnum), typeof(PagableListBox), new UIPropertyMetadata(ScrollModeEnum.CUSTOM_CODE));

        /// <summary>
        /// Identifies the <see cref="PageChangeCommand"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty PageChangeCommandProperty =
            DependencyProperty.Register("PageChangeCommand", typeof(ICommand), typeof(PagableListBox));

        // Using a DependencyProperty as the backing store for IsPagerNavigatorVisible.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsPagerNavigatorVisibleProperty =
            DependencyProperty.Register("IsPagerNavigatorVisible", typeof(bool), typeof(PagableListBox), new UIPropertyMetadata(false));

        // Using a DependencyProperty as the backing store for PagerNavigatorMargin.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PagerNavigatorMarginProperty =
            DependencyProperty.Register("PagerNavigatorMargin", typeof(Thickness), typeof(PagableListBox), new UIPropertyMetadata(new Thickness(0, 0, 0, 0)));

        #endregion Dependency Properties

        #region Private Method

        private static void OnTotalPageChanged(DependencyObject dp, DependencyPropertyChangedEventArgs e)
        {
            if (dp is PagableListBox listBox && e.NewValue is int totalCount)
            {
                listBox.IsPagerNavigatorVisible = totalCount > 1;
            }
        }

        private static void GoToNextPageChanged(DependencyObject dp, DependencyPropertyChangedEventArgs e)
        {
            PagableListBox pagableListBox = dp as PagableListBox;

            if (pagableListBox != null)
            {
                if ((bool)e.NewValue)
                {
                    if (pagableListBox.ScrollMode == ScrollModeEnum.VERTICAL)
                    {
                        ScrollBar.PageDownCommand.Execute(null, pagableListBox._verticalScrollBar);
                    }
                    else
                    {
                        ScrollBar.PageRightCommand.Execute(null, pagableListBox._horizontalScrollBar);
                    }
                }
            }
        }

        private void OnPageChanged(object payload)
        {
            RoutedPropertyChangedEventArgs<double> e = payload as RoutedPropertyChangedEventArgs<double>;

            if (e == null)
            {
                return;
            }

            double offset = e.NewValue;

            CalcuateCurrentPage(offset);

            if (this.ScrollMode == ScrollModeEnum.VERTICAL)
            {
                _scrollViewer.ScrollToVerticalOffset((offset % _scrollViewer.ViewportHeight != 0 && e.NewValue > e.OldValue ? CurrentPageIndex : (CurrentPageIndex - 1)) * _scrollViewer.ViewportHeight);
            }
            else
            {
                _scrollViewer.ScrollToHorizontalOffset((offset % _scrollViewer.ViewportWidth != 0 ? CurrentPageIndex : (CurrentPageIndex - 1)) * _scrollViewer.ViewportWidth);
            }

            if (IsGoToNextPage == false)
            {
                //SelectableHelper.SelectedBlock = null;
            }
            else
            {
                IsGoToNextPage = false;
            }
        }

        /// <summary>
        /// Cleans up the event handler for the old items source and add handler to new items source.
        /// </summary>
        protected override void OnItemsSourceChanged(IEnumerable oldValue, IEnumerable newValue)
        {
            base.OnItemsSourceChanged(oldValue, newValue);

            this.InvalidateArrange();
            this.UpdateLayout();

            if (oldValue is INotifyCollectionChanged)
            {
                INotifyCollectionChanged instance = oldValue as INotifyCollectionChanged;
                instance.CollectionChanged -= OnItemsSourceCollectionChanged;
            }

            UpdatePagnation(true, true);

            bool needCheckUpdate = false;
            if (_wrapPanel != null && _scrollViewer != null)
            {
                switch (ScrollMode)
                {
                    case ScrollModeEnum.CUSTOM_CODE:
                    case ScrollModeEnum.VERTICAL:
                        needCheckUpdate = _wrapPanel.Height % _scrollViewer.ViewportHeight != 0;
                        break;
                    case ScrollModeEnum.HORIZONTAL:
                        needCheckUpdate = _wrapPanel.Width % _scrollViewer.ViewportWidth != 0;
                        break;
                    default:
                        break;
                }

            }

            if (needCheckUpdate)
            {
                this.InvalidateArrange();
                this.UpdateLayout();
            }
        }

        /// <summary>
        /// Handles the collection of current items source to upate the pagnation.
        /// </summary>
        private void OnItemsSourceCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            UpdatePagnation(false, true);
        }

        private void UpdatePagnation(bool isUpdateHandler, bool isUpdateTotalSize)
        {
            if (null == ItemsSource)
            {
                return;
            }

            IEnumerable source = ItemsSource;

            if (isUpdateHandler && (source is INotifyCollectionChanged))
            {
                INotifyCollectionChanged instance = source as INotifyCollectionChanged;
                instance.CollectionChanged += OnItemsSourceCollectionChanged;
            }

            if (isUpdateTotalSize && _scrollViewer != null)
            {
                CalcuatePageTotal();
            }
        }

        private void CalcuatePageTotal()
        {
            if (this.ScrollMode == ScrollModeEnum.VERTICAL)
            {
                if (_scrollViewer != null && _scrollViewer.ExtentHeight != 0)
                {
                    TotalPage = (int)(_scrollViewer.ExtentHeight / _scrollViewer.ViewportHeight);

                    if (_scrollViewer.ExtentHeight % _scrollViewer.ViewportHeight != 0)
                    {
                        TotalPage += 1;
                    }
                }
                else
                {
                    TotalPage = 1;
                }
            }
            else
            {
                if (_scrollViewer != null && _scrollViewer.ExtentWidth != 0)
                {
                    TotalPage = (int)(_scrollViewer.ExtentWidth / _scrollViewer.ViewportWidth);

                    if (_scrollViewer.ExtentWidth % _scrollViewer.ViewportWidth != 0)
                    {
                        TotalPage += 1;
                    }
                }
                else
                {
                    TotalPage = 1;
                }
            }
        }

        private void CalcuateCurrentPage(double offset)
        {
            if (this.ScrollMode == ScrollModeEnum.VERTICAL)
            {
                CurrentPageIndex = (int)(offset / _scrollViewer.ViewportHeight) + 1;
            }
            else
            {
                CurrentPageIndex = (int)(offset / _scrollViewer.ViewportWidth) + 1;
            }
        }

        protected override Size ArrangeOverride(Size arrangeBounds)
        {
            if (_wrapPanel != null)
            {
                Size size = new Size(0, 0);
                var children = _wrapPanel.Children;
                if (children != null && children.Count > 0)
                {
                    foreach (var item in children)
                    {
                        if (item is ListBoxItem listBoxItem)
                        {
                            switch (ScrollMode)
                            {
                                case ScrollModeEnum.CUSTOM_CODE:
                                case ScrollModeEnum.VERTICAL:
                                    size.Height += listBoxItem.DesiredSize.Height;
                                    break;
                                case ScrollModeEnum.HORIZONTAL:
                                    size.Width += listBoxItem.DesiredSize.Width;
                                    break;
                                default:
                                    break;
                            }
                        }
                    }
                }

                if (_scrollViewer != null)
                {
                    switch (ScrollMode)
                    {
                        case ScrollModeEnum.CUSTOM_CODE:
                        case ScrollModeEnum.VERTICAL:
                            int pageCountVertical = (int)(size.Height / _scrollViewer.ViewportHeight);
                            if (size.Height % _scrollViewer.ViewportHeight != 0)
                            {
                                pageCountVertical += 1;
                            }

                            size.Height = pageCountVertical * _scrollViewer.ViewportHeight;
                            break;
                        case ScrollModeEnum.HORIZONTAL:
                            int pageCountHorizontal = (int)(size.Width / _scrollViewer.ViewportWidth);
                            if (size.Width % _scrollViewer.ViewportWidth != 0)
                            {
                                pageCountHorizontal += 1;
                            }

                            size.Width = pageCountHorizontal * _scrollViewer.ViewportWidth;
                            break;
                        default:
                            break;
                    }
                }

                if (size.Width != 0)
                {
                    _wrapPanel.Width = size.Width;
                }

                if (size.Height != 0)
                {
                    _wrapPanel.Height = size.Height;
                }
            }
            return base.ArrangeOverride(arrangeBounds);
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            _scrollViewer = FindChild<ScrollViewer>(this);
            _horizontalScrollBar = FindChild<ScrollBar>(this, "PART_HorizontalScrollBar");
            _verticalScrollBar = FindChild<ScrollBar>(this, "PART_VerticalScrollBar");
            _wrapPanel = FindChild<WrapPanel>(this);

            CalcuatePageTotal();

            if (_wrapPanel != null)
            {
                if (ScrollMode == ScrollModeEnum.HORIZONTAL)
                {
                    _wrapPanel.Width = _scrollViewer.ViewportWidth * TotalPage;
                }
                else
                {
                    _wrapPanel.Height = _scrollViewer.ViewportHeight * TotalPage;
                }
            }
        }

        private void OnUnloaded(object sender, RoutedEventArgs e)
        {
            this.Loaded -= OnLoaded;
            this.Unloaded -= OnUnloaded;

            INotifyCollectionChanged instance = ItemsSource as INotifyCollectionChanged;
            if (instance != null)
            {
                instance.CollectionChanged -= OnItemsSourceCollectionChanged;
            }
        }

        #endregion Private Method

        #region Private Static Method

        /// <summary>
        /// Updates the pagnation when page size is changed.
        /// </summary>
        private static void OnPageSizeChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            PagableListBox listBox = sender as PagableListBox;

            if (null != listBox)
            {
                listBox.UpdatePagnation(false, false);
            }
        }

        public static T FindChild<T>(DependencyObject parent) where T : DependencyObject
        {
            return FindChild<T>(parent, string.Empty);
        }

        public static T FindChild<T>(DependencyObject parent, string childName) where T : DependencyObject
        {
            if (parent == null) return null;

            T foundChild = null;

            int childrenCount = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < childrenCount; i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                T childType = child as T;
                if (childType == null)
                {
                    foundChild = FindChild<T>(child, childName);

                    if (foundChild != null) break;
                }
                else if (!string.IsNullOrEmpty(childName))
                {
                    var frameworkElement = child as FrameworkElement;
                    if (frameworkElement != null && frameworkElement.Name == childName)
                    {
                        foundChild = (T)child;
                        break;
                    }
                }
                else
                {
                    foundChild = (T)child;
                    break;
                }
            }

            return foundChild;
        }

        #endregion Private Static Method
    }
}