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
    public sealed class SymbolTextButton : Button
    {
        public SymbolTextButton()
        {
            this.DefaultStyleKey = typeof(SymbolTextButton);
        }

        public static DependencyProperty IconProperty = DependencyProperty.Register("Icon", typeof(IconElement), typeof(SymbolTextButton), null);
        public IconElement Icon
        {
            get => GetValue(IconProperty) as IconElement;
            set => SetValue(IconProperty, value);
        }


        public static DependencyProperty HoverForegroundProperty = DependencyProperty.Register("HoverForeground", typeof(Brush), typeof(SymbolTextButton), null);
        public Brush HoverForeground
        {
            get => GetValue(HoverForegroundProperty) as Brush;
            set => SetValue(HoverForegroundProperty, value);
        }

        public static DependencyProperty PressedForegroundProperty = DependencyProperty.Register("PressedForeground", typeof(Brush), typeof(SymbolTextButton), null);
        public Brush PressedForeground
        {
            get => GetValue(PressedForegroundProperty) as Brush;
            set => SetValue(PressedForegroundProperty, value);
        }
    }
}
