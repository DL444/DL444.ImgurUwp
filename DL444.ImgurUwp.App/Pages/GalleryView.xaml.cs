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
using DL444.ImgurUwp.ApiClient;
using System.Threading;
using System.Threading.Tasks;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace DL444.ImgurUwp.App.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class GalleryView : Page
    {
        public GalleryView()
        {
            this.InitializeComponent();
        }

        GalleryViewPageViewModel ViewModel { get; set; }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (e.Parameter is DisplayParams.Section sect)
            {
                var cache = ViewModelCacheManager.Instance.Peek<GalleryViewPageViewModel>();
                if (cache != null && cache.Section == sect)
                {
                    ViewModel = cache;
                    Bindings.Update();
                    await ViewModel.RecoverScrollPosition(RootListView);
                }
                else
                {
                    DisplayParams.Sort sort = sect == DisplayParams.Section.User ? DisplayParams.Sort.Time : DisplayParams.Sort.Viral;
                    ViewModel = new GalleryViewPageViewModel(sect, sort);
                    Bindings.Update();
                    ViewModelCacheManager.Instance.Push(ViewModel);
                }
            }
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            if (e.NavigationMode == NavigationMode.Back)
            {
                ViewModelCacheManager.Instance.Pop<GalleryViewPageViewModel>();
            }
            else
            {
                ViewModel.SetScrollPosition(RootListView);
            }
        }

        private void FrontPageGrid_ItemClick(object sender, ItemClickEventArgs e)
        {
            var item = e.ClickedItem as GalleryItemViewModel;
            GalleryIncrementalSource source = new GalleryIncrementalSource(ViewModel.Section, ViewModel.Sort, ViewModel.Items, ViewModel.Items.Source.Page);
            Navigation.Navigate(typeof(GalleryItemDetails), new GalleryItemDetailsNavigationParameter(item, source));
        }

        private void GoUpButton_Click(object sender, RoutedEventArgs e)
        {
            RootListView.ScrollIntoView(RootListView.Items.FirstOrDefault());
        }
    }
}
