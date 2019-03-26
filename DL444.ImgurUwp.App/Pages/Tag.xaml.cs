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
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace DL444.ImgurUwp.App.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Tag : Page, INotifyPropertyChanged
    {
        public Tag()
        {
            this.InitializeComponent();
        }

        bool _pageLoading;

        TagPageViewModel PageViewModel { get; set; } = new TagPageViewModel();
        bool PageLoading
        {
            get => _pageLoading;
            set
            {
                _pageLoading = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(PageLoading)));
            }
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            if(e.Parameter is TagViewModel vm)
            {
                var cache = ViewModelManager.GetViewModel<TagPageViewModel>(nameof(TagPageViewModel));
                if (cache != null && cache.ViewModel.Name == vm.Name)
                {
                    PageViewModel = cache;
                    Bindings.Update();
                    FollowBtn.IsEnabled = true;
                }
                else
                {
                    PageLoading = true;
                    PageViewModel = await TagPageViewModel.CreateFromTag(vm);
                    Bindings.Update();
                    FollowBtn.IsEnabled = true;
                    PageLoading = false;
                    ViewModelManager.AddOrUpdateViewModel(nameof(TagPageViewModel), PageViewModel);
                }
            }
        }

        private void TagList_ItemClick(object sender, ItemClickEventArgs e)
        {
            var item = e.ClickedItem as GalleryItemViewModel;
            var source = new TagItemsSource(PageViewModel.ViewModel.Name, PageViewModel.Items, PageViewModel.Items.Source.Page);
            Navigation.Navigate(typeof(GalleryItemDetails), new GalleryItemDetailsNavigationParameter(item, source));
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
