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

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace DL444.ImgurUwp.App.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class NonGalleryItemDetails : Page
    {
        public NonGalleryItemDetails()
        {
            this.InitializeComponent();
            SubItemTemplateSelector.ImageTemplate = this.Resources["ImageTemplate"] as DataTemplate;
            SubItemTemplateSelector.VideoTemplate = this.Resources["VideoTemplate"] as DataTemplate;
        }

        NonGalleryItemDetailsPageViewModel ViewModel { get; set; }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if(e.Parameter is ItemViewModel vm)
            {
                ViewModel = await NonGalleryItemDetailsPageViewModel.CreateFromItem(vm);
                Bindings.Update();
            }
        }
    }
}
