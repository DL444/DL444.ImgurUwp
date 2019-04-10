using DL444.ImgurUwp.App.ViewModels;
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

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace DL444.ImgurUwp.App.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SearchResult : Page
    {
        public SearchResult()
        {
            this.InitializeComponent();
            this.Loaded += SearchResult_Loaded;
        }

        private async void SearchResult_Loaded(object sender, RoutedEventArgs e)
        {
            await ViewModel.RecoverScrollPosition(RootList);
        }

        SearchResultViewModel ViewModel { get; set; }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            if(e.Parameter is string terms)
            {
                var cache = ViewModelCacheManager.Instance.Peek<SearchResultViewModel>();
                if(cache != null && cache.Terms == terms)
                {
                    ViewModel = cache;
                }
                else
                {
                    ViewModel = new SearchResultViewModel(terms);
                    ViewModelCacheManager.Instance.Push(ViewModel);
                }
                Bindings.Update();
            }
        }

        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            base.OnNavigatingFrom(e);
            if (e.NavigationMode == NavigationMode.Back)
            {
                ViewModelCacheManager.Instance.Pop<SearchResultViewModel>();
            }
            else
            {
                ViewModel.SetScrollPosition(RootList);
            }
        }

        private void ResultGrid_ItemClick(object sender, ItemClickEventArgs e)
        {
            var item = e.ClickedItem as GalleryItemViewModel;
            Navigation.Navigate(typeof(GalleryItemDetails), new GalleryItemDetailsNavigationParameter(item, ViewModel.Items.Source));
        }
    }
}
