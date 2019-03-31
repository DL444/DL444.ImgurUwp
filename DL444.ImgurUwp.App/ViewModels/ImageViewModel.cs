using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DL444.ImgurUwp.Models;

namespace DL444.ImgurUwp.App.ViewModels
{
    public class ImageViewModel : INotifyPropertyChanged, IReportable
    {
        Image _image;
        string originalDescription;

        public Image Image
        {
            get => _image;
            set
            {
                _image = value;
                if (_image != null)
                {
                    string link = _image.Link;
                    if (string.IsNullOrWhiteSpace(link)) { Thumbnail = null; }
                    else
                    {
                        if (_image.Animated)
                        {
                            Thumbnail = $"{_image.Link.Remove(_image.Link.LastIndexOf('/'))}/{_image.Id}_d.jpg?maxwidth=520&shape=thumb&fidelity=mid";
                            HugeThumbnail = _image.Link;
                        }
                        else
                        {
                            Thumbnail = $"{_image.Link.Replace(_image.Id, $"{_image.Id}_d")}?maxwidth=520&shape=thumb&fidelity=mid";
                            if (_image.Size > 512000)
                            {
                                // This is how imgur.com is pulling this off. Just add 'r'.
                                HugeThumbnail = _image.Link.Replace(_image.Id, $"{_image.Id}r");
                            }
                            else
                            {
                                HugeThumbnail = _image.Link;
                            }
                        }
                    }
                    RichDescriptionBox = RichTextParser.GetRichContentBox(Description);
                    originalDescription = Description;
                }
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(null));
            }
        }

        public string Id => Image.Id;
        public string Title => Image.Title;
        public string Description
        {
            get => Image.Description;
            set
            {
                Image.Description = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Image)));
            }
        }
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
        public bool Favorite => Image.Favorite == true;
        public bool Nsfw => Image.Nsfw == true;
        public string Vote => Image.Vote;
        public bool InGallery => Image.InGallery;

        public bool DescriptionChanged => !(originalDescription == Description);
        public virtual bool Uploaded => true;

        public string Thumbnail { get; private set; }
        public string HugeThumbnail { get; private set; }
        public bool HasDescription => !string.IsNullOrWhiteSpace(Description);
        public Windows.UI.Xaml.FrameworkElement RichDescriptionBox { get; private set; }


        public Command CopyUrlCommand { get; private set; }
        public Command ShareCommand { get; private set; }
        public AsyncCommand<object> DownloadCommand { get; private set; }
        public AsyncCommand<bool> ReportCommand { get; private set; }

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
            string filename = Link.Substring(Link.LastIndexOf('/') + 1);
            using (var imageStream = System.IO.WindowsRuntimeStreamExtensions.AsInputStream(await ApiClient.Client.DownloadMediaAsync(Link)))
            {
                await CommonOperations.Save(imageStream, filename);
            }
            return null;
        }
        async Task<bool> Report()
        {
            Controls.ReportConfirmDialog dialog = new Controls.ReportConfirmDialog(this);
            var result = await dialog.ShowAsync();
            if (result == Windows.UI.Xaml.Controls.ContentDialogResult.Primary)
            {
                return await ApiClient.Client.ReportGalleryItemAsync(this.Id, dialog.SelectedReason);
            }
            return true;
        }
        private void TransferMgr_DataRequested(Windows.ApplicationModel.DataTransfer.DataTransferManager sender, Windows.ApplicationModel.DataTransfer.DataRequestedEventArgs args)
        {
            var request = args.Request;
            request.Data.SetWebLink(new Uri($"https://imgur.com/{Id}"));
            request.Data.Properties.Title = $"Image from Imgur";
        }

        public ImageViewModel()
        {
            CopyUrlCommand = new Command(CopyUrl);
            ShareCommand = new Command(Share);
            DownloadCommand = new AsyncCommand<object>(Download);
            ReportCommand = new AsyncCommand<bool>(Report);
        }
        public ImageViewModel(Image image) : this()
        {
            Image = image;
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
