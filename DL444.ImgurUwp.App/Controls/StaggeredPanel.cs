using System;
using System.Linq;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace DL444.ImgurUwp.App.Controls
{
    // Edited from https://github.com/windows-toolkit/WindowsCommunityToolkit/blob/master/Microsoft.Toolkit.Uwp.UI.Controls/StaggeredPanel/StaggeredPanel.cs
    // The original does not recognize the alignment of its items.
    public class StaggeredPanel : Panel
    {
        private double _columnWidth;

        public StaggeredPanel()
        {
            RegisterPropertyChangedCallback(Panel.HorizontalAlignmentProperty, OnHorizontalAlignmentChanged);
        }

        public double DesiredColumnWidth
        {
            get { return (double)GetValue(DesiredColumnWidthProperty); }
            set { SetValue(DesiredColumnWidthProperty, value); }
        }

        public static readonly DependencyProperty DesiredColumnWidthProperty = DependencyProperty.Register(nameof(DesiredColumnWidth), typeof(double), typeof(StaggeredPanel), new PropertyMetadata(250d, OnDesiredColumnWidthChanged));

        public Thickness Padding
        {
            get { return (Thickness)GetValue(PaddingProperty); }
            set { SetValue(PaddingProperty, value); }
        }

        public static readonly DependencyProperty PaddingProperty = DependencyProperty.Register(nameof(Padding), typeof(Thickness), typeof(StaggeredPanel), new PropertyMetadata(default(Thickness), OnPaddingChanged));

        protected override Size MeasureOverride(Size availableSize)
        {
            availableSize.Width = availableSize.Width - Padding.Left - Padding.Right;
            availableSize.Height = availableSize.Height - Padding.Top - Padding.Bottom;

            // For when width is even less than disired col width of even one column:
            _columnWidth = Math.Min(DesiredColumnWidth, availableSize.Width);
            int numColumns = (int)Math.Floor(availableSize.Width / _columnWidth);
            if (HorizontalAlignment == HorizontalAlignment.Stretch)
            {
                _columnWidth = availableSize.Width / numColumns;
            }

            var columnHeights = new double[numColumns];

            for (int i = 0; i < Children.Count; i++)
            {
                var columnIndex = GetColumnIndex(columnHeights);

                var child = Children[i];
                child.Measure(new Size(_columnWidth, availableSize.Height));
                var elementSize = child.DesiredSize;
                columnHeights[columnIndex] += elementSize.Height;
            }

            double desiredHeight = columnHeights.Max();

            return new Size(availableSize.Width, desiredHeight);
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            double horizontalOffset = Padding.Left;
            double verticalOffset = Padding.Top;
            int numColumns = (int)Math.Floor(finalSize.Width / _columnWidth);
            if (HorizontalAlignment == HorizontalAlignment.Right)
            {
                horizontalOffset += finalSize.Width - (numColumns * _columnWidth);
            }
            else if (HorizontalAlignment == HorizontalAlignment.Center)
            {
                horizontalOffset += (finalSize.Width - (numColumns * _columnWidth)) / 2;
            }

            var columnHeights = new double[numColumns];

            for (int i = 0; i < Children.Count; i++)
            {
                var columnIndex = GetColumnIndex(columnHeights);

                var child = Children[i];
                var elementSize = child.DesiredSize;

                double elementHeight = elementSize.Height;

                Rect bounds = new Rect(horizontalOffset + (_columnWidth * columnIndex), columnHeights[columnIndex] + verticalOffset, _columnWidth, elementHeight);
                child.Arrange(bounds);

                columnHeights[columnIndex] += elementSize.Height;
            }

            return base.ArrangeOverride(finalSize);
        }

        private static void OnDesiredColumnWidthChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var panel = (StaggeredPanel)d;
            panel.InvalidateMeasure();
        }

        private static void OnPaddingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var panel = (StaggeredPanel)d;
            panel.InvalidateMeasure();
        }

        private void OnHorizontalAlignmentChanged(DependencyObject sender, DependencyProperty dp)
        {
            InvalidateMeasure();
        }

        private int GetColumnIndex(double[] columnHeights)
        {
            int columnIndex = 0;
            double height = columnHeights[0];
            for (int j = 1; j < columnHeights.Length; j++)
            {
                if (columnHeights[j] < height)
                {
                    columnIndex = j;
                    height = columnHeights[j];
                }
            }

            return columnIndex;
        }
    }
}
