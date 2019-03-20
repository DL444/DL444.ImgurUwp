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
using Windows.ApplicationModel.DataTransfer;
using Windows.ApplicationModel.DataTransfer.ShareTarget;
using System.ComponentModel;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace DL444.ImgurUwp.App.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Upload : Page
    {
        public Upload()
        {
            this.InitializeComponent();
            AlbumEditorTemplateSelector.ImageTemplate = this.Resources["ExistingImageDataTemplate"] as DataTemplate;
            AlbumEditorTemplateSelector.VideoTemplate = this.Resources["ExistingVideoDataTemplate"] as DataTemplate;
            AlbumEditorTemplateSelector.UploadTemplate = this.Resources["UploaderImageDataTemplate"] as DataTemplate;
        }

        ShareOperation shareOp = null;

        UploadViewModel ViewModel { get; set; } = new UploadViewModel();

        private void ImageList_DragOver(object sender, DragEventArgs e)
        {
            if(ViewModel.Images.Count < 50 && e.DataView.Contains(StandardDataFormats.StorageItems) || e.DataView.Contains(StandardDataFormats.Bitmap))
            {
                e.AcceptedOperation = DataPackageOperation.Copy;
                e.DragUIOverride.Caption = "Add";
                e.DragUIOverride.IsGlyphVisible = false;
            }
        }

        private async void ImageList_Drop(object sender, DragEventArgs e)
        {
            if (e.DataView.Contains(StandardDataFormats.StorageItems))
            {
                var items = await e.DataView.GetStorageItemsAsync();
                foreach(var i in items)
                {
                    if(i is Windows.Storage.StorageFile f)
                    {
                        string ext = f.Name.Substring(f.Name.LastIndexOf('.') + 1);
                        switch(ext)
                        {
                            case "jpg":
                            case "jpeg":
                            case "png":
                            case "gif":
                            case "tif":
                            case "tiff":
                                UploadImageViewModel imageVm = await UploadImageViewModel.CreateFromStreamAsync(await f.OpenStreamForReadAsync());
                                ViewModel.Images.Add(imageVm);
                                break;
                            default:
                                // TODO: Alert user
                                break;
                        }
                    }
                }
            }
            else if(e.DataView.Contains(StandardDataFormats.Bitmap))
            {
                var item = await e.DataView.GetBitmapAsync();
                if(item != null)
                {
                    UploadImageViewModel imageVm = await UploadImageViewModel.CreateFromStreamAsync((await item.OpenReadAsync()).AsStreamForRead());
                    ViewModel.Images.Add(imageVm);
                }
            }
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            if(e.Parameter is ShareOperation op)
            {
                shareOp = op;
                if(op.Data.Contains(StandardDataFormats.Bitmap))
                {
                    var item = await op.Data.GetBitmapAsync();
                    if (item != null)
                    {
                        UploadImageViewModel imageVm = await UploadImageViewModel.CreateFromStreamAsync((await item.OpenReadAsync()).AsStreamForRead());
                        ViewModel.Images.Add(imageVm);
                    }
                }
                else if(op.Data.Contains(StandardDataFormats.StorageItems))
                {
                    var items = await op.Data.GetStorageItemsAsync();
                    foreach (var i in items)
                    {
                        if (i is Windows.Storage.StorageFile f)
                        {
                            UploadImageViewModel imageVm = await UploadImageViewModel.CreateFromStreamAsync(await f.OpenStreamForReadAsync());
                            ViewModel.Images.Add(imageVm);
                        }
                    }
                }
                else
                {
                    shareOp = null;
                    op.ReportError("You can only share images to Imgur.");
                    // TODO: Localize
                }
                op.ReportDataRetrieved();
                // After uploading, you will have to call methods to notify completion.
                // See https://docs.microsoft.com/en-us/windows/uwp/app-to-app/receive-data
            }
            else if(e.Parameter is AccountAlbumViewModel vm)
            {

            }
            else if(e.Parameter is string albumId)
            {

            }
        }

        private async void RemoveImageBtn_Click(object sender, RoutedEventArgs e)
        {
            ImageViewModel model = (sender as FrameworkElement).Tag as ImageViewModel;
            if(model.Uploaded)
            {
                var result = await ApiClient.Client.EditAlbumImageAsync(ViewModel.AlbumId, new[] { model.Id }, ImgurUwp.ApiClient.AlbumEditMode.Remove);
                if(result == false)
                {
                    // TODO: Notify user.
                    return;
                }
            }
            ViewModel.Images.Remove(model);
        }
    }

    class AlbumEditorTemplateSelector : DataTemplateSelector
    {
        public static DataTemplate ImageTemplate { get; set; }
        public static DataTemplate VideoTemplate { get; set; }
        public static DataTemplate UploadTemplate { get; set; }

        protected override DataTemplate SelectTemplateCore(object item)
        {
            if (item is ImageViewModel image)
            {
                if (!image.Uploaded) { return UploadTemplate; }
                else
                {
                    if (image.IsAnimated) { return VideoTemplate; }
                    else { return ImageTemplate; }
                }
            }
            else
            {
                return base.SelectTemplateCore(item);
            }
        }
    }
}
