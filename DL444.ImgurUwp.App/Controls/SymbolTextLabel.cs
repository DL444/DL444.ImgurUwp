using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;

// The Templated Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234235

namespace DL444.ImgurUwp.App.Controls
{
    public sealed class SymbolTextLabel : ContentControl
    {
        public SymbolTextLabel()
        {
            this.DefaultStyleKey = typeof(SymbolTextLabel);
        }

        public static DependencyProperty IconProperty = DependencyProperty.Register("Icon", typeof(IconElement), typeof(SymbolTextLabel), null);
        public IconElement Icon
        {
            get => GetValue(IconProperty) as IconElement;
            set => SetValue(IconProperty, value);
        }
    }
}
