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
using DL444.ImgurUwp.App.ViewModels;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Threading;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace DL444.ImgurUwp.App.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class AccountContent : Page, INotifyPropertyChanged
    {
        public AccountContent()
        {
             this.InitializeComponent();
        }

        private AccountViewModel _account;

        public AccountViewModel Account
        {
            get => _account;
            set
            {
                _account = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Account)));
            }
        }
        bool IsOwner => Account == null ? false : Account.Username == ApiClient.OwnerAccount;
        bool IsNotOwner => !IsOwner;

        IncrementalLoadingCollection<NonGalleryFavoriteIncrementalSource, ItemViewModel> NonGalleryFavorites 
            = new IncrementalLoadingCollection<NonGalleryFavoriteIncrementalSource, ItemViewModel>();
        IncrementalLoadingCollection<GalleryFavoriteIncrementalSource, GalleryItemViewModel> GalleryFavorites
            = new IncrementalLoadingCollection<GalleryFavoriteIncrementalSource, GalleryItemViewModel>();
        IncrementalLoadingCollection<CommentIncrementalSource, CommentViewModel> Comments 
            = new IncrementalLoadingCollection<CommentIncrementalSource, CommentViewModel>();

        IncrementalLoadingCollection<MyAlbumIncrementalSource, AccountAlbumViewModel> MyAlbums = null;
        IncrementalLoadingCollection<MyImageIncrementalSource, ItemViewModel> MyImages = null;

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            int index = 0;
            if(e.Parameter is AccountViewModel vm)
            {
                Account = vm;
            }
            else if(e.Parameter is ValueTuple<AccountViewModel, int> vmIndex)
            {
                Account = vmIndex.Item1;
                index = vmIndex.Item2;
            }
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsOwner)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsNotOwner)));
            if(index > 1)
            {
                while(RootPivot.Items.Count < 3)
                {
                    await Task.Delay(100);
                }
            }
            RootPivot.SelectedIndex = index;
        }

        private async void RootPivot_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(Account == null)
            {
                await Task.Delay(200);
            }

            switch((e.AddedItems[0] as PivotItem).Tag as string)
            {
                case "Albums":
                    if(IsOwner && MyAlbums == null)
                    {
                        MyAlbums = new IncrementalLoadingCollection<MyAlbumIncrementalSource, AccountAlbumViewModel>();
                        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(MyAlbums)));
                    }
                    break;
                case "Images":
                    if(IsOwner && MyImages == null)
                    {
                        MyImages = new IncrementalLoadingCollection<MyImageIncrementalSource, ItemViewModel>();
                        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(MyImages)));
                    }
                    break;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void GalleryFavGrid_ItemClick(object sender, ItemClickEventArgs e)
        {
            Navigation.ContentFrame.Navigate(typeof(GalleryItemDetails), new GalleryItemDetailsNavigationParameter(e.ClickedItem as GalleryItemViewModel, new StaticIncrementalSource<GalleryItemViewModel>(GalleryFavorites)));
        }

        private void ItemGrid_ItemClick(object sender, ItemClickEventArgs e)
        {
            if(e.ClickedItem is AccountAlbumViewModel accountAlbum)
            {
                if(accountAlbum.InGallery)
                {
                    Navigation.ContentFrame.Navigate(typeof(Pages.GalleryItemDetails), accountAlbum.Id);
                }
                else
                {
                    Navigation.ContentFrame.Navigate(typeof(Pages.Upload), accountAlbum);
                }
            }
            else if(e.ClickedItem is ItemViewModel item)
            {
                if (item.InGallery)
                {
                    Navigation.ContentFrame.Navigate(typeof(Pages.GalleryItemDetails), item.Id);
                }
            }
        }
    }

    public class NonGalleryFavoriteIncrementalSource : IncrementalItemsSource<ItemViewModel>
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
    public class GalleryFavoriteIncrementalSource : IncrementalItemsSource<GalleryItemViewModel>
    {
        public int Page { get; private set; }

        protected override async Task<IEnumerable<GalleryItemViewModel>> GetItemsFromSourceAsync(CancellationToken cancellationToken)
        {
            var items = new List<GalleryItemViewModel>();
            var favs = await ApiClient.Client.GetAccountGalleryFavoritesAsync("me", Page);
            foreach (var f in favs)
            {
                items.Add(new GalleryItemViewModel(f));
            }
            Page++;
            return items;
        }
    }
    public class CommentIncrementalSource : IncrementalItemsSource<CommentViewModel>
    {
        public int Page { get; private set; }

        protected override async Task<IEnumerable<CommentViewModel>> GetItemsFromSourceAsync(CancellationToken cancellationToken)
        {
            var items = new List<CommentViewModel>();
            var comments = await ApiClient.Client.GetAccountCommentsAsync("me", page: Page);
            foreach (var c in comments)
            {
                items.Add(new CommentViewModel(c));
            }
            Page++;
            return items;
        }
    }
    public class MyImageIncrementalSource : IncrementalItemsSource<ItemViewModel>
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
    public class MyAlbumIncrementalSource : IncrementalItemsSource<AccountAlbumViewModel>
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
    public class StaticIncrementalSource<T> : IncrementalItemsSource<T>
    {
        public StaticIncrementalSource(IEnumerable<T> items) : base(items) { }

        protected override async Task<IEnumerable<T>> GetItemsFromSourceAsync(CancellationToken cancellationToken)
        {
            return null;
        }
    }
}
