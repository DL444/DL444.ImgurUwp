using DL444.ImgurUwp.App.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using System.Threading.Tasks;
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
    public sealed partial class AccountDetails : Page, INotifyPropertyChanged
    {
        AccountDetailsPageViewModel PageViewModel { get; set; }
        bool _pageLoading;

        public AccountDetails()
        {
            this.InitializeComponent();
        }

        public bool PageLoading
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
            if(e.Parameter is AccountViewModel vm)
            {
                var cache = ViewModelManager.GetViewModel<AccountDetailsPageViewModel>(nameof(AccountDetailsPageViewModel));
                if(cache != null && cache.ViewModel.Username == vm.Username)
                {
                    PageViewModel = cache;
                }
                else
                {
                    PageLoading = true;
                    PageViewModel = await AccountDetailsPageViewModel.CreateFromAccount(vm);
                    PageLoading = false;
                    ViewModelManager.AddOrUpdateViewModel(nameof(AccountDetailsPageViewModel), PageViewModel);
                }
                Bindings.Update();
            }
            else if(e.Parameter is string username)
            {
                var cache = ViewModelManager.GetViewModel<AccountDetailsPageViewModel>(nameof(AccountDetailsPageViewModel));
                if(cache != null && cache.ViewModel.Username == username)
                {
                    PageViewModel = cache;
                }
                else
                {
                    PageLoading = true;
                    PageViewModel = await AccountDetailsPageViewModel.CreateFromAccountUsername(username);
                    PageLoading = false;
                    ViewModelManager.AddOrUpdateViewModel(nameof(AccountDetailsPageViewModel), PageViewModel);
                }
                Bindings.Update();
            }
        }

        private void BioTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            BioAcceptBtn.Visibility = Visibility.Visible;
        }

        private async void BioTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            BioAcceptBtn.Visibility = Visibility.Collapsed;
            await PageViewModel.ChangeBio();
        }

        private void PostList_ItemClick(object sender, ItemClickEventArgs e)
        {
            var item = e.ClickedItem as GalleryItemViewModel;
            var source = new AccountPostSource(PageViewModel.ViewModel.Username, PageViewModel.Posts, PageViewModel.Posts.Source.Page);
            Navigation.ContentFrame.Navigate(typeof(Pages.GalleryItemDetails), new GalleryItemDetailsNavigationParameter(item, source));
        }

        private void AccountContentButton_Click(object sender, RoutedEventArgs e)
        {
            if(PageViewModel.ViewModel == null) { return; }
            Navigation.ContentFrame.Navigate(typeof(AccountContent), PageViewModel.ViewModel, new Windows.UI.Xaml.Media.Animation.DrillInNavigationTransitionInfo());
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
