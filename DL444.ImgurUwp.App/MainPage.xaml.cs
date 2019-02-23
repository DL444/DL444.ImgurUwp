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
using UI = Microsoft.UI.Xaml.Controls;

using DL444.ImgurUwp.Models;
using Windows.UI.ViewManagement;
using DL444.ImgurUwp.App.ViewModels;
using System.ComponentModel;
using Windows.UI;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace DL444.ImgurUwp.App
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page, INotifyPropertyChanged
    {
        private AccountViewModel _currentAccount = new AccountViewModel();

        AccountViewModel CurrentAccount
        {
            get => _currentAccount;
            set
            {
                _currentAccount = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CurrentAccount)));
            }
        }

        public MainPage()
        {
            this.InitializeComponent();
            SetTitleBarButtonColor();
            this.ActualThemeChanged += (sender, e) => SetTitleBarButtonColor();
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            foreach (var i in RootNavView.MenuItems)
            {
                var item = (i as UI.NavigationViewItem);
                if (item.Tag as string == "viral")
                {
                    RootNavView.SelectedItem = item;
                }
            }

            var meAccount = await ApiClient.Client.GetAccountAsync("me");
            CurrentAccount = new AccountViewModel(meAccount);
            Bindings.Update();
            var galleryItems = await ApiClient.Client.GetGalleryItemsAsync();
            ContentFrame.Navigate(typeof(GalleryView), new GalleryCollectionViewModel(galleryItems));
        }

        void SetTitleBarButtonColor()
        {
            var titleBar = ApplicationView.GetForCurrentView().TitleBar;
            titleBar.ButtonForegroundColor = (Color)Application.Current.Resources["SystemBaseHighColor"];
            titleBar.ButtonInactiveForegroundColor = (Color)Application.Current.Resources["SystemBaseMediumColor"];
        }

        private void RootNavView_SelectionChanged(UI.NavigationView sender, UI.NavigationViewSelectionChangedEventArgs args)
        {

        }

        private void Signout_Click(object sender, RoutedEventArgs e)
        {
            var credentialVault = new Windows.Security.Credentials.PasswordVault();
            var credentialList = credentialVault.RetrieveAll();
            foreach (var i in credentialList)
            {
                credentialVault.Remove(i);
            }
            (Window.Current.Content as Frame).Navigate(typeof(LoginPage));
        }

        private void AccountDetails_Click(object sender, RoutedEventArgs e)
        {
            ContentFrame.Navigate(typeof(AccountDetailsPage), CurrentAccount);
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
