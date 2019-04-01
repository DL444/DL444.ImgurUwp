using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.UI.Xaml;

namespace DL444.ImgurUwp.App.ViewModels
{
    class AccountDetailsPageViewModel : CachingViewModel, INotifyPropertyChanged
    {
        private AccountViewModel _viewModel;
        private string originalBio;
        private int _hiddenTrophiesCount;
        readonly Func<MessageBus.GalleryRemoveMessage, bool> galleryRemoveHandler;
        readonly Func<MessageBus.BioChangedMessage, bool> bioChangeHandler;
        readonly Func<MessageBus.GalleryPostMessage, bool> galleryPostHandler;

        public AccountViewModel ViewModel
        {
            get => _viewModel;
            set
            {
                _viewModel = value;
                originalBio = value.Biography;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ViewModel)));
            }
        }

        public GalleryProfileViewModel Profile { get; set; }
        public ObservableCollection<TrophyViewModel> Trophies { get; set; } = new ObservableCollection<TrophyViewModel>();
        public ObservableCollection<TrophyViewModel> DisplayedTrophies { get; set; } = new ObservableCollection<TrophyViewModel>();
        public int HiddenTrophiesCount
        {
            get => _hiddenTrophiesCount;
            set
            {
                _hiddenTrophiesCount = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(HiddenTrophiesCount)));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(MoreTrophiesButtonVisibility)));
            }
        }
        public Visibility MoreTrophiesButtonVisibility => HiddenTrophiesCount == 0 ? Visibility.Collapsed : Visibility.Visible;


        public IncrementalLoadingCollection<AccountPostSource, GalleryItemViewModel> Posts { get; set; } = new IncrementalLoadingCollection<AccountPostSource, GalleryItemViewModel>();

        public string BioPlaceholderText => IsOwner ? "Tell Imgur a little about yourself..." : "";
        public Visibility BioVisilibity => this.IsOwner ? Visibility.Visible : (ViewModel == null || string.IsNullOrWhiteSpace(ViewModel.Biography) ? Visibility.Collapsed : Visibility.Visible);

        public bool IsOwner => ViewModel == null ? false : ViewModel.Username == ApiClient.OwnerAccount;

        public async Task ChangeBio()
        {
            if(originalBio != ViewModel.Biography)
            {
                bool result = await ApiClient.Client.SetAccountProfileAsync(ViewModel.Username, ViewModel.Biography);
                if (result == true)
                {
                    originalBio = ViewModel.Biography;
                    MessageBus.ViewModelMessageBus.Instance.SendMessage(new MessageBus.BioChangedMessage(ViewModel.Biography));
                }
                else
                {
                    ViewModel.Biography = originalBio;
                }
            }
        }

        public static async Task<AccountDetailsPageViewModel> CreateFromAccountUsername(string username)
        {
            var account = await ApiClient.Client.GetAccountAsync(username);
            return await CreateFromAccount(new AccountViewModel(account));
        }
        public static async Task<AccountDetailsPageViewModel> CreateFromAccount(AccountViewModel vm)
        {
            AccountDetailsPageViewModel r = new AccountDetailsPageViewModel();
            r.ViewModel = vm;
            var profile = await ApiClient.Client.GetAccountGalleryProfileAsync(vm.Username);
            r.Profile = new GalleryProfileViewModel(profile);

            foreach (var t in profile.Trophies)
            {
                var trophyVm = new TrophyViewModel(t);
                r.Trophies.Add(trophyVm);
                if (r.DisplayedTrophies.Count < 7 && !r.DisplayedTrophies.Any(x => x.Name == trophyVm.Name))
                {
                    r.DisplayedTrophies.Add(trophyVm);
                }
            }
            r.HiddenTrophiesCount = r.Trophies.Count - r.DisplayedTrophies.Count;

            AccountPostSource source = new AccountPostSource(vm.Username);
            r.Posts = new IncrementalLoadingCollection<AccountPostSource, GalleryItemViewModel>(source);
            return r;
        }

        AccountDetailsPageViewModel()
        {
            galleryRemoveHandler = new Func<MessageBus.GalleryRemoveMessage, bool>(x =>
            {
                if (IsOwner)
                {
                    return Posts.Remove(i => i.Id == x.Id && i.IsAlbum == x.IsAlbum);
                }
                return false;
            });
            MessageBus.ViewModelMessageBus.Instance.RegisterListener(new MessageBus.GalleryRemoveMessageListener(galleryRemoveHandler));
            bioChangeHandler = new Func<MessageBus.BioChangedMessage, bool>(x =>
            {
                if(IsOwner)
                {
                    originalBio = x.NewBio;
                    ViewModel.Biography = x.NewBio;
                    return true;
                }
                return false;
            });
            MessageBus.ViewModelMessageBus.Instance.RegisterListener(new MessageBus.BioChangedMessageListener(bioChangeHandler));
            galleryPostHandler = new Func<MessageBus.GalleryPostMessage, bool>(x =>
            {
                if(IsOwner)
                {
                    Posts.Insert(0, new GalleryItemViewModel(x.Item));
                    return true;
                }
                return false;
            });
            MessageBus.ViewModelMessageBus.Instance.RegisterListener(new MessageBus.GalleryPostMessageListener(galleryPostHandler));
        }

        public event PropertyChangedEventHandler PropertyChanged;
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
        public AccountPostSource(string account, IEnumerable<GalleryItemViewModel> items, int startPage) : base(items)
        {
            Account = account ?? throw new ArgumentNullException(nameof(account));
            Page = startPage;
        }

        protected override async Task<IEnumerable<GalleryItemViewModel>> GetItemsFromSourceAsync(CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(Account))
            {
                return null;
            }

            var posts = await ApiClient.Client.GetAccountSubmissionsAsync(Account, Page);
            List<GalleryItemViewModel> items = new List<GalleryItemViewModel>();
            foreach (var p in posts)
            {
                items.Add(new GalleryItemViewModel(p));
            }
            Page++;
            return items;
        }
    }
}
