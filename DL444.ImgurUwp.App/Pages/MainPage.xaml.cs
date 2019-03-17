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
using Windows.UI.ViewManagement;
using DL444.ImgurUwp.App.ViewModels;
using System.ComponentModel;
using Windows.UI;
using DL444.ImgurUwp.ApiClient;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace DL444.ImgurUwp.App.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page, INotifyPropertyChanged
    {
        private AccountViewModel _currentAccount;

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
            Navigation.InitializeNavigationHelper(ContentFrame);
            ContentFrame.CacheSize = 5;
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            ContentFrame.Navigate(typeof(GalleryView), DisplayParams.Section.Hot);

            await RefreshAccount();
            ApiClient.OwnerAccount = CurrentAccount.Username;
        }

        void SetTitleBarButtonColor()
        {
            var titleBar = ApplicationView.GetForCurrentView().TitleBar;
            titleBar.ButtonForegroundColor = (Color)Application.Current.Resources["SystemBaseHighColor"];
            titleBar.ButtonInactiveForegroundColor = (Color)Application.Current.Resources["SystemBaseMediumColor"];
        }

        private void Signout_Click(object sender, RoutedEventArgs e)
        {
            var credentialVault = new Windows.Security.Credentials.PasswordVault();
            var credentialList = credentialVault.RetrieveAll();
            foreach (var i in credentialList)
            {
                credentialVault.Remove(i);
            }
            (Window.Current.Content as Frame).Navigate(typeof(Login));
        }

        private void AccountDetails_Click(object sender, RoutedEventArgs e)
        {
            ContentFrame.Navigate(typeof(AccountDetails), CurrentAccount);
        }

        private void AccountContent_Click(object sender, RoutedEventArgs e)
        {
            int index = int.Parse(((sender as MenuFlyoutItem).Tag as string));
            ContentFrame.Navigate(typeof(AccountContent), (CurrentAccount, index));
        }

        private void Settings_Click(object sender, RoutedEventArgs e)
        {
            ContentFrame.Navigate(typeof(Settings.SettingsFrame));
        }

        private void Upload_Click(object sender, RoutedEventArgs e)
        {
            ContentFrame.Navigate(typeof(Upload));
        }

        private void RootNavView_BackRequested(UI.NavigationView sender, UI.NavigationViewBackRequestedEventArgs args)
        {
            if(ContentFrame.CanGoBack)
            {
                ContentFrame.GoBack();
            }
        }

        private void ContentFrame_Navigated(object sender, NavigationEventArgs e)
        {
            if(e.SourcePageType == typeof(GalleryView))
            {
                if(e.Parameter is DisplayParams.Section s)
                {
                    if(s == DisplayParams.Section.Hot) { RootNavView.SelectedItem = RootNavView.MenuItems[0]; return; }
                    else if(s == DisplayParams.Section.User) { RootNavView.SelectedItem = RootNavView.MenuItems[1]; return; }
                }
            }
            RootNavView.SelectedItem = null;
        }

        private void RootNavView_ItemInvoked(UI.NavigationView sender, UI.NavigationViewItemInvokedEventArgs args)
        {
            UI.NavigationViewItem item = args.InvokedItemContainer as UI.NavigationViewItem;
            if (item == null) { return; }

            switch(item.Tag as string)
            {
                case "viral":
                    ContentFrame.Navigate(typeof(GalleryView), DisplayParams.Section.Hot);
                    break;
                case "user":
                    ContentFrame.Navigate(typeof(GalleryView), DisplayParams.Section.User);
                    break;
                case "random":
                    //ContentFrame.Navigate(typeof(GalleryView), DisplayParams.Section.);
                    break;
            }
        }

        public async System.Threading.Tasks.Task RefreshAccount()
        {
            var meAccount = await ApiClient.Client.GetAccountAsync("me");
            CurrentAccount = new AccountViewModel(meAccount);
            Bindings.Update();
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
