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
    public sealed partial class AccountContent : Page
    {
        public AccountContent()
        {
            this.InitializeComponent();
        }

        AccountContentPageViewModel ViewModel { get; set; }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            int index = 0;
            if (e.Parameter is AccountViewModel vm)
            {
                var cache = ViewModelCacheManager.Instance.Peek<AccountContentPageViewModel>();
                if (cache != null && cache.Account.Username == vm.Username)
                {
                    ViewModel = cache;
                }
                else
                {
                    ViewModel = new AccountContentPageViewModel(vm);
                    ViewModelCacheManager.Instance.Push(ViewModel);
                }
                Bindings.Update();
            }
            else if (e.Parameter is ValueTuple<AccountViewModel, int> vmIndex)
            {
                var cache = ViewModelCacheManager.Instance.Peek<AccountContentPageViewModel>();
                if (cache != null && cache.Account.Username == vmIndex.Item1.Username)
                {
                    ViewModel = cache;
                }
                else
                {
                    ViewModel = new AccountContentPageViewModel(vmIndex.Item1);
                    ViewModelCacheManager.Instance.Push(ViewModel);
                }
                Bindings.Update();
                index = vmIndex.Item2;
            }
            if (index > 1)
            {
                while (RootPivot.Items.Count < 3)
                {
                    await Task.Delay(100);
                }
            }
            RootPivot.SelectedIndex = index;
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            if(e.NavigationMode == NavigationMode.Back)
            {
                ViewModelCacheManager.Instance.Pop<AccountContentPageViewModel>();
            }
            else
            {
                Navigation.ContentFrame.BackStack.RemoveAt(Navigation.ContentFrame.BackStack.Count - 1);
                Navigation.ContentFrame.BackStack.Add(new PageStackEntry(typeof(AccountContent), (ViewModel.Account, RootPivot.SelectedIndex), null));
            }
        }

        private void GalleryFavGrid_ItemClick(object sender, ItemClickEventArgs e)
        {
            Navigation.ContentFrame.Navigate(typeof(GalleryItemDetails), new GalleryItemDetailsNavigationParameter(e.ClickedItem as GalleryItemViewModel, new StaticIncrementalSource<GalleryItemViewModel>(ViewModel.GalleryFavorites)));
        }

        private void NonGalleryFavGrid_ItemClick(object sender, ItemClickEventArgs e)
        {
            // Hey! Everything seems wrong from Favorites endpoint.
            // Issues: 
            // Account information is missing in hidden and secret items.
            // In gallery information is incorrect for public items.
            // Fix: 
            // -  Always treat hidden and secret items as non-owner.
            // -  For public items, check for Points field. 
            //    It is reliably null for non-gallery items, and not null for gallery items.
            // Note:
            // gallery items may have a hidden/secret privacy level, but account id is still available.

            if (e.ClickedItem is ItemViewModel item)
            {
                if(item.AccountUrl == null || item.Item.Points == null)
                {
                    // Non-gallery.
                    // TODO: Implement non-owner non-gallery view.
                }
                else
                {
                    // Gallery.
                    Navigation.ContentFrame.Navigate(typeof(Pages.GalleryItemDetails), item.Id);
                }
            }
        }

        private void AlbumGrid_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (e.ClickedItem is AccountAlbumViewModel accountAlbum)
            {
                if (accountAlbum.InGallery)
                {
                    Navigation.ContentFrame.Navigate(typeof(Pages.GalleryItemDetails), accountAlbum.Id);
                }
                else
                {
                    Navigation.ContentFrame.Navigate(typeof(Pages.Upload), accountAlbum);
                }
            }
        }

        private void ImageGrid_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (e.ClickedItem is ItemViewModel item)
            {
                Navigation.Navigate(typeof(ImageView), (ViewModel.MyImages, item));
            }
        }

        private void AllItemGrd_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (e.ClickedItem is ItemViewModel item)
            {
                if (item.InGallery)
                {
                    Navigation.ContentFrame.Navigate(typeof(GalleryItemDetails), item.Id);
                }
                else
                {
                    if (!item.IsAlbum)
                    {
                        Navigation.Navigate(typeof(ImageView), (ViewModel.MyImages, item));
                    }
                    else
                    {
                        Navigation.Navigate(typeof(Upload), item);
                    }
                }
            }
        }
    }
}
