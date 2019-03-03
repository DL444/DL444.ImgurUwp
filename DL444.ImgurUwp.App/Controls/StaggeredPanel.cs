using System;
using System.Linq;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls;
using System.Collections.Generic;

namespace DL444.ImgurUwp.App.Controls
{
    // This implementation is faulty. See below.
    public class StaggeredLayout : VirtualizingLayout
    {
        private double _columnWidth;
        StaggeredLayoutState state = null;

        public double DesiredColumnWidth
        {
            get { return (double)GetValue(DesiredColumnWidthProperty); }
            set { SetValue(DesiredColumnWidthProperty, value); }
        }
        public static readonly DependencyProperty DesiredColumnWidthProperty = DependencyProperty.Register(nameof(DesiredColumnWidth), typeof(double), typeof(StaggeredLayout), new PropertyMetadata(250d, OnDesiredColumnWidthChanged));

        public Thickness Padding
        {
            get { return (Thickness)GetValue(PaddingProperty); }
            set { SetValue(PaddingProperty, value); }
        }
        public static readonly DependencyProperty PaddingProperty = DependencyProperty.Register(nameof(Padding), typeof(Thickness), typeof(StaggeredLayout), new PropertyMetadata(default(Thickness), OnPaddingChanged));

        protected override Size MeasureOverride(VirtualizingLayoutContext context, Size availableSize)
        {
            availableSize.Width = availableSize.Width/* - Padding.Left - Padding.Right*/;
            availableSize.Height = availableSize.Height/* - Padding.Top - Padding.Bottom*/;

            // For when width is even less than desired col width of even one column:
            _columnWidth = Math.Min(DesiredColumnWidth, availableSize.Width);
            int numColumns = (int)Math.Floor(availableSize.Width / _columnWidth);
            _columnWidth = availableSize.Width / numColumns;

            if(state == null || state.Width != context.RealizationRect.Width)
            {
                state = new StaggeredLayoutState(numColumns, _columnWidth, context.RealizationRect.Width);
            }

            if(context.ItemCount == 0) { return new Size(availableSize.Width, 0); }

            // The viewport consists the current view, one view before, and one view after.
            double viewUpperBound = context.RealizationRect.Top;
            double viewLowerBound = context.RealizationRect.Bottom;
            System.Diagnostics.Debug.WriteLine($"Viewport: {viewUpperBound}, {viewLowerBound}");

            int firstIndex = state.GetFirstItemInView(viewUpperBound);
            int lastIndex = state.GetLastItemInView(viewLowerBound);
            System.Diagnostics.Debug.WriteLine($"Bounds: {firstIndex}, {lastIndex}");

            int index = firstIndex;

            while(state.CacheCount < context.ItemCount)
            {
                Rect rect;
                bool cached = state.TryGetRect(lastIndex, out rect);
                if(cached)
                {
                    break;
                }

                // The culpit might be auto recycling. Supressing it solves the problem, 
                // but defeats virtualization and raises runtime expections, so must fix it.

                // Okay, the real problem is the custom image control. Without that this implementation works just fine.
                // The culpit is the video control. Even using that alone produces the issue.
                // So might consider extract the first frame and put it in an image box.

                var child = context.GetOrCreateElementAt(lastIndex/*, ElementRealizationOptions.SuppressAutoRecycle*/);
                rect = state.CreateRectForChild(lastIndex, child);
                lastIndex++;
                if (rect.Top > viewLowerBound) { break; }
            }

            while(index < lastIndex)
            {
                var child = context.GetOrCreateElementAt(index);
                Rect rect;
                if (index < state.CacheCount)
                {
                    rect = state.GetRect(index);
                }
                else
                {
                    rect = state.CreateRectForChild(index, child);
                }

                // It turns out this measurement is pretty important for elements to stay within their bounds.
                child.Measure(new Size(rect.Width, rect.Height));
                child.Arrange(rect);
                index++;
            }

            // Use this to reproduce bug.

            //var firstElement = context.GetOrCreateElementAt(0);
            //context.RecycleElement(firstElement);
            //firstElement = context.GetOrCreateElementAt(0);
            //firstElement.Measure(new Size(_columnWidth, availableSize.Height));
            //Rect r = state.GetRect(0);
            //firstElement.Arrange(r);

            double desiredHeight = state.GetTotalHeight();
            return new Size(availableSize.Width, desiredHeight);
        }

        private static void OnDesiredColumnWidthChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var panel = (StaggeredLayout)d;
            panel.InvalidateMeasure();
        }

        private static void OnPaddingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var panel = (StaggeredLayout)d;
            panel.InvalidateMeasure();
        }

        internal class StaggeredLayoutState
        {
            public double Width { get; }
            // Including (returned index in view and cached)
            public int GetFirstItemInView(double viewportUpperBound)
            {
                for(int i = 0; i < CacheCount; i++)
                {
                    if (_layoutRects[i].Bottom > viewportUpperBound)
                    {
                        return i;
                    }
                }
                return 0;
            }
            // Excluding (returned index not cached or not in view)
            public int GetLastItemInView(double viewportLowerBound)
            {
                for (int i = 0; i < CacheCount; i++)
                {
                    if(_layoutRects[i].Top > viewportLowerBound)
                    {
                        return i;
                    }
                }
                return CacheCount;
            }

            public int ColumnCount { get; }
            public double ColumnWidth
            {
                get => _columnWidth;
                set => _columnWidth = value;
            }

            public int SelectColumn()
            {
                int col = 0;
                for(int i = 0; i < height.Length; i++)
                {
                    if(height[i] < height[col]) { col = i; }
                }
                return col;
            }

            public double GetTotalHeight() => height.Max();

            public int CacheCount => _layoutRects.Count;

            public Rect GetRect(int index) => _layoutRects[index];
            public bool TryGetRect(int index, out Rect rect)
            {
                return _layoutRects.TryGetValue(index, out rect);
            }

            public Rect CreateRectForChild(int index, UIElement child)
            {
                int col = SelectColumn();
                var horizontalOffset = _columnWidth * col;
                var verticalOffset = height[col];
                var w = _columnWidth;
                child.Measure(new Size(_columnWidth, double.PositiveInfinity));
                var h = child.DesiredSize.Height;
                Rect rect = new Rect(horizontalOffset, verticalOffset, w, h);
                _layoutRects.Add(index, rect);
                height[col] += h;
                return rect;
            }

            Dictionary<int, Rect> _layoutRects = new Dictionary<int, Rect>();
            public double[] height;
            private double _columnWidth;

            public StaggeredLayoutState(int columnCount, double columnWidth, double width)
            {
                ColumnCount = columnCount;
                height = new double[columnCount];
                _columnWidth = columnWidth;
                Width = width;
            }
        }
    }
}
