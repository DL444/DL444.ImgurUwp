using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Media.Imaging;

namespace DL444.ImgurUwp.App.ViewModels
{
    class UploadViewModel : INotifyPropertyChanged
    {
        public const int ImageSizeLimit = 10 * 1024 * 1024;
        private string _title;
        private string _albumId;
        private bool _albumCreated;
        private bool _uploading;
        private double _progress;
        private bool _canAddTag = true;
        private string originalTitle;

        public ObservableCollection<string> Tags { get; private set; } = null;
        public List<ImageViewModel> DeleteList { get; } = new List<ImageViewModel>();

        public string Title
        {
            get => _title;
            set
            {
                _title = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Title)));
            }
        }
        public string AlbumId
        {
            get => _albumId;
            set
            {
                _albumId = value;
                AlbumCreated = AlbumId != null;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(AlbumId)));
            }
        }
        public bool AlbumCreated
        {
            get => _albumCreated;
            set
            {
                _albumCreated = value;
                if(Tags == null) { Tags = new ObservableCollection<string>(); }
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(AlbumCreated)));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Tags)));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CanUpload)));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CanSave)));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CanPostToGallery)));
            }
        }
        public bool Uploading
        {
            get => _uploading;
            set
            {
                _uploading = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Uploading)));
            }
        }
        public double Progress
        {
            get => _progress;
            set
            {
                _progress = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Progress)));
            }
        }
        public bool CanAddTag
        {
            get => _canAddTag;
            set
            {
                _canAddTag = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CanAddTag)));
            }
        }

        public bool IsMature { get; set; }


        public bool CanUpload => !AlbumCreated;
        public bool CanSave => AlbumCreated;
        public bool CanPostToGallery => AlbumCreated;

        public ObservableCollection<ImageViewModel> Images { get; } = new ObservableCollection<ImageViewModel>();
        public bool HasImage => Images.Count > 0;

        public bool AddImage(ImageViewModel image)
        {
            if(Images.Count < 50)
            {
                Images.Add(image);
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(HasImage)));
                return true;
            }
            else { return false; }
        }
        public bool RemoveImage(ImageViewModel image)
        {
            if (image.Uploaded) { DeleteList.Add(image); }
            var result = Images.Remove(image);
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(HasImage)));
            return result;
        }

        public AsyncCommand<bool> PickImageCommand { get; private set; }
        public AsyncCommand<string> UploadImagesCommand { get; private set; }
        public AsyncCommand<bool> PostToGalleryCommand { get; private set; }

        async Task<bool> PickImage()
        {
            bool allSuccess = true;
            var picker = GetFilePicker();
            var files = await picker.PickMultipleFilesAsync();
            foreach(var f in files)
            {
                ulong size = (await f.GetBasicPropertiesAsync()).Size;
                if(size > ImageSizeLimit)
                {
                    allSuccess = false;
                    continue;
                }

                UploadImageViewModel imageVm = await UploadImageViewModel.CreateFromStreamAsync(await f.OpenStreamForReadAsync());
                AddImage(imageVm);
            }
            PickImageCommand.RaiseCanExecuteChanged();
            return allSuccess;
        }
        async Task<string> UploadImages()
        {
            // TODO: Imgur does not return single-image albums. This is a design choice, and many users expect this behavior. 
            // So we should try to avoid creating single-image albums.
            // But if an album BECOMES single-image, there's no need to worry, since we could still get access to them, with Items endpoint.
            Progress = 0;
            Uploading = true;
            if(!AlbumCreated)
            {
                (string id, _) = await ApiClient.Client.CreateAlbumAsync(title: Title);
                AlbumId = id;
                originalTitle = Title;
                ViewModelCacheManager.Instance.InvalidateCache<AccountContentPageViewModel>(x => x.IsOwner);
            }
            else if(originalTitle != Title)
            {
                await ApiClient.Client.UpdateAlbumInfoAsync(AlbumId, title: Title);
                // TODO: Notify
            }

            double progressStep = 0;
            int uploadCount = Images.Count(x => !x.Uploaded);
            if(uploadCount != 0) { progressStep = 100.0 / uploadCount; }

            foreach(var i in Images)
            {
                if(i is UploadImageViewModel upload)
                {
                    if(!i.Uploaded)
                    {
                        await upload.Upload(AlbumId);
                        Progress += progressStep;
                        continue;
                    }
                    else
                    {
                        i.Image.Description = upload.PreviewDescription;
                    }
                }
                if(i.DescriptionChanged)
                {
                    await ApiClient.Client.UpdateImageInfoAsync(i.Id, description: i.Description);
                }
            }

            if(DeleteList.Any())
            {
                var deleteResult = await ApiClient.Client.EditAlbumImageAsync(AlbumId, DeleteList.Select(x => x.Id), ImgurUwp.ApiClient.AlbumEditMode.Remove);
                if (deleteResult == true) { DeleteList.Clear(); }
            }
            Uploading = false;
            return AlbumId;
        }
        async Task<bool> PostToGallery()
        {
            await UploadImages();
            Uploading = true;
            var result = await ApiClient.Client.PostToGalleryAsync(AlbumId, Title, null, IsMature, Tags);
            var item = await ApiClient.Client.GetGalleryItemAsync(AlbumId);
            Uploading = false;
            MessageBus.ViewModelMessageBus.Instance.SendMessage(new MessageBus.GalleryPostMessage(item));
            Navigation.Navigate(typeof(Pages.GalleryItemDetails), new Pages.GalleryItemDetailsNavigationParameter(new GalleryItemViewModel(item), null));
            return result;
        }
        public bool AddTag(string tag)
        {
            if (string.IsNullOrWhiteSpace(tag)) { return false; }
            if(CanAddTag && !Tags.Contains(tag))
            {
                Tags.Add(tag);
                if (Tags.Count >= 5) { CanAddTag = false; }
                return true;
            }
            else
            {
                return false;
            }
        }
        public void RemoveTag(string tag)
        {
            if(Tags.Remove(tag) == true)
            {
                if(Tags.Count < 5) { CanAddTag = true; }
            }
        }

        public UploadViewModel()
        {
            PickImageCommand = new AsyncCommand<bool>(PickImage, () => Images.Count < 50);
            UploadImagesCommand = new AsyncCommand<string>(UploadImages);
            PostToGalleryCommand = new AsyncCommand<bool>(PostToGallery);
        }

        public static async Task<UploadViewModel> CreateFromAccountAlbum(AccountAlbumViewModel accountVm)
        {
            return await CreateFromAlbumId(accountVm.Id);
        }
        public static async Task<UploadViewModel> CreateFromAlbumId(string accountId)
        {
            var result = new UploadViewModel();
            var album = await ApiClient.Client.GetAlbumAsync(accountId);
            result.AlbumCreated = true;
            result.AlbumId = album.Id;
            result.originalTitle = album.Title;
            result.Title = album.Title;
            foreach (var i in album.Images)
            {
                result.AddImage(new ImageViewModel(i));
            }
            return result;
        }

        Windows.Storage.Pickers.FileOpenPicker GetFilePicker()
        {
            Windows.Storage.Pickers.FileOpenPicker picker = new Windows.Storage.Pickers.FileOpenPicker();
            picker.ViewMode = Windows.Storage.Pickers.PickerViewMode.Thumbnail;
            picker.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.PicturesLibrary;
            picker.FileTypeFilter.Add(".jpg");
            picker.FileTypeFilter.Add(".jpeg");
            picker.FileTypeFilter.Add(".png");
            picker.FileTypeFilter.Add(".gif");
            picker.FileTypeFilter.Add(".tif");
            picker.FileTypeFilter.Add(".tiff");
            return picker;
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }

    public class UploadImageViewModel : ImageViewModel, INotifyPropertyChanged, IDisposable
    {
        private Stream _imageStream;
        private BitmapImage _image;
        private string _description;

        public Stream ImageStream
        {
            get => _imageStream;
            set
            {
                _imageStream = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ImageStream)));
            }
        }
        public BitmapImage PreviewImage
        {
            get => _image;
            set
            {
                _image = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(PreviewImage)));
            }
        }
        public string PreviewDescription
        {
            get => _description;
            set
            {
                _description = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(PreviewDescription)));
            }
        }
        public override bool Uploaded => Image != null;

        public async Task<Models.Image> Upload(string albumId = null)
        {
            Image = await ApiClient.Client.UploadImageAsync(ImageStream, albumId, description: PreviewDescription);
            return Image;
        }

        public new event PropertyChangedEventHandler PropertyChanged;

        private UploadImageViewModel() { }

        public static async Task<UploadImageViewModel> CreateFromStreamAsync(Stream imageStream)
        {
            UploadImageViewModel imageVm = new UploadImageViewModel();
            imageVm.ImageStream = imageStream;
            var image = new BitmapImage();
            await image.SetSourceAsync(imageStream.AsRandomAccessStream());
            imageVm.PreviewImage = image;
            return imageVm;
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    ImageStream.Dispose();
                }

                ImageStream = null;
                PreviewImage = null;
                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }
        #endregion
    }
}
