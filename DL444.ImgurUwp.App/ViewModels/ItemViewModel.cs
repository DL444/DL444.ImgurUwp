using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DL444.ImgurUwp.Models;
using Windows.ApplicationModel.DataTransfer;

namespace DL444.ImgurUwp.App.ViewModels
{
    class ItemViewModel : INotifyPropertyChanged
    {
        IItem _item;
        Image _displayImage;
        string _thumbnail;

        public IItem Item
        {
            get => _item;
            set
            {
                _item = value;
                if(value.IsAlbum == true)
                {
                    Album album = value as Album;
                    _displayImage = album.Images.FirstOrDefault(x => x.Id == album.Cover);
                    if(_displayImage == null)
                    {
                        _displayImage = album.Images.FirstOrDefault();
                    }
                }
                else
                {
                    _displayImage = value as Image;
                }

                if(_displayImage != null)
                {
                    string link = DisplayImage.Link;
                    if (string.IsNullOrWhiteSpace(link)) { _thumbnail = null; }
                    else
                    {
                        if (DisplayImage.Animated)
                        {
                            _thumbnail = $"{DisplayImage.Link.Remove(DisplayImage.Link.LastIndexOf('/'))}/{DisplayImage.Id}_d.jpg?maxwidth=520&shape=thumb&fidelity=mid";
                        }
                        else
                        {
                            _thumbnail = $"{DisplayImage.Link.Replace(DisplayImage.Id, $"{DisplayImage.Id}_d")}?maxwidth=520&shape=thumb&fidelity=mid";
                        }
                    }
                }

                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(null));
            }
        }
        public bool IsAlbum => Item.IsAlbum == true;
        public bool InGallery => Item.InGallery;

        public string Id => Item.Id;
        public string Title => Item.Title;
        public string Description => Item.Description;
        public DateTime DateTime => Convert.ToDateTime(Item.DateTime);
        public string Link => Item.Link;
        public string AccountUrl => Item.AccountUrl;

