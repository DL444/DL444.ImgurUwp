using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DL444.ImgurUwp.Models;

namespace DL444.ImgurUwp.App.ViewModels
{
    public class GalleryItemViewModel : INotifyPropertyChanged
    {
        IGalleryItem _item;
        private Image _displayImage;
        string _thumbnail;
        private bool _upvoted;
        private bool _downvoted;
        private bool _favorite;

        public IGalleryItem Item
        {
            get => _item;
            set
            {
                _item = value;
                if (value.IsAlbum)
                {
                    GalleryAlbum album = value as GalleryAlbum;
                    foreach (var i in album.Images)
                    {
                        if (i.Id == album.Cover)
                        {
                            DisplayImage = i;
                            break;
                        }
                    }
                }
                else
                {
                    DisplayImage = (value as GalleryImage).ToImage();
                }

                if (DisplayImage != null)
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

                _favorite = _item.Favorite;
                _upvoted = _item.Vote == "up";
                _downvoted = _item.Vote == "down";
                NotifyPropertyChanged();
            }
        }
        public bool IsAlbum => _item.IsAlbum;

        public string Id => _item.Id;
        public string Title => _item.Title;
        public string Description => _item.Description;
        public DateTime DateTime => Convert.ToDateTime(_item.DateTime);
        public string Link => _item.Link;
        public string AccountUrl => _item.AccountUrl;
        public string AccountId => _item.AccountId;
        public string Topic => _item.Topic;
        public int TopicId => _item.TopicId;
        public bool Nsfw => _item.Nsfw == true;
        public int CommentCount => _item.CommentCount;
        public int Ups
        {
            get => _item.Ups;
            set
            {
                _item.Ups = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Ups)));
            }
        }
        public int Downs
        {
            get => _item.Downs;
            set
            {
                _item.Downs = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Downs)));
            }
        }

        public bool Upvoted
        {
            get => _upvoted;
            set
            {
                _upvoted = value;
                if(value == true)
                {
                    Downvoted = false;
                }
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Upvoted)));
            }
        }
        public bool Downvoted
        {
            get => _downvoted;
            set
            {
                _downvoted = value;
                if (value == true)
                {
                    Upvoted = false;
                }
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Downvoted)));
            }
        }

        public int Points => _item.Points;
        public int Score => _item.Score;
        public int Views => _item.Views;
        public bool InMostViral => _item.InMostViral;
        public bool Favorite
        {
            get => _favorite;
            set
            {
                _favorite = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Favorite)));
            }
        }

        public int ImageCount
        {
            get
            {
                if(Item == null) { return 0; }
                else if(Item is GalleryImage) { return 1; }
                else if(Item is GalleryAlbum a) { return a.ImageCount; }
                else { return 0; }
            }
        }

        public string Thumbnail => _thumbnail;

        public Image DisplayImage
        {
            get => _displayImage;
            private set
            {
                _displayImage = value;
                NotifyPropertyChanged(nameof(DisplayImage));
            }
        }

        public GalleryItemViewModel() : this(Defaults.DefaultImage) { }
        public GalleryItemViewModel(IGalleryItem item)
        {
            Item = item;
            UpvoteCommand = new AsyncCommand<bool>(() => Vote(ImgurUwp.ApiClient.Vote.Up));
            DownvoteCommand = new AsyncCommand<bool>(() => Vote(ImgurUwp.ApiClient.Vote.Down));
            CopyUrlCommand = new Command(CopyUrl);
            ShareCommand = new Command(Share);
            OpenBrowserCommand = new AsyncCommand<object>(OpenBrowser);
            DownloadCommand = new AsyncCommand<object>(Download);
        }

        protected void NotifyPropertyChanged(string propertyName = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        public event PropertyChangedEventHandler PropertyChanged;

        public AsyncCommand<bool> UpvoteCommand { get; private set; }
        public AsyncCommand<bool> DownvoteCommand { get; private set; }

        public Command CopyUrlCommand { get; private set; }
        public Command ShareCommand { get; private set; }
        public AsyncCommand<object> OpenBrowserCommand { get; private set; }
        public AsyncCommand<object> DownloadCommand { get; private set; }

        public async Task<bool> Vote(ImgurUwp.ApiClient.Vote vote)
        {
            bool result = await ApiClient.Client.VoteGalleryItemAsync(Id, vote);
            Votes votes = await ApiClient.Client.GetGalleryVotesAsync(Id);
            Ups = votes.Ups;
            Downs = votes.Downs;
            return result;
        }

        void CopyUrl()
        {
            var package = new Windows.ApplicationModel.DataTransfer.DataPackage();
            package.SetText(Link);
            Windows.ApplicationModel.DataTransfer.Clipboard.SetContent(package);
        }

        void Share()
        {
            var transferMgr = Windows.ApplicationModel.DataTransfer.DataTransferManager.GetForCurrentView();
            transferMgr.DataRequested += TransferMgr_DataRequested;
            Windows.ApplicationModel.DataTransfer.DataTransferManager.ShowShareUI();
        }

        async Task<object> OpenBrowser()
        {
            await Windows.System.Launcher.LaunchUriAsync(new Uri($"https://imgur.com/{Id}"));
            return null;
        }

        async Task<object> Download()
        {
            var pictureLib = await Windows.Storage.StorageLibrary.GetLibraryAsync(Windows.Storage.KnownLibraryId.Pictures);
            var defaultFolder = pictureLib.SaveFolder;

            using (var imageStream = System.IO.WindowsRuntimeStreamExtensions.AsInputStream(await ApiClient.Client.DownloadMediaAsync(DisplayImage.Link)))
            {
                string filename = DisplayImage.Link.Substring(DisplayImage.Link.LastIndexOf('/') + 1);
                var file = await defaultFolder.CreateFileAsync(filename);
                using (var fileStream = (await file.OpenAsync(Windows.Storage.FileAccessMode.ReadWrite)))
                {
                    await Windows.Storage.Streams.RandomAccessStream.CopyAndCloseAsync(imageStream, fileStream);
                }
            }
            return null;
        }

        private void TransferMgr_DataRequested(Windows.ApplicationModel.DataTransfer.DataTransferManager sender, Windows.ApplicationModel.DataTransfer.DataRequestedEventArgs args)
        {
            var request = args.Request;
            request.Data.SetWebLink(new Uri($"https://imgur.com/{Id}"));
            request.Data.Properties.Title = $"{Title} - Imgur";
        }
    }

    public class FavoriteIconConverter : Windows.UI.Xaml.Data.IValueConverter
    {
        static string falseIcon = "\xE006";
        static string trueIcon = "\xE00B";

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            bool val = (bool)value;
            if(val == true)
            {
                return trueIcon;
            }
            else
            {
                return falseIcon;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotSupportedException();
        }
    }
}
