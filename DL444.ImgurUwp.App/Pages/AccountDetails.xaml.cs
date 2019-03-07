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
        
        AccountViewModel ViewModel
        {
            get => _viewModel;
            set
            {
                _viewModel = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ViewModel)));
            }
        }

        GalleryProfileViewModel Profile { get; set; }
        ObservableCollection<TrophyViewModel> Trophies { get; set; } = new ObservableCollection<TrophyViewModel>();

        public AccountDetails()
        {
            this.InitializeComponent();
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            if(e.Parameter is AccountViewModel vm)
            {
                ViewModel = vm;

                var profile = await ApiClient.Client.GetAccountGalleryProfileAsync(vm.Username);
                Profile = new GalleryProfileViewModel(profile);
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Profile)));
                foreach(var t in profile.Trophies)
                {
                    Trophies.Add(new TrophyViewModel(t));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
