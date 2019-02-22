using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using UI = Microsoft.UI.Xaml.Controls;
using DL444.ImgurUwp.App.ViewModels;
using DL444.ImgurUwp.ApiClient;
using DL444.ImgurUwp.Models;
using System.ComponentModel;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace DL444.ImgurUwp.App
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class FrontPage : Page, INotifyPropertyChanged
    {
        private AccountViewModel _currentAccount = new AccountViewModel();
        private GalleryCollectionViewModel _frontPageItems = new GalleryCollectionViewModel();

        AccountViewModel CurrentAccount
        {
            get => _currentAccount;
            set
            {
                _currentAccount = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CurrentAccount)));
            }
        }
        GalleryCollectionViewModel FrontPageItems
        {
            get => _frontPageItems;
            set
            {
                _frontPageItems = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(FrontPageItems)));
            }
        }

        public FrontPage()
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
            FrontPageItems = new GalleryCollectionViewModel(galleryItems);
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

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
