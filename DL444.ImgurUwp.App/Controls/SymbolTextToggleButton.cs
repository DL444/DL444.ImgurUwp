using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;

// The Templated Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234235

namespace DL444.ImgurUwp.App.Controls
{
    public sealed class SymbolTextToggleButton : ToggleButton
    {
        public SymbolTextToggleButton()
        {
            this.DefaultStyleKey = typeof(SymbolTextToggleButton);
        }

        public static DependencyProperty IconProperty = DependencyProperty.Register("Icon", typeof(IconElement), typeof(SymbolTextToggleButton), null);
        public IconElement Icon
        {
            get => GetValue(IconProperty) as IconElement;
            set => SetValue(IconProperty, value);
        }

        public static DependencyProperty HoverForegroundProperty = DependencyProperty.Register("HoverForeground", typeof(Brush), typeof(SymbolTextToggleButton), null);
        public Brush HoverForeground
        {
            get => GetValue(HoverForegroundProperty) as Brush;
            set => SetValue(HoverForegroundProperty, value);
        }

        public static DependencyProperty PressedForegroundProperty = DependencyProperty.Register("PressedForeground", typeof(Brush), typeof(SymbolTextToggleButton), null);
        public Brush PressedForeground
        {
            get => GetValue(PressedForegroundProperty) as Brush;
            set => SetValue(PressedForegroundProperty, value);
        }

        public static DependencyProperty CheckedForegroundProperty = DependencyProperty.Register("CheckedForeground", typeof(Brush), typeof(SymbolTextToggleButton), null);
        public Brush CheckedForeground
        {
            get => GetValue(CheckedForegroundProperty) as Brush;
            set => SetValue(CheckedForegroundProperty, value);
        }

        public static DependencyProperty CheckedHoverForegroundProperty = DependencyProperty.Register("CheckedHoverForeground", typeof(Brush), typeof(SymbolTextToggleButton), null);
        public Brush CheckedHoverForeground
        {
            get => GetValue(CheckedHoverForegroundProperty) as Brush;
            set => SetValue(CheckedHoverForegroundProperty, value);
        }
    }
}
