using System;
using System.Collections.Generic;
using System.ComponentModel;
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
    public sealed partial class ImagePickerDialog : ContentDialog, INotifyPropertyChanged
    {
        public ImagePickerDialog()
        {
            this.InitializeComponent();
        }
        public ImagePickerDialog(int maxCount) : this()
        {
            MaxSelectionCount = maxCount;
        }

        internal ViewModels.AccountImagePickerViewModel ViewModel { get; private set; } = new ViewModels.AccountImagePickerViewModel();
        int MaxSelectionCount { get; } = 50;

        public string SelectCountHintText => $"{ImageGridView.SelectedItems.Count} selected";

        private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            ViewModel.UpdateSelection(from s in ImageGridView.SelectedItems select (s as ViewModels.ItemViewModel), null);
        }

        private void ImageGridView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            IsPrimaryButtonEnabled = ImageGridView.SelectedItems.Count != 0 && ImageGridView.SelectedItems.Count <= MaxSelectionCount;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectCountHintText)));
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
