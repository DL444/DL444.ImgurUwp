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
using System.ComponentModel;
using DL444.ImgurUwp.ApiClient;
using Microsoft.Toolkit.Uwp;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace DL444.ImgurUwp.App
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class GalleryView : Page, INotifyPropertyChanged
    {
        private GalleryCollectionViewModel _viewModel;

        IncrementalLoadingCollection<GalleryItemsSource, GalleryItemViewModel> ViewModel { get; set; }
            = new IncrementalLoadingCollection<GalleryItemsSource, GalleryItemViewModel>();

        private DisplayParams.Section _section;
        public DisplayParams.Section Section
        {
            get => _section;
            set
            {
                if(value != _section)
                {
                    _section = value;
                    // TODO: Request items.
                }
            }
        }
        private DisplayParams.Sort _sort;
        public DisplayParams.Sort Sort
        {
            get => _sort;
            set
            {
                if(value != _sort)
                {
                    _sort = value;
                    // TODO: Request items.
                }
            }
        }

        public GalleryView()
        {
            this.InitializeComponent();
        }


        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            //if(e.Parameter is DisplayParams.Section sect)
            //{
            //    var galleryItems = await ApiClient.Client.GetGalleryItemsAsync(DisplayParams.Sort.Viral, 0, sect);
            //    ViewModel = new GalleryCollectionViewModel(galleryItems);
            //}
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
