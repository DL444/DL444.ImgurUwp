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
    public class UploadViewModel : INotifyPropertyChanged
    {
        public const int ImageSizeLimit = 10 * 1024 * 1024;
        private string _title;

        public string Title
        {
            get => _title;
            set
            {
                _title = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Title)));
            }
        }
        public ObservableCollection<UploadImageViewModel> Images { get; } = new ObservableCollection<UploadImageViewModel>();

        public AsyncCommand<bool> PickImageCommand { get; private set; }

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
                Images.Add(imageVm);
            }
            PickImageCommand.RaiseCanExecuteChanged();
            return allSuccess;
        }

        public UploadViewModel()
        {
            PickImageCommand = new AsyncCommand<bool>(PickImage, () => Images.Count < 50);
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

    public class UploadImageViewModel : INotifyPropertyChanged, IDisposable
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
        public BitmapImage Image
        {
            get => _image;
            set
            {
                _image = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Image)));
            }
        }
        public string Description
        {
            get => _description;
            set
            {
                _description = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Description)));
            }
        }
        public Models.Image UploadedImage { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        private UploadImageViewModel() { }

        public static async Task<UploadImageViewModel> CreateFromStreamAsync(Stream imageStream)
        {
            UploadImageViewModel imageVm = new UploadImageViewModel();
            imageVm.ImageStream = imageStream;
            var image = new BitmapImage();
            await image.SetSourceAsync(imageStream.AsRandomAccessStream());
            imageVm.Image = image;
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
                Image = null;
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
