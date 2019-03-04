using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DL444.ImgurUwp.Models;

namespace DL444.ImgurUwp.App.ViewModels
{
    public class ImageViewModel : INotifyPropertyChanged
    {
        Image _image;

        public Image Image
        {
            get => _image;
            set
            {
                _image = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(null));
            }
        }

        public string Id => Image.Id;
        public string Title => Image.Title;
        public string Description => Image.Description;
        public DateTime DateTime => Convert.ToDateTime(Image.DateTime);
        public string Type => Image.Type;
        public bool IsAnimated => Image.Animated;
        public int Width => Image.Width;
        public int Height => Image.Height;
        public int Size => Image.Size;
        public int Views => Image.Views;
        public long Bandwidth => Image.Bandwidth;
        public string DeleteHash => Image.DeleteHash;
        public string Name => Image.Name;
        public string Section => Image.Section;
        public string Link => Image.Link;
        public string Gifv => Image.Gifv;
        public string Mp4 => Image.Mp4;
        public int Mp4Size => Image.Mp4Size;
        public bool Looping => Image.Looping;
        public bool Favorite => Image.Favorite;
        public bool Nsfw => Image.Nsfw == true;
        public string Vote => Image.Vote;
        public bool InGallery => Image.InGallery;

        public Command CopyUrlCommand { get; private set; }
        public Command ShareCommand { get; private set; }
        public AsyncCommand<object> DownloadCommand { get; private set; }

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
        async Task<object> Download()
        {
            var pictureLib = await Windows.Storage.StorageLibrary.GetLibraryAsync(Windows.Storage.KnownLibraryId.Pictures);
            var defaultFolder = pictureLib.SaveFolder;

            using (var imageStream = System.IO.WindowsRuntimeStreamExtensions.AsInputStream(await ApiClient.Client.DownloadMediaAsync(Link)))
            {
                string filename = Link.Substring(Link.LastIndexOf('/') + 1);
                var file = await defaultFolder.CreateFileAsync(filename);
                using (var fileStream = await file.OpenAsync(Windows.Storage.FileAccessMode.ReadWrite))
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
            request.Data.Properties.Title = $"Image from Imgur";
        }

        public ImageViewModel() { }
        public ImageViewModel(Image image)
        {
            Image = image;
            CopyUrlCommand = new Command(CopyUrl);
            ShareCommand = new Command(Share);
            DownloadCommand = new AsyncCommand<object>(Download);
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
