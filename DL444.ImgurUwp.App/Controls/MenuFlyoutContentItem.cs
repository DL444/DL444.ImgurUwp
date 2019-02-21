using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The Templated Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234235

namespace DL444.ImgurUwp.App.Controls
{
    public sealed class MenuFlyoutContentItem : MenuFlyoutItem
    {
        public MenuFlyoutContentItem()
        {
            this.DefaultStyleKey = typeof(MenuFlyoutContentItem);
        }

        public UIElement Content
        {
            get => GetValue(ContentProperty) as UIElement;
            set => SetValue(ContentProperty, value);
        }

        public static readonly DependencyProperty ContentProperty =
            DependencyProperty.Register("Content", typeof(UIElement), typeof(MenuFlyoutContentItem), null);
    }
}
