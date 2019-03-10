using DL444.ImgurUwp.App.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace DL444.ImgurUwp.App.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class AccountDetails : Page, INotifyPropertyChanged
    {
        private AccountViewModel _viewModel;
        private string originalBio;
        
        AccountViewModel ViewModel
        {
            get => _viewModel;
            set
            {
                _viewModel = value;
                originalBio = value.Biography;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ViewModel)));
            }
        }

        GalleryProfileViewModel Profile { get; set; }
        ObservableCollection<TrophyViewModel> Trophies { get; set; } = new ObservableCollection<TrophyViewModel>();

        string BioPlaceholderText { get; set; }

        public bool IsOwner => ViewModel == null ? false : ViewModel.Username == ApiClient.OwnerAccount;

        public AccountDetails()
        {
            this.InitializeComponent();
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            if(e.Parameter is AccountViewModel vm)
            {
                await PrepareViewModels(vm);
            }
            else if(e.Parameter is string username)
            {
                var account = await ApiClient.Client.GetAccountAsync(username);
                await PrepareViewModels(new AccountViewModel(account));
            }
        }

        async System.Threading.Tasks.Task PrepareViewModels(AccountViewModel vm)
        {
            ViewModel = vm;
            if(IsOwner) { BioPlaceholderText = "Tell Imgur a little about yourself..."; }
            else { BioPlaceholderText = ""; }
            Bindings.Update();

            var profile = await ApiClient.Client.GetAccountGalleryProfileAsync(vm.Username);
            Profile = new GalleryProfileViewModel(profile);
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Profile)));
            foreach (var t in profile.Trophies)
            {
                Trophies.Add(new TrophyViewModel(t));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void BioTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            BioAcceptBtn.Visibility = Visibility.Visible;
        }

        private async void BioTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            BioAcceptBtn.Visibility = Visibility.Collapsed;
            if(originalBio != ViewModel.Biography)
            {
                bool result = await ApiClient.Client.SetAccountBioAsync(ViewModel.Username, ViewModel.Biography);
                if(result == true)
                {
                    originalBio = ViewModel.Biography;
                }
                else
                {
                    ViewModel.Biography = originalBio;
                }
            }
        }
    }
}
