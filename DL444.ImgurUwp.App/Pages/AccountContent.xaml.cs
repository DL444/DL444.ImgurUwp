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

        ObservableCollection<ItemViewModel> NonGalleryFavorites { get; } = new ObservableCollection<ItemViewModel>();
        ObservableCollection<GalleryItemViewModel> GalleryFavorites { get; } = new ObservableCollection<GalleryItemViewModel>();
        ObservableCollection<CommentViewModel> Comments { get; } = new ObservableCollection<CommentViewModel>();

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            if(e.Parameter is AccountViewModel vm)
            {
                Account = vm;
            }
            else if(e.Parameter is ValueTuple<AccountViewModel, int> vmIndex)
            {
                Account = vmIndex.Item1;
                RootPivot.SelectedIndex = vmIndex.Item2;
            }
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsOwner)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsNotOwner)));
        }

        private async void RootPivot_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(Account == null)
            {
                await Task.Delay(200);
            }

            switch((e.AddedItems[0] as PivotItem).Tag as string)
            {
                case "Favorites":
                    if(IsOwner)
                    {
                        if(NonGalleryFavorites.Count != 0) { break; }
                        var favorites = await ApiClient.Client.GetAccountFavoritesAsync("me");
                        foreach(var f in favorites)
                        {
                            NonGalleryFavorites.Add(new ItemViewModel(f));
                        }
                    }
                    else
                    {
                        if(GalleryFavorites.Count != 0) { break; }
                        var favorites = await ApiClient.Client.GetAccountGalleryFavoritesAsync(Account.Username);
                        foreach(var f in favorites)
                        {
                            GalleryFavorites.Add(new GalleryItemViewModel(f));
                        }
                    }
                    break;
                case "Comments":
                    if (Comments.Count != 0) { break; }
                    var comments = await ApiClient.Client.GetAccountCommentsAsync(Account.Username);
                    foreach (var c in comments)
                    {
                        if (c.Deleted) { continue; }
                        Comments.Add(new CommentViewModel(c));
                    }
                    break;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void GalleryFavGrid_ItemClick(object sender, ItemClickEventArgs e)
        {
            Navigation.ContentFrame.Navigate(typeof(GalleryItemDetails), (e.ClickedItem as GalleryItemViewModel, new GalleryCollectionViewModel(GalleryFavorites)));
        }

        private void FavGrid_ItemClick(object sender, ItemClickEventArgs e)
        {

        }
    }
}
