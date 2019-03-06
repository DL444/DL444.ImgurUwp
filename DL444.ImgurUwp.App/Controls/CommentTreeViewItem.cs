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
    public sealed class CommentTreeViewItem : Microsoft.UI.Xaml.Controls.TreeViewItem
    {
        public CommentTreeViewItem()
        {
            this.DefaultStyleKey = typeof(CommentTreeViewItem);
        }
        protected override Size ArrangeOverride(Size finalSize)
        {
            if ((this.ItemsSource as IEnumerable<object>).Count() == 0)
            {
                ChevronVisibility = Visibility.Collapsed;
            }
            else
            {
                ChevronVisibility = Visibility.Visible;
            }
            return base.ArrangeOverride(finalSize);
        }

        public static readonly DependencyProperty ChevronVisibilityProperty = DependencyProperty.Register(nameof(ChevronVisibility), 
            typeof(Visibility), typeof(CommentTreeViewItem), new PropertyMetadata(Visibility.Collapsed, null));
        public Visibility ChevronVisibility
        {
            get => (Visibility)GetValue(ChevronVisibilityProperty);
            private set => SetValue(ChevronVisibilityProperty, value);
        }
    }
}
