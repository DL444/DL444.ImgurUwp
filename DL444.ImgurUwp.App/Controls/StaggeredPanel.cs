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

        protected override void UninitializeForContextCore(VirtualizingLayoutContext context)
        {
            base.UninitializeForContextCore(context);
            context.LayoutState = null;
        }

        protected override Size MeasureOverride(VirtualizingLayoutContext context, Size availableSize)
        {
            availableSize.Width = availableSize.Width/* - Padding.Left - Padding.Right*/;
            availableSize.Height = availableSize.Height/* - Padding.Top - Padding.Bottom*/;

            // For when width is even less than desired col width of even one column:
            _columnWidth = Math.Min(DesiredColumnWidth, availableSize.Width);
            int numColumns = (int)Math.Floor(availableSize.Width / _columnWidth);
            _columnWidth = availableSize.Width / numColumns;

            if(context.LayoutState == null)
            {
                context.LayoutState = new StaggeredLayoutState(numColumns, _columnWidth);
            }
            var state = context.LayoutState as StaggeredLayoutState;

            if(context.ItemCount == 0) { return new Size(availableSize.Width, 0); }

            double viewUpperBound = context.RealizationRect.Top;
            double viewLowerBound = context.RealizationRect.Bottom;

            int firstIndex = state.GetFirstItemInView(viewUpperBound);
            int lastIndex = state.GetLastItemInView(viewLowerBound);
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
                child.Arrange(rect);
                index++;
            }

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

            public StaggeredLayoutState(int columnCount, double columnWidth)
            {
                ColumnCount = columnCount;
                height = new double[columnCount];
                _columnWidth = columnWidth;
            }
        }
    }

    // See GitHub: Microsoft/UI-Xaml-Gallery
    public class PinterestLayout : VirtualizingLayout
    {
        public PinterestLayout()
        {
            Width = 250.0;
        }

        public double Width { get; set; }

        protected override Size MeasureOverride(VirtualizingLayoutContext context, Size availableSize)
        {
            var viewport = context.RealizationRect;
            System.Diagnostics.Debug.WriteLine("Measure: " + viewport);

            if (availableSize.Width != m_lastAvailableWidth)
            {
                UpdateCachedBounds(availableSize);
                m_lastAvailableWidth = availableSize.Width;
            }

            // Initialize column offsets
            int numColumns = (int)(availableSize.Width / Width);
            if (m_columnOffsets.Count == 0)
            {
                for (int i = 0; i < numColumns; i++)
                {
                    m_columnOffsets.Add(0);
                }
            }

            double horizontalOffset = (availableSize.Width - numColumns * Width) / 2;

            m_firstIndex = GetStartIndex(viewport);
            int currentIndex = m_firstIndex;
            double nextOffset = -1.0;

            // Measure items from start index to when we hit the end of the viewport.
            while (currentIndex < context.ItemCount && nextOffset < viewport.Bottom)
            {
                System.Diagnostics.Debug.WriteLine("Measuring " + currentIndex);
                var child = context.GetOrCreateElementAt(currentIndex);
                child.Measure(new Size(Width, availableSize.Height));

                if (currentIndex >= m_cachedBounds.Count)
                {
                    // We do not have bounds for this index. Lay it out and cache it.
                    int columnIndex = GetIndexOfLowestColumn(m_columnOffsets, out nextOffset);
                    m_cachedBounds.Add(new Rect(columnIndex * Width + horizontalOffset, nextOffset, Width, child.DesiredSize.Height));
                    m_columnOffsets[columnIndex] += child.DesiredSize.Height;
                }
                else
                {
                    if (currentIndex + 1 == m_cachedBounds.Count)
                    {
                        // Last element. Use the next offset.
                        GetIndexOfLowestColumn(m_columnOffsets, out nextOffset);
                    }
                    else
                    {
                        nextOffset = m_cachedBounds[currentIndex + 1].Top;
                    }
                }

                child.Arrange(m_cachedBounds[currentIndex]);

                m_lastIndex = currentIndex;
                currentIndex++;
            }

            var extent = GetExtentSize(availableSize);
            return extent;
        }

        // The children are arranged during measure, so ArrangeOverride can be a no-op.
        //protected override Size ArrangeOverride(VirtualizingLayoutContext context, Size finalSize)
        //{
        //    Debug.WriteLine("Arrange: " + context.RealizationRect);
        //    for (int index = m_firstIndex; index <= m_lastIndex; index++)
        //    {
        //        Debug.WriteLine("Arranging " + index);
        //        var child = context.GetElementAt(index);
        //        child.Arrange(m_cachedBounds[index]);
        //    }
        //    return finalSize;
        //}

        private void UpdateCachedBounds(Size availableSize)
        {
            int numColumns = (int)(availableSize.Width / Width);
            m_columnOffsets.Clear();
            for (int i = 0; i < numColumns; i++)
            {
                m_columnOffsets.Add(0);
            }

            double horizontalOffset = (availableSize.Width - numColumns * Width) / 2;

            for (int index = 0; index < m_cachedBounds.Count; index++)
            {
                double nextOffset = 0.0;
                int columnIndex = GetIndexOfLowestColumn(m_columnOffsets, out nextOffset);
                var oldHeight = m_cachedBounds[index].Height;
                m_cachedBounds[index] = new Rect(columnIndex * Width + horizontalOffset, nextOffset, Width, oldHeight);
                m_columnOffsets[columnIndex] += oldHeight;
            }
        }

        private int GetStartIndex(Rect viewport)
        {
            int startIndex = 0;
            if (m_cachedBounds.Count == 0)
            {
                startIndex = 0;
            }
            else
            {
                // find first index that intersects the viewport
                // perhaps this can be done more efficiently than walking
                // from the start of the list.
                for (int i = 0; i < m_cachedBounds.Count; i++)
                {
                    var currentBounds = m_cachedBounds[i];
                    if (currentBounds.Y < viewport.Bottom &&
                        currentBounds.Bottom > viewport.Top)
                    {
                        startIndex = i;
                        break;
                    }
                }
            }

            return startIndex;
        }

        private int GetIndexOfLowestColumn(List<double> columnOffsets, out double lowestOffset)
        {
            int lowestIndex = 0;
            lowestOffset = columnOffsets[lowestIndex];
            for (int index = 0; index < columnOffsets.Count; index++)
            {
                var currentOffset = columnOffsets[index];
                if (lowestOffset > currentOffset)
                {
                    lowestOffset = currentOffset;
                    lowestIndex = index;
                }
            }

            return lowestIndex;
        }

        private Size GetExtentSize(Size availableSize)
        {
            double largestColumnOffset = m_columnOffsets[0];
            for (int index = 0; index < m_columnOffsets.Count; index++)
            {
                var currentOffset = m_columnOffsets[index];
                if (largestColumnOffset < currentOffset)
                {
                    largestColumnOffset = currentOffset;
                }
            }

            return new Size(availableSize.Width, largestColumnOffset);
        }

        int m_firstIndex = 0;
        int m_lastIndex = 0;
        double m_lastAvailableWidth = 0.0;
        List<double> m_columnOffsets = new List<double>();
        List<Rect> m_cachedBounds = new List<Rect>();
    }
}
