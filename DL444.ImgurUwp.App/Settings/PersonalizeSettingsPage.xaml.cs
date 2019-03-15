using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using DL444.ImgurUwp.Models;
using System.ComponentModel;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace DL444.ImgurUwp.App.Settings
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class PersonalizeSettingsPage : Page, INotifyPropertyChanged
    {
        public PersonalizeSettingsPage()
        {
            this.InitializeComponent();
            this.NavigationCacheMode = NavigationCacheMode.Enabled;
        }

        private ProfileImageViewModel _selectedAvatar;
        private ProfileImageViewModel _selectedCover;
        string originalAvatar;
        string originalCover;
        bool loaded;

        ObservableCollection<ProfileImageViewModel> Avatars { get; } = new ObservableCollection<ProfileImageViewModel>();
        ObservableCollection<ProfileImageViewModel> Covers { get; } = new ObservableCollection<ProfileImageViewModel>();

        ProfileImageViewModel SelectedAvatar
        {
            get => _selectedAvatar;
            set
            {
                _selectedAvatar = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedAvatar)));
            }
        }
        ProfileImageViewModel SelectedCover
        {
            get => _selectedCover;
            set
            {
                _selectedCover = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedCover)));
            }
        }


        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            if (loaded) { return; }
            var avatars = await ApiClient.Client.GetAccountAvailableAvatarsAsync("me");
            var covers = await ApiClient.Client.GetAccountAvailableCoversAsync("me");
            var account = await ApiClient.Client.GetAccountAsync("me");
            originalAvatar = account.AvatarName;
            originalCover = account.CoverName;
            foreach (var a in avatars)
            {
                Avatars.Add(new ProfileImageViewModel(a));
            }
            foreach(var c in covers)
            {
                Covers.Add(new ProfileImageViewModel(c));
            }
            SelectedAvatar = Avatars.FirstOrDefault(x => x.Name == originalAvatar);
            AvatarGridView.SelectedItem = SelectedAvatar;
            SelectedCover = Covers.FirstOrDefault(x => x.Name == originalCover);
            CoverGridView.SelectedItem = SelectedCover;
            loaded = true;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void AvatarGridView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SelectedAvatar = e.AddedItems[0] as ProfileImageViewModel;
            ApplyButton.IsEnabled = CanApply();
        }

        private void CoverGridView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SelectedCover = e.AddedItems[0] as ProfileImageViewModel;
            ApplyButton.IsEnabled = CanApply();
        }

        private async void ApplyButton_Click(object sender, RoutedEventArgs e)
        {
            if(SelectedAvatar.Name != originalAvatar || SelectedCover.Name != originalCover)
            {
                string selectedAvatar = SelectedAvatar.Name;
                string selectedCover = SelectedCover.Name;
                ApplyButton.IsEnabled = false;
                ApplyProgress.IsActive = true;
                var success = await ApiClient.Client.SetAccountProfileAsync("me", avatar: SelectedAvatar.Name, cover: SelectedCover.Name);
                if(success)
                {
                    originalAvatar = selectedAvatar;
                    originalCover = selectedCover;
                    await ((Window.Current.Content as Frame).Content as Pages.MainPage).RefreshAccount();
                }
                // TODO: Notify user.
                ApplyButton.IsEnabled = CanApply();
                ApplyProgress.IsActive = false;
            }
        }

        bool CanApply()
        {
            if (SelectedCover == null || SelectedAvatar == null) { return false; }
            return SelectedAvatar.Name != originalAvatar || SelectedCover.Name != originalCover;
        }
    }
}
