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
        }

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
    }
}
