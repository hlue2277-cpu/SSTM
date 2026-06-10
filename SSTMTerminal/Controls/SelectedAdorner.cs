using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace SSTMTerminal.Controls
{
    public class SelectedAdorner : Adorner
    {
        public static Brush PathBrush = new SolidColorBrush(Colors.Gray);
        public const double PathThickness = 1;


        private static SelectedAdorner _currentAdorner;

        public static SelectedAdorner CurrentAdorner
        {
            get { return _currentAdorner; }
            set { _currentAdorner = value; }
        }

        private double amend = 42;
        private readonly FrameworkElement _targetContent;
        private readonly VisualCollection _collection;
        private Border _contentBorder;
        private readonly Control _selectedSource;
        private double _width;
        private double _height;
        private Point _point;

        public Canvas AdornerPanel { get; set; }

        private void DrawPath()
        {
            // Popup content
            Border border = new Border();
            border.BorderThickness = new Thickness(0);

            AdornerPanel.Children.Add(border);
            _contentBorder = border;
            _collection.Add(AdornerPanel);
        }

        public SelectedAdorner(Control adornedElement, FrameworkElement targetContent, Point point, double width, double height) : base(adornedElement)
        {
            _collection = new VisualCollection(this);
            AdornerPanel = new Canvas();
            _selectedSource = adornedElement;
            _targetContent = targetContent;
            _width = width;
            _height = height;
            _point = point;
            DrawPath();
        }
        protected override Size ArrangeOverride(Size finalSize)
        {
            AdornerPanel.Arrange(new Rect(0, 0, AdornerPanel.ActualWidth, AdornerPanel.ActualHeight));
            return finalSize;
        }

        protected override void OnRender(DrawingContext drawingContext)
        {

            Canvas.SetTop(_contentBorder, _width);
            Canvas.SetLeft(_contentBorder, _height);
            PathGeometry border = new PathGeometry();
            border.Figures.Add(new PathFigure(new Point(_selectedSource.ActualWidth, 0),
                new List<PathSegment> {
                    new LineSegment(new Point(20, 0),true),
                    new ArcSegment(new Point(0,20),new Size(20,20),0,false, SweepDirection.Counterclockwise,true),
                    new LineSegment(new Point(0, _selectedSource.ActualHeight-20),true),
                    new ArcSegment(new Point(20,_selectedSource.ActualHeight),new Size(20,20),0,false, SweepDirection.Counterclockwise,true),
                    new LineSegment(new Point(_selectedSource.ActualWidth, _selectedSource.ActualHeight),true),
                    new LineSegment(new Point(_selectedSource.ActualWidth, _height+ _point.Y - amend),true),
                    new LineSegment(new Point(_selectedSource.ActualWidth+_width, _height+ _point.Y - amend),true),
                    new LineSegment(new Point(_selectedSource.ActualWidth+_width, _point.Y),true),
                    new LineSegment(new Point(_selectedSource.ActualWidth, _point.Y),true),
            }, true));
            drawingContext.DrawGeometry(null, new Pen
            {
                Brush = PathBrush,
                Thickness = PathThickness
            }, border);


            base.OnRender(drawingContext);
        }
    }
}
