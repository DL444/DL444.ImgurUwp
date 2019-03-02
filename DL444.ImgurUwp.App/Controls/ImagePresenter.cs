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
using Windows.UI.Xaml.Media.Imaging;

// The Templated Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234235

namespace DL444.ImgurUwp.App.Controls
{
    public sealed class ImagePresenter : Control
    {
        public ImagePresenter()
        {
            this.DefaultStyleKey = typeof(ImagePresenter);
            this.Loaded += ImagePresenter_Loaded;
            SizeChanged += ImagePresenter_SizeChanged;
        }

        private void ImagePresenter_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if(e.NewSize.Width == e.PreviousSize.Width) { return; }
            double width = e.NewSize.Width;
            PlaceholderHeight = ImageHeight * (width / ImageWidth);
        }

        private void ImagePresenter_Loaded(object sender, RoutedEventArgs e)
        {
            SetVisualState(IsAnimated, ImageCount);
        }

        public static readonly DependencyProperty ImageSourceProperty = DependencyProperty.Register("ImageSource", typeof(BitmapSource), typeof(ImagePresenter), null);
        public BitmapSource ImageSource
        {
            get => GetValue(ImageSourceProperty) as BitmapSource;
            private set => SetValue(ImageSourceProperty, value);
        }
        public static readonly DependencyProperty VideoSourceProperty = DependencyProperty.Register("VideoSource", typeof(Uri), typeof(ImagePresenter), null);
        public Uri VideoSource
        {
            get => GetValue(VideoSourceProperty) as Uri;
            private set => SetValue(VideoSourceProperty, value);
        }


        public static readonly DependencyProperty SourceUrlProperty = DependencyProperty.Register("SourceUrl", typeof(string), typeof(ImagePresenter), new PropertyMetadata(null, SourceUrlChanged));
        public string SourceUrl
        {
            get => GetValue(SourceUrlProperty) as string;
            set => SetValue(SourceUrlProperty, value);
        }
        static void SourceUrlChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            string url = e.NewValue as string;
            ImagePresenter instance = sender as ImagePresenter;
            if (string.IsNullOrWhiteSpace(url)) { instance.ImageSource = null; }
            else
            {
                instance.ImageSource = new BitmapImage(new Uri(url));
                instance.VideoSource = new Uri(url);
            }
        }

        public static readonly DependencyProperty IsAnimatedProperty = DependencyProperty.Register("IsAnimated", typeof(bool), typeof(ImagePresenter), new PropertyMetadata(false, PropertyValueChanged));
        public bool IsAnimated
        {
            get => (bool)GetValue(IsAnimatedProperty);
            set => SetValue(IsAnimatedProperty, value);
        }

        public static readonly DependencyProperty AutoplayProperty = DependencyProperty.Register("Autoplay", typeof(bool), typeof(ImagePresenter), null);
        public bool Autoplay
        {
            get => (bool)GetValue(AutoplayProperty);
            set => SetValue(AutoplayProperty, value);
        }

        // These two values are used to calculate placeholder dimensions.
        public static readonly DependencyProperty ImageHeightProperty = DependencyProperty.Register("ImageHeight", typeof(double), typeof(ImagePresenter), null);
        public double ImageHeight
        {
            get => (double)GetValue(ImageHeightProperty);
            set => SetValue(ImageHeightProperty, value);
        }
        public static readonly DependencyProperty ImageWidthProperty = DependencyProperty.Register("ImageWidth", typeof(double), typeof(ImagePresenter), null);
        public double ImageWidth
        {
            get => (double)GetValue(ImageWidthProperty);
            set => SetValue(ImageWidthProperty, value);
        }
        public static readonly DependencyProperty PlaceholderHeightProperty = DependencyProperty.Register("PlaceholderHeight", typeof(double), typeof(ImagePresenter), null);
        public double PlaceholderHeight
        {
            get => (double)GetValue(PlaceholderHeightProperty);
            private set => SetValue(PlaceholderHeightProperty, value);
        }

        public static readonly DependencyProperty ImageCountProperty = DependencyProperty.Register("ImageCount", typeof(int), typeof(ImagePresenter), new PropertyMetadata(1, PropertyValueChanged));
        public int ImageCount
        {
            get => (int)GetValue(ImageCountProperty);
            set => SetValue(ImageCountProperty, value);
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            Size initSize = base.MeasureOverride(availableSize);
            if (initSize.Height == 0)
            {
                double width = availableSize.Width;
                double placeholderHeight = ImageHeight * (width / ImageWidth);
                initSize.Height += placeholderHeight;
                if (initSize.Height > MaxHeight)
                {
                    initSize.Height = MaxHeight;
                }
            }
            return initSize;
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            return base.ArrangeOverride(finalSize);
        }

        static void PropertyValueChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            ImagePresenter instance = sender as ImagePresenter;
            instance.SetVisualState(instance.IsAnimated, instance.ImageCount);
        }

        void SetVisualState(bool isAnimated, int imageCount)
        {
            if (isAnimated)
            {
                VisualStateManager.GoToState(this, "GifVideo", false);
            }
            else
            {
                VisualStateManager.GoToState(this, "StaticImage", false);
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
    }
}
