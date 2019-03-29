using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DL444.ImgurUwp.App.ViewModels
{
    class AccountContentPageViewModel : CachingViewModel, INotifyPropertyChanged
    {
        private AccountViewModel _account;

        public AccountViewModel Account
        {
            get => _account;
            set
            {
                _account = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Account)));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsOwner)));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsNotOwner)));

                if (IsOwner)
                {
                    NonGalleryFavorites = new IncrementalLoadingCollection<NonGalleryFavoriteIncrementalSource, ItemViewModel>();
                    Comments = new IncrementalLoadingCollection<CommentIncrementalSource, CommentViewModel>();
                    MyAlbums = new IncrementalLoadingCollection<MyAlbumIncrementalSource, AccountAlbumViewModel>();
                    MyImages = new IncrementalLoadingCollection<MyImageIncrementalSource, ItemViewModel>();
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(NonGalleryFavorites)));
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Comments)));
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(MyAlbums)));
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(MyImages)));
                }
                else
                {
                    GalleryFavoriteIncrementalSource favSource = new GalleryFavoriteIncrementalSource(Account.Username);
                    GalleryFavorites = new IncrementalLoadingCollection<GalleryFavoriteIncrementalSource, GalleryItemViewModel>(favSource);
                    CommentIncrementalSource commentSource = new CommentIncrementalSource(Account.Username);
                    Comments = new IncrementalLoadingCollection<CommentIncrementalSource, CommentViewModel>(commentSource);
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(GalleryFavorites)));
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Comments)));
                }
            }
        }
        public bool IsOwner => Account == null ? false : Account.Username == ApiClient.OwnerAccount;
        public bool IsNotOwner => !IsOwner;

        public IncrementalLoadingCollection<NonGalleryFavoriteIncrementalSource, ItemViewModel> NonGalleryFavorites { get; private set; } = null;
        public IncrementalLoadingCollection<GalleryFavoriteIncrementalSource, GalleryItemViewModel> GalleryFavorites { get; private set; } = null;
        public IncrementalLoadingCollection<CommentIncrementalSource, CommentViewModel> Comments { get; private set; } = null;

        public IncrementalLoadingCollection<MyAlbumIncrementalSource, AccountAlbumViewModel> MyAlbums { get; private set; } = null;
        public IncrementalLoadingCollection<MyImageIncrementalSource, ItemViewModel> MyImages { get; private set; } = null;

        public AccountContentPageViewModel() { }
        public AccountContentPageViewModel(AccountViewModel account) => Account = account ?? throw new ArgumentNullException(nameof(account));

        public override bool EqualTo(object item)
        {
            return item is AccountContentPageViewModel vm && this.Account.Username == vm.Account.Username;
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }

    class NonGalleryFavoriteIncrementalSource : IncrementalItemsSource<ItemViewModel>
    {
        public int Page { get; private set; }

        protected override async Task<IEnumerable<ItemViewModel>> GetItemsFromSourceAsync(CancellationToken cancellationToken)
        {
            var items = new List<ItemViewModel>();
            var favs = await ApiClient.Client.GetAccountFavoritesAsync("me", Page);
            foreach (var f in favs)
            {
                items.Add(new ItemViewModel(f));
            }
            Page++;
            return items;
        }
    }
    class GalleryFavoriteIncrementalSource : IncrementalItemsSource<GalleryItemViewModel>
    {
        public int Page { get; private set; }
        public string Username { get; }

        protected override async Task<IEnumerable<GalleryItemViewModel>> GetItemsFromSourceAsync(CancellationToken cancellationToken)
        {
            var items = new List<GalleryItemViewModel>();
            // TODO: Wait what? Me?
            var favs = await ApiClient.Client.GetAccountGalleryFavoritesAsync(Username, Page);
            foreach (var f in favs)
            {
                items.Add(new GalleryItemViewModel(f));
            }
            Page++;
            return items;
        }

        public GalleryFavoriteIncrementalSource() : this("me") { }
        public GalleryFavoriteIncrementalSource(string username) => Username = username ?? throw new ArgumentNullException(nameof(username));
    }
    class CommentIncrementalSource : IncrementalItemsSource<CommentViewModel>
    {
        public int Page { get; private set; }
        public string Username { get; }

        protected override async Task<IEnumerable<CommentViewModel>> GetItemsFromSourceAsync(CancellationToken cancellationToken)
        {
            var items = new List<CommentViewModel>();
            var comments = await ApiClient.Client.GetAccountCommentsAsync(Username, page: Page);
            foreach (var c in comments)
            {
                items.Add(new CommentViewModel(c));
            }
            Page++;
            return items;
        }

        public CommentIncrementalSource() : this("me") { }
        public CommentIncrementalSource(string username) => Username = username ?? throw new ArgumentNullException(nameof(username));
    }
    class MyImageIncrementalSource : IncrementalItemsSource<ItemViewModel>
    {
        public int Page { get; private set; }

        protected override async Task<IEnumerable<ItemViewModel>> GetItemsFromSourceAsync(CancellationToken cancellationToken)
        {
            var items = new List<ItemViewModel>();
            var images = await ApiClient.Client.GetAccountImagesAsync("me", Page);
            foreach (var i in images)
            {
                items.Add(new ItemViewModel(i));
            }
            Page++;
            return items;
        }
    }
    class MyAlbumIncrementalSource : IncrementalItemsSource<AccountAlbumViewModel>
    {
        public int Page { get; private set; }

        protected override async Task<IEnumerable<AccountAlbumViewModel>> GetItemsFromSourceAsync(CancellationToken cancellationToken)
        {
            var items = new List<AccountAlbumViewModel>();
            var albums = await ApiClient.Client.GetAccountAlbumsAsync("me", Page);
            foreach (var a in albums)
            {
                items.Add(new AccountAlbumViewModel(a));
            }
            Page++;
            return items;
        }
    }
}
