using System;
using System.Collections.Generic;
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

namespace DL444.ImgurUwp.App.Settings
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class AccountSettingsPage : Page, INotifyPropertyChanged
    {
        public AccountSettingsPage()
        {
            this.InitializeComponent();
            this.NavigationCacheMode = NavigationCacheMode.Enabled;
        }

        private bool _loaded = true;
        public bool PageLoading
        {
            get => _loaded;
            set
            {
                _loaded = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(PageLoading)));
            }
        }


        AccountSettingsViewModel ViewModel { get; set; }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            if (!PageLoading) { return; }
            var settings = await ApiClient.Client.GetAccountSettingsAsync("me");
            ViewModel = new AccountSettingsViewModel(settings);
            Bindings.Update();
            PageLoading = false;
        }

        private async void AcceptTerms_Click(object sender, RoutedEventArgs e)
        {
            AcceptTermsBtn.IsEnabled = false;
            TermAcceptProgress.IsActive = true;
            var result = await ApiClient.Client.AcceptGalleryTermsAsync("me");
            if(result == true)
            {
                ViewModel.GalleryTermsNotAccepted = false;
            }
            else
            {
                AcceptTermsBtn.IsEnabled = true;
            }
            TermAcceptProgress.IsActive = false;
        }

        private async void VerifyEmail_Click(object sender, RoutedEventArgs e)
        {
            SendEmailBtn.IsEnabled = false;
            await ApiClient.Client.VerifyAccountEmailAsync("me");
            EmailSentText.Visibility = Visibility.Visible;
        }

        private async void PublicByDefault_Toggled(object sender, RoutedEventArgs e)
        {
            if (PublicImgSwitch.IsOn == ViewModel.PublicImagesByDefault) { return; }

            PublicImgSwitch.IsEnabled = false;
            var result = await ApiClient.Client.SetAccountSettingsAsync("me", publicImagesByDefault: PublicImgSwitch.IsOn);
            if (result == true)
            {
                ViewModel.PublicImagesByDefault = PublicImgSwitch.IsOn;
            }
            PublicImgSwitch.IsEnabled = true;
        }

        private async void AlbumPrivacyLevel_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var tag = (e.AddedItems[0] as ComboBoxItem).Tag as string;
            Models.AlbumPrivacyOptions value;
            switch(tag)
            {
                case "Hidden":
                    value = Models.AlbumPrivacyOptions.Hidden;
                    break;
                case "Public":
                    value = Models.AlbumPrivacyOptions.Public;
                    break;
                case "Secret":
                    value = Models.AlbumPrivacyOptions.Secret;
                    break;
                default:
                    throw new ArgumentOutOfRangeException($"The selected item tag {tag} is out of the expected range.");
            }
            if(value == ViewModel.AlbumPrivacy) { return; }

            AlbumPrivacyBox.IsEnabled = false;
            var result = await ApiClient.Client.SetAccountSettingsAsync("me", albumDefaultPrivacy: value);
            if (result == true)
            {
                ViewModel.AlbumPrivacy = value;
            }
            AlbumPrivacyBox.IsEnabled = true;
        }

        private async void Messaging_Toggled(object sender, RoutedEventArgs e)
        {
            if(MessageSwitch.IsOn == ViewModel.MessagingEnabled) { return; }
            MessageSwitch.IsEnabled = false;
            var result = await ApiClient.Client.SetAccountSettingsAsync("me", messagingEnabled: MessageSwitch.IsOn);
            if (result == true)
            {
                ViewModel.PublicImagesByDefault = MessageSwitch.IsOn;
            }
            MessageSwitch.IsEnabled = true;
        }

        private async void ShowMature_Toggled(object sender, RoutedEventArgs e)
        {
            if (MatureSwitch.IsOn == ViewModel.ShowMature) { return; }
            MatureSwitch.IsEnabled = false;
            var result = await ApiClient.Client.SetAccountSettingsAsync("me", showMature: MatureSwitch.IsOn);
            if(result == true)
            {
                ViewModel.ShowMature = MatureSwitch.IsOn;
            }
            MatureSwitch.IsEnabled = true;
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }

    public class AlbumPrivacyConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var option = (Models.AlbumPrivacyOptions)value;
            switch (option)
            {
                case Models.AlbumPrivacyOptions.Public:
                    return 2;
                case Models.AlbumPrivacyOptions.Hidden:
                    return 0;
                case Models.AlbumPrivacyOptions.Secret:
                    return 1;
                default:
                    return -1;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            int index = (int)value;
            if (index == 0) { return Models.AlbumPrivacyOptions.Hidden; }
            if (index == 1) { return Models.AlbumPrivacyOptions.Secret; }
            if (index == 2) { return Models.AlbumPrivacyOptions.Public; }
            else { throw new ArgumentOutOfRangeException($"Passed index {index} is not in expected range."); }
        }
    }

    public class NullableBoolCoverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var val = value as bool?;
            return val == true;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return (bool?)((bool)value);
        }
    }
}
