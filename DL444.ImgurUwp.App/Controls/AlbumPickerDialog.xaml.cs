using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Content Dialog item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace DL444.ImgurUwp.App.Controls
{
    public sealed partial class AlbumPickerDialog : ContentDialog
    {
        internal ViewModels.AccountAlbumPickerViewModel ViewModel { get; private set; } = new ViewModels.AccountAlbumPickerViewModel();
        Models.Image Image { get; set; }

        public AlbumPickerDialog()
        {
            this.InitializeComponent();
        }
        public AlbumPickerDialog(Models.Image image) : this() => Image = image;

        private void CreateNew_Click(object sender, RoutedEventArgs e)
        {
            Navigation.Navigate(typeof(Pages.Upload), Image);
            this.Hide();
        }
    }
}