        public int Views => Item.Views;
        public string DeleteHash => Item.DeleteHash;
        public string Section => Item.Section;
        public bool Nsfw => Item.Nsfw == true;
        public bool Favorite
        {
            get => Item.Favorite == true;
            set
            {
                Item.Favorite = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Favorite)));
            }
        }

        public int ImageCount
        {
            get
            {
                switch (Item)
                {
                    case null:
                        return 0;
                    case Image _:
                    case GalleryImage _:
                        return 1;
                    case Album a:
                        return a.ImageCount;
                    case GalleryAlbum ga:
                        return ga.ImageCount;
                    default:
                        return 0;
                }
            }
        }
        public Windows.UI.Xaml.Media.Imaging.BitmapImage Thumbnail
        {
            get
            {
                if(string.IsNullOrEmpty(_thumbnail)) { return null; }
                else
                {
                    return new Windows.UI.Xaml.Media.Imaging.BitmapImage(new Uri(_thumbnail));
                }
            }
        }
        public Image DisplayImage
        {
            get => _displayImage;
            set
            {
                _displayImage = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(DisplayImage)));
            }
        }

        public bool HasTitle => !string.IsNullOrEmpty(Title);
        public bool HasImage => ImageCount > 0;
        public bool IsOwner => AccountUrl == ApiClient.OwnerAccount;

        public AsyncCommand<object> DownloadCommand { get; private set; }
        public Command CopyUrlCommand { get; private set; }
        public AsyncCommand<bool> DeleteCommand { get; private set; }
        public AsyncCommand<bool> FavoriteItemCommand { get; private set; }
        public Command ShareCommand { get; set; }
        public AsyncCommand<object> OpenBrowserCommand { get; private set; }

        async Task<object> Download()
        {
            string filename = DisplayImage.Link.Substring(DisplayImage.Link.LastIndexOf('/') + 1);
            using (var imageStream = System.IO.WindowsRuntimeStreamExtensions.AsInputStream(await ApiClient.Client.DownloadMediaAsync(DisplayImage.Link)))
            {
                await CommonOperations.Save(imageStream, filename);
            }
            return null;
        }
        void CopyUrl()
        {
            var package = new Windows.ApplicationModel.DataTransfer.DataPackage();
            package.SetText(Link);
            Windows.ApplicationModel.DataTransfer.Clipboard.SetContent(package);
        }
        async Task<bool> Delete()
        {
            bool result;
            if(IsAlbum)
            {
                if(ImageCount == 1)
                {
                    var confirmResult = await CreateConfirmDialog().ShowAsync();
                    if (confirmResult == Windows.UI.Xaml.Controls.ContentDialogResult.Primary)
                    {
                        result = await ApiClient.Client.DeleteAlbumAsync(Id);
                    }
                    else if (confirmResult == Windows.UI.Xaml.Controls.ContentDialogResult.Secondary)
                    {
                        string imageId = DisplayImage.Id;
                        var imageResult = await ApiClient.Client.DeleteImageAsync(imageId);
                        result = await ApiClient.Client.DeleteAlbumAsync(Id);
                        if(imageResult == true)
                        {
                            MessageBus.ViewModelMessageBus.Instance.SendMessage(new MessageBus.ItemDeleteMessage(DisplayImage.Id, false));
                        }
                    }
                    else { return true; }
                }
                else
                {
                    result = await ApiClient.Client.DeleteAlbumAsync(Id);
                }
            }
            else
            {
                result = await ApiClient.Client.DeleteImageAsync(Id);
            }

            if(result == true)
            {
                MessageBus.ViewModelMessageBus.Instance.SendMessage(new MessageBus.ItemDeleteMessage(Id, IsAlbum));
            }
            return result;

            Windows.UI.Xaml.Controls.ContentDialog CreateConfirmDialog()
            {
                Windows.UI.Xaml.Controls.ContentDialog confirmDialog = new Windows.UI.Xaml.Controls.ContentDialog();
                confirmDialog.Title = "This is a single-image album";
                confirmDialog.Content = "Do you want to keep that image inside?";
                confirmDialog.PrimaryButtonText = "Yes";
                confirmDialog.SecondaryButtonText = "No";
                confirmDialog.CloseButtonText = "Cancel";
                confirmDialog.DefaultButton = Windows.UI.Xaml.Controls.ContentDialogButton.Primary;
                return confirmDialog;
            }
        }

        async Task<bool> FavoriteItem()
        {
            bool result;
            if (this.IsAlbum)
            {
                result = await ApiClient.Client.FavoriteAlbumAsync(Id);
            }
            else
            {
                result = await ApiClient.Client.FavoriteImageAsync(Id);
            }
            Favorite = result;
            MessageBus.ViewModelMessageBus.Instance.SendMessage(new MessageBus.FavoriteChangedMessage(Id, IsAlbum, Favorite, this.Item));
            return result;
        }
        void Share()
        {
            var transferMgr = DataTransferManager.GetForCurrentView();
            transferMgr.DataRequested += TransferMgr_DataRequested;
            DataTransferManager.ShowShareUI();
        }
        async Task<object> OpenBrowser()
        {
            await Windows.System.Launcher.LaunchUriAsync(new Uri(Link));
            return null;
        }

        private void TransferMgr_DataRequested(DataTransferManager sender, DataRequestedEventArgs args)
        {
            var request = args.Request;
            request.Data.SetWebLink(new Uri(Link));
            request.Data.Properties.Title = string.IsNullOrWhiteSpace(Title) ? "Take a look at this on Imgur!" : $"{Title} - Imgur";
        }

        public ItemViewModel()
        {
            DownloadCommand = new AsyncCommand<object>(Download);
            CopyUrlCommand = new Command(CopyUrl);
            DeleteCommand = new AsyncCommand<bool>(Delete);
            FavoriteItemCommand = new AsyncCommand<bool>(FavoriteItem);
            ShareCommand = new Command(Share);
            OpenBrowserCommand = new AsyncCommand<object>(OpenBrowser);
        }
        public ItemViewModel(IItem item) : this()
        {
            Item = item;
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
