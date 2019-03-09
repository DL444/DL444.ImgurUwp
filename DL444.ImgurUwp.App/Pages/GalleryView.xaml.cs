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
using DL444.ImgurUwp.App.ViewModels;
using System.ComponentModel;
using DL444.ImgurUwp.ApiClient;
using Microsoft.Toolkit.Uwp;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace DL444.ImgurUwp.App.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class GalleryView : Page, INotifyPropertyChanged
    {
        private GalleryCollectionViewModel _viewModel;

        GalleryCollectionViewModel ViewModel
        {
            get => _viewModel;
            set
            {
                _viewModel = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ViewModel)));
            }
        }


        IncrementalLoadingCollection<GalleryItemsSource, GalleryItemViewModel> ViewModel_New { get; set; }
            = new IncrementalLoadingCollection<GalleryItemsSource, GalleryItemViewModel>();

        private DisplayParams.Section _section;
        public DisplayParams.Section Section
        {
            get => _section;
            set
            {
                if(value != _section)
                {
                    _section = value;
                    // TODO: Request items.
                }
            }
        }
        private DisplayParams.Sort _sort;
        public DisplayParams.Sort Sort
        {
            get => _sort;
            set
            {
                if(value != _sort)
                {
                    _sort = value;
                    // TODO: Request items.
                }
            }
        }

        public GalleryView()
        {
            this.InitializeComponent();
            // Universal Windows Platform does not allow custom RoutedEvent. So have to use code-behind.
            FrontpageLayout.ItemClicked += FrontpageLayout_ItemClicked;
            this.NavigationCacheMode = NavigationCacheMode.Enabled;
        }

        private void FrontpageLayout_ItemClicked(object sender, RoutedEventArgs e)
        {
            GalleryItemViewModel item = (sender as Button).Tag as GalleryItemViewModel;
            Navigation.Navigate(typeof(GalleryItemDetails), item);
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (e.Parameter is DisplayParams.Section sect && ViewModel == null)
            {
                var galleryItems = await ApiClient.Client.GetGalleryItemsAsync(DisplayParams.Sort.Viral, 0, sect);
                ViewModel = new GalleryCollectionViewModel(galleryItems);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
