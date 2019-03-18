using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;

// The Templated Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234235

namespace DL444.ImgurUwp.App.Controls
{
    public sealed class FrontpageImagePresenter : Control
    {
        public FrontpageImagePresenter()
        {
            this.DefaultStyleKey = typeof(FrontpageImagePresenter);
        }

        public static readonly DependencyProperty SourceProperty = DependencyProperty.Register(nameof(Source), typeof(ImageSource), typeof(FrontpageImagePresenter), null);
        public ImageSource Source
        {
            get => GetValue(SourceProperty) as ImageSource;
            set => SetValue(SourceProperty, value);
        }

        public static readonly DependencyProperty ImageWidthProperty = DependencyProperty.Register(nameof(ImageWidth), typeof(double), typeof(FrontpageImagePresenter), new PropertyMetadata(0d, OnImageMeasurePropertyChanged));
        public double ImageWidth
        {
            get => (double)GetValue(ImageWidthProperty);
            set => SetValue(ImageWidthProperty, value);
        }
        public static readonly DependencyProperty ImageHeightProperty = DependencyProperty.Register(nameof(ImageHeight), typeof(double), typeof(FrontpageImagePresenter), new PropertyMetadata(0d, OnImageMeasurePropertyChanged));
        public double ImageHeight
        {
            get => (double)GetValue(ImageHeightProperty);
            set => SetValue(ImageHeightProperty, value);
        }

        public static readonly DependencyProperty PlaceholderHeightProperty = DependencyProperty.Register(nameof(PlaceholderHeight), typeof(double), typeof(FrontpageImagePresenter), null);
        public double PlaceholderHeight
        {
            get => (double)GetValue(PlaceholderHeightProperty);
            private set => SetValue(PlaceholderHeightProperty, value);
        }

        public static readonly DependencyProperty IsAnimatedProperty = DependencyProperty.Register(nameof(IsAnimated), typeof(bool), typeof(FrontpageImagePresenter), new PropertyMetadata(false, OnVisualStatePropertyChanged));
        public bool IsAnimated
        {
            get => (bool)GetValue(IsAnimatedProperty);
            set => SetValue(IsAnimatedProperty, value);
        }
        public static readonly DependencyProperty ImageCountProperty = DependencyProperty.Register(nameof(ImageCount), typeof(int), typeof(FrontpageImagePresenter), new PropertyMetadata(1, OnVisualStatePropertyChanged));
        public int ImageCount
        {
            get => (int)GetValue(ImageCountProperty);
            set => SetValue(ImageCountProperty, value);
        }

        static void OnImageMeasurePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var instance = d as FrontpageImagePresenter;
            instance.SetPlaceholderHeight(instance.ActualWidth);
        }
        static void OnVisualStatePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var instance = d as FrontpageImagePresenter;
            instance.SetVisualState(instance.IsAnimated, instance.ImageCount);
        }

        void SetVisualState(bool isAnimated, int imageCount)
        {
            if(isAnimated)
            {
                VisualStateManager.GoToState(this, "Animated", false);
            }
            else
            {
                VisualStateManager.GoToState(this, "Static", false);
            }

            if(imageCount > 1)
            {
                VisualStateManager.GoToState(this, "Album", false);
            }
            else
            {
                VisualStateManager.GoToState(this, "Image", false);
            }
        }
        void SetPlaceholderHeight(double width)
        {
            if (ImageWidth == 0 || ImageHeight == 0) { return; }
            PlaceholderHeight = (width / ImageWidth) * ImageHeight;
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            SetPlaceholderHeight(availableSize.Width);
            return base.MeasureOverride(availableSize);
        }
    }

    public class ImageCountVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if((int)value > 1) { return Visibility.Visible; }
            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotSupportedException();
        }
    }
}
