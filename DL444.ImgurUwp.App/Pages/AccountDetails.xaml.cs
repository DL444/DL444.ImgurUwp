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
using Microsoft.Toolkit.Uwp;

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
        private int _hiddenTrophiesCount;

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
        ObservableCollection<TrophyViewModel> DisplayedTrophies { get; set; } = new ObservableCollection<TrophyViewModel>();
        int HiddenTrophiesCount
        {
            get => _hiddenTrophiesCount;
            set
            {
                _hiddenTrophiesCount = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(HiddenTrophiesCount)));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(MoreTrophiesButtonVisibility)));
            }
        }
        Visibility MoreTrophiesButtonVisibility => HiddenTrophiesCount == 0 ? Visibility.Collapsed : Visibility.Visible;


        IncrementalLoadingCollection<AccountPostSource, GalleryItemViewModel> Posts = new IncrementalLoadingCollection<AccountPostSource, GalleryItemViewModel>();

        ObservableCollection<CommentViewModel> Comments { get; set; } = new ObservableCollection<CommentViewModel>();

        string BioPlaceholderText { get; set; }

        public bool IsOwner => ViewModel == null ? false : ViewModel.Username == ApiClient.OwnerAccount;

        public AccountDetails()
        {
            this.InitializeComponent();
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            // TODO: After implementing account subpage, clean up this.
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
            else if(e.Parameter is ValueTuple<AccountViewModel, int> vmParam)
            {
                await PrepareViewModels(vmParam.Item1);
                //AccountPivot.SelectedIndex = vmParam.Item2;
            }
            else if(e.Parameter is ValueTuple<string, int> userParam)
            {
                var account = await ApiClient.Client.GetAccountAsync(userParam.Item1);
                await PrepareViewModels(new AccountViewModel(account));
                //AccountPivot.SelectedIndex = userParam.Item2;
            }
        }

        async Task PrepareViewModels(AccountViewModel vm)
        {
            ViewModel = vm;
            if(IsOwner) { BioPlaceholderText = "Tell Imgur a little about yourself..."; }
            else { BioPlaceholderText = ""; }
            Bindings.Update();

            var profile = await ApiClient.Client.GetAccountGalleryProfileAsync(ViewModel.Username);
            Profile = new GalleryProfileViewModel(profile);
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Profile)));

            foreach (var t in profile.Trophies)
            {
                var trophyVm = new TrophyViewModel(t);
                Trophies.Add(trophyVm);
                if (DisplayedTrophies.Count < 7 && !DisplayedTrophies.Any(x => x.Name == trophyVm.Name))
                {
                    DisplayedTrophies.Add(trophyVm);
                }
            }
            HiddenTrophiesCount = Trophies.Count - DisplayedTrophies.Count;

            AccountPostSource source = new AccountPostSource(ViewModel.Username);
            Posts = new IncrementalLoadingCollection<AccountPostSource, GalleryItemViewModel>(source);
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Posts)));

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

        private async void Pivot_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // TODO: Remove this after redesign.
            while(ViewModel == null)
            {
                await Task.Delay(200);
            }
            switch ((e.AddedItems[0] as PivotItem).Tag as string)
            {
                case "Trophies":
                    if(Profile != null) { break; }
                    var profile = await ApiClient.Client.GetAccountGalleryProfileAsync(ViewModel.Username);
                    Profile = new GalleryProfileViewModel(profile);
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Profile)));
                    foreach (var t in profile.Trophies)
                    {
                        Trophies.Add(new TrophyViewModel(t));
                    }
                    break;
                case "Posts":
                    if (Posts.Count != 0) { break; }
                    AccountPostSource source = new AccountPostSource(ViewModel.Username);
                    Posts = new IncrementalLoadingCollection<AccountPostSource, GalleryItemViewModel>(source);
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Posts)));
                    break;
                case "Favorites":
                    //if (IsOwner)
                    //{

                    //}
                    //else
                    //{

                    //}
                    break;
                case "Comments":
                    if (Comments.Count != 0) { break; }
                    var comments = await ApiClient.Client.GetAccountCommentsAsync(ViewModel.Username);
                    foreach(var c in comments)
                    {
                        if (c.Deleted) { continue; }
                        Comments.Add(new CommentViewModel(c));
                    }
                    break;
                case "Images":
                    break;
            }
        }

        private void PostList_ItemClick(object sender, ItemClickEventArgs e)
        {
            var item = e.ClickedItem as GalleryItemViewModel;
            var gallery = new GalleryCollectionViewModel(Posts);
            Navigation.ContentFrame.Navigate(typeof(Pages.GalleryItemDetails), (item, gallery));
        }
    }

    public class AccountPostSource : IncrementalItemsSource<GalleryItemViewModel>
    {
        public string Account { get; private set; }
        public int Page { get; private set; }

        public AccountPostSource() : this("") { }
        public AccountPostSource(string account)
        {
            Account = account ?? throw new ArgumentNullException(nameof(account));
        }

        protected override async Task<IEnumerable<GalleryItemViewModel>> GetItemsFromSourceAsync(CancellationToken cancellationToken)
        {
            if(string.IsNullOrEmpty(Account))
            {
                return null;
            }

            var posts = await ApiClient.Client.GetAccountSubmissionsAsync(Account, Page);
            List<GalleryItemViewModel> items = new List<GalleryItemViewModel>();
            foreach(var p in posts)
            {
                items.Add(new GalleryItemViewModel(p));
            }
            Page++;
            return items;
        }
    }
}
