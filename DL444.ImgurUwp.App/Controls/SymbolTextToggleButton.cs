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
            this.Loaded += SymbolTextToggleButton_Loaded;
        }
        private void SymbolTextToggleButton_Loaded(object sender, RoutedEventArgs e)
        {
            if (Icon == null)
            {
                IconOn = false;
            }
            else
            {
                IconOn = true;
            }
        }

        public static DependencyProperty IconProperty = DependencyProperty.Register("Icon", typeof(IconElement), typeof(SymbolTextToggleButton), new PropertyMetadata(null, Icon_Changed));
        public IconElement Icon
        {
            get => GetValue(IconProperty) as IconElement;
            set => SetValue(IconProperty, value);
        }

        private bool _iconOn;
        bool IconOn
        {
            get => _iconOn;
            set
            {
                _iconOn = value;
                SetVisualState(IconOn);
            }
        }

        static void Icon_Changed(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            SymbolTextToggleButton instance = sender as SymbolTextToggleButton;
            if (e.NewValue == null)
            {
                instance.IconOn = false;
            }
            else
            {
                instance.IconOn = true;
            }
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

        void SetVisualState(bool isIconOn)
        {
            if (isIconOn)
            {
                VisualStateManager.GoToState(this, "IconOn", false);
            }
            else
            {
                VisualStateManager.GoToState(this, "IconOff", false);
            }
        }
    }
}
