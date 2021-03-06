﻿using System;
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
    public sealed partial class Upload : Page, INotifyPropertyChanged
    {
        // TODO: Copy URL in Upload page. (For sharing scenarios)

        public Upload()
        {
            this.InitializeComponent();
            AlbumEditorTemplateSelector.ImageTemplate = this.Resources["ExistingImageDataTemplate"] as DataTemplate;
            AlbumEditorTemplateSelector.VideoTemplate = this.Resources["ExistingVideoDataTemplate"] as DataTemplate;
            AlbumEditorTemplateSelector.UploadTemplate = this.Resources["UploaderImageDataTemplate"] as DataTemplate;
            AlbumEditorTemplateSelector.UploadExistingTemplate = this.Resources["UploaderExistingImageDataTemplate"] as DataTemplate;
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
                                ViewModel.AddImage(imageVm);
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
                    ViewModel.AddImage(imageVm);
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
                        ViewModel.AddImage(imageVm);
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
                            ViewModel.AddImage(imageVm);
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
            else if(e.Parameter is AccountAlbumViewModel albumVm)
            {
                ViewModel = await UploadViewModel.CreateFromAccountAlbum(albumVm);
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ViewModel)));
            }
            else if(e.Parameter is UploadViewModel upVm)
            {
                ViewModel = upVm;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ViewModel)));
            }
            else if(e.Parameter is ItemViewModel itemVm)
            {
                if(itemVm.IsAlbum)
                {
                    ViewModel = UploadViewModel.CreateFromAlbum(itemVm.Item as Models.Album);
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ViewModel)));
                }
            }
            else if(e.Parameter is string albumId)
            {
                ViewModel = await UploadViewModel.CreateFromAlbumId(albumId);
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ViewModel)));
            }
            else if(e.Parameter is Models.Image img && img != null)
            {
                ViewModel.AddImage(new ImageViewModel(img));
            }
        }

        private void RemoveImageBtn_Click(object sender, RoutedEventArgs e)
        {
            ImageViewModel model = (sender as FrameworkElement).Tag as ImageViewModel;
            ViewModel.RemoveImage(model);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void NewTagBox_QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            if(ViewModel.AddTag(args.QueryText) == true)
            {
                sender.Text = "";
            }
        }

        private void RemoveTag_Click(object sender, RoutedEventArgs e)
        {
            string tag = (sender as FrameworkElement).Tag as string;
            ViewModel.RemoveTag(tag);
        }
    }

    class AlbumEditorTemplateSelector : DataTemplateSelector
    {
        public static DataTemplate ImageTemplate { get; set; }
        public static DataTemplate VideoTemplate { get; set; }
        public static DataTemplate UploadTemplate { get; set; }
        public static DataTemplate UploadExistingTemplate { get; set; }

        protected override DataTemplate SelectTemplateCore(object item)
        {
            if (item is ImageViewModel image)
            {
                if(item is UploadImageViewModel)
                {
                    return UploadTemplate;
                }
                else if(item is UploadExistingImageViewModel)
                {
                    return UploadExistingTemplate;
                }
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
