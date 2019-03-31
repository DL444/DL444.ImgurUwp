using DL444.ImgurUwp.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media.Imaging;

namespace DL444.ImgurUwp.App.ViewModels
{
    class AccountAlbumViewModel : INotifyPropertyChanged
    {
        private Album _album;
        public Album Album
        {
            get => _album;
            set
            {
                _album = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(null));
            }
        }

        public string Id => Album.Id;
        public string Title
        {
            get => Album.Title;
            set
            {
                Album.Title = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Title)));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(HasTitle)));
            }
        }
        public string Description => Album.Description;
        public DateTime DateTime => Convert.ToDateTime(Album.DateTime);
        public string Cover => Album.Cover;
        public int CoverWidth => Album.CoverWidth ?? 0;
        public int CoverHeight => Album.CoverHeight ?? 0;
        public string AccountUrl => Album.AccountUrl;
        public string AccountId => Album.AccountId;
        public string Privacy => Album.Privacy;
        public int Views => Album.Views;
        public string Link => Album.Link;
        public bool Favorite
        {
            get => Album.Favorite == true;
            set
            {
                Album.Favorite = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Favorite)));
            }
        }

        public bool Nsfw => Album.Nsfw == true;
        public int Order => Album.Order;
        public string DeleteHash => Album.DeleteHash;
        public int ImageCount => Album.ImageCount;
        public List<Image> Images => Album.Images;
        public bool InGallery => Album.InGallery;
        public bool IsAlbum => true;

        public bool HasImage => ImageCount > 0;
        public BitmapImage Thumbnail
        {
            get
            {
                if (string.IsNullOrEmpty(Cover)) { return null; }
                return new BitmapImage(new Uri($"https://i.imgur.com/{Cover}h.jpg"));
            }
        }
        public bool IsAnimated
        {
            get
            {
                if(Images == null || Images.Count == 0) { return false; }
                return Images[0].Animated;
            }
        }
        public bool HasTitle => !string.IsNullOrEmpty(Title);

        public AsyncCommand<bool> DeleteAlbumCommand { get; private set; }
        public AsyncCommand<bool> GalleryRemoveCommand { get; private set; }
        public AsyncCommand<bool> FavoriteItemCommand { get; private set; }
        public Command ShareCommand { get; private set; }
        public Command CopyUrlCommand { get; private set; }
        //public Command DownloadCommand { get; private set; }
        public AsyncCommand<object> OpenBrowserCommand { get; private set; }

        async Task<bool> DeleteAlbum()
        {
            if (InGallery) { return false; }
            var result = await ApiClient.Client.DeleteAlbumAsync(Id);
            if(result == true)
            {
                MessageBus.ViewModelMessageBus.Instance.SendMessage(new MessageBus.ItemDeleteMessage(Id, IsAlbum));
            }
            return result;
        }
        async Task<bool> GalleryRemove()
        {
            var result = await ApiClient.Client.RemoveGalleryItemAsync(Id);
            if(result == true)
            {
                // TODO: Implement
            }
            return result;
        }
        async Task<bool> FavoriteItem()
        {
            bool result = await ApiClient.Client.FavoriteAlbumAsync(Id);
            Favorite = result;
            MessageBus.ViewModelMessageBus.Instance.SendMessage(new MessageBus.FavoriteChangedMessage(Id, IsAlbum, Favorite, this.Album));
            return result;
        }
        void Share()
        {
            var transferMgr = Windows.ApplicationModel.DataTransfer.DataTransferManager.GetForCurrentView();
            transferMgr.DataRequested += TransferMgr_DataRequested;
            Windows.ApplicationModel.DataTransfer.DataTransferManager.ShowShareUI();
        }
        void CopyUrl()
        {
            var package = new Windows.ApplicationModel.DataTransfer.DataPackage();
            package.SetText(Link);
            Windows.ApplicationModel.DataTransfer.Clipboard.SetContent(package);
        }
        async Task<object> Download()
        {
            // TODO: Implement

            //var pictureLib = await Windows.Storage.StorageLibrary.GetLibraryAsync(Windows.Storage.KnownLibraryId.Pictures);
            //var defaultFolder = pictureLib.SaveFolder;

            //using (var imageStream = System.IO.WindowsRuntimeStreamExtensions.AsInputStream(await ApiClient.Client.DownloadMediaAsync(DisplayImage.Link)))
            //{
            //    string filename = DisplayImage.Link.Substring(DisplayImage.Link.LastIndexOf('/') + 1);
            //    var file = await defaultFolder.CreateFileAsync(filename, Windows.Storage.CreationCollisionOption.GenerateUniqueName);
            //    using (var fileStream = (await file.OpenAsync(Windows.Storage.FileAccessMode.ReadWrite)))
            //    {
            //        await Windows.Storage.Streams.RandomAccessStream.CopyAndCloseAsync(imageStream, fileStream);
            //    }
            //}
            return null;
        }
        async Task<object> OpenBrowser()
        {
            await Windows.System.Launcher.LaunchUriAsync(new Uri(Link));
            return null;
        }

        public AccountAlbumViewModel()
        {
            DeleteAlbumCommand = new AsyncCommand<bool>(DeleteAlbum);
            GalleryRemoveCommand = new AsyncCommand<bool>(GalleryRemove);
            FavoriteItemCommand = new AsyncCommand<bool>(FavoriteItem);
            ShareCommand = new Command(Share);
            CopyUrlCommand = new Command(CopyUrl);
            //DownloadCommand = new Command(Download);
            OpenBrowserCommand = new AsyncCommand<object>(OpenBrowser);
        }
        public AccountAlbumViewModel(Album album) : this()
        {
            Album = album;
        }

        private void TransferMgr_DataRequested(Windows.ApplicationModel.DataTransfer.DataTransferManager sender, Windows.ApplicationModel.DataTransfer.DataRequestedEventArgs args)
        {
            var request = args.Request;
            request.Data.SetWebLink(new Uri(Link));
            request.Data.Properties.Title = string.IsNullOrWhiteSpace(Title) ? "Take a look at this on Imgur!" : $"{Title} - Imgur";
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
