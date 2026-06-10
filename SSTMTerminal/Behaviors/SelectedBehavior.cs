using SSTMTerminal.Controls;
using SSTMTerminal.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Interactivity;
using System.Windows.Media;

namespace SSTMTerminal.Behaviors
{
    public class SelectedBehavior : Behavior<ListBox>
    {
        private SelectedAdorner _adorner;

        public UIElement PlacementTarget
        {
            get { return (UIElement)GetValue(PlacementTargetProperty); }
            set { SetValue(PlacementTargetProperty, value); }
        }

        // Using a DependencyProperty as the backing store for PlacementTarget.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PlacementTargetProperty =
            DependencyProperty.Register("PlacementTarget", typeof(UIElement), typeof(SelectedBehavior), new PropertyMetadata(null));

        private bool isFirstSelected = false;

        protected override void OnAttached()
        {
            base.OnAttached();

            AssociatedObject.Loaded += (o, e) =>
            {
                if (o is ListBox lb && isFirstSelected)
                {
                    isFirstSelected = false;

                    var lbi = lb.ItemContainerGenerator.ContainerFromIndex(lb.SelectedIndex) as ListBoxItem;
                    if (lbi != null)
                    {
                        Point relativeOffset;
                        if (PlacementTarget != null && PlacementTarget is ListBox plb)
                        {
                            GeneralTransform generalTransform = PlacementTarget.TransformToVisual(lbi);
                            relativeOffset = generalTransform.Transform(new Point(0, 0));
                            AdornerLayer layer = AdornerLayerHelper.GetAdornerLayer(AssociatedObject);

                            _adorner = new SelectedAdorner(lbi, plb, relativeOffset, plb.ActualWidth, plb.ActualHeight);

                            if (SelectedAdorner.CurrentAdorner == _adorner)
                            {
                                layer.Remove(_adorner);
                                SelectedAdorner.CurrentAdorner = null;
                            }
                            else if (SelectedAdorner.CurrentAdorner != null)
                            {
                                layer.Remove(SelectedAdorner.CurrentAdorner);
                                SelectedAdorner.CurrentAdorner = _adorner; ;
                                layer.Remove(_adorner);
                                layer.Add(_adorner);
                            }
                            else
                            {
                                SelectedAdorner.CurrentAdorner = _adorner;
                                layer.Remove(_adorner);
                                layer.Add(_adorner);
                            }
                        }
                    }
                }

            };

            AssociatedObject.SelectionChanged += (o, e) =>
            {
                if (o is ListBox lb && lb.IsLoaded)
                {
                    var lbi = lb.ItemContainerGenerator.ContainerFromIndex(lb.SelectedIndex) as ListBoxItem;
                    if (lbi != null)
                    {
                        Point relativeOffset;
                        if (PlacementTarget != null && PlacementTarget is ListBox plb)
                        {
                            GeneralTransform generalTransform = PlacementTarget.TransformToVisual(lbi);
                            relativeOffset = generalTransform.Transform(new Point(0, 0));
                            AdornerLayer layer = AdornerLayerHelper.GetAdornerLayer(AssociatedObject);
                            _adorner = new SelectedAdorner(lbi, plb, relativeOffset, plb.ActualWidth, plb.ActualHeight);

                            if (SelectedAdorner.CurrentAdorner == _adorner)
                            {
                                layer.Remove(_adorner);
                                SelectedAdorner.CurrentAdorner = null;
                            }
                            else if (SelectedAdorner.CurrentAdorner != null)
                            {
                                layer.Remove(SelectedAdorner.CurrentAdorner);
                                SelectedAdorner.CurrentAdorner = _adorner; ;
                                layer.Remove(_adorner);
                                layer.Add(_adorner);
                            }
                            else
                            {
                                SelectedAdorner.CurrentAdorner = _adorner;
                                layer.Remove(_adorner);
                                layer.Add(_adorner);
                            }
                        }
                    }
                }
                else
                {
                    isFirstSelected = true;
                }
            };
        }

        protected override void OnDetaching()
        {
            AdornerLayer layer = AdornerLayerHelper.GetAdornerLayer(AssociatedObject);

            if (SelectedAdorner.CurrentAdorner == _adorner)
            {
                layer.Remove(_adorner);
                SelectedAdorner.CurrentAdorner = null;
            }

            base.OnDetaching();
        }

    }
}
