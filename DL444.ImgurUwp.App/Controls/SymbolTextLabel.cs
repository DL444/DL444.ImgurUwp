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
            this.Loaded += SymbolTextLabel_Loaded;
        }

        private void SymbolTextLabel_Loaded(object sender, RoutedEventArgs e)
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

        public static DependencyProperty IconProperty = DependencyProperty.Register("Icon", typeof(IconElement), typeof(SymbolTextLabel), new PropertyMetadata(null, Icon_Changed));
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
            SymbolTextLabel instance = sender as SymbolTextLabel;
            if (e.NewValue == null)
            {
                instance.IconOn = false;
            }
            else
            {
                instance.IconOn = true;
            }
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
