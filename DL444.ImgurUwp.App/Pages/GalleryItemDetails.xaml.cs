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
using DL444.ImgurUwp.Models;
using DL444.ImgurUwp.App.ViewModels;
using System.Collections.ObjectModel;
using System.ComponentModel;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace DL444.ImgurUwp.App.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class GalleryItemDetails : Page, INotifyPropertyChanged
    {
        GalleryItemViewModel ViewModel { get; set; }
        ObservableCollection<ImageViewModel> Images { get; } = new ObservableCollection<ImageViewModel>();
        ObservableCollection<CommentViewModel> Comments { get; set; } = new ObservableCollection<CommentViewModel>();
        ObservableCollection<TagViewModel> Tags { get; set; } = new ObservableCollection<TagViewModel>();
        Visibility TagBarVisibility { get; set; } = Visibility.Collapsed;
        GalleryCollectionViewModel GalleryVm { get; set; }
        
        public GalleryItemDetails()
        {
            this.InitializeComponent();
            SubItemTemplateSelector.ImageTemplate = this.Resources["ImageTemplate"] as DataTemplate;
            SubItemTemplateSelector.VideoTemplate = this.Resources["VideoTemplate"] as DataTemplate;
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            if(e.Parameter is ValueTuple<GalleryItemViewModel, GalleryCollectionViewModel> vms)
            {
                GalleryVm = vms.Item2;
                await PrepareViewModels(vms.Item1);
            }
        }

        async System.Threading.Tasks.Task PrepareViewModels(GalleryItemViewModel vm)
        {
            ViewModel = vm;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ViewModel)));
            Images.Clear();
            Comments.Clear();
            Tags.Clear();
            TagBarVisibility = Visibility.Collapsed;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(TagBarVisibility)));

            if (vm.IsAlbum)
            {
                var album = vm.Item as GalleryAlbum;
                foreach (var i in album.Images)
                {
                    Images.Add(new ImageViewModel(i));
                }
            }
            else
            {
                Images.Add(new ImageViewModel(vm.DisplayImage));
            }

            foreach (var t in vm.Tags)
            {
                Tags.Add(new TagViewModel(t));
            }

            if (Tags.Count > 0)
            {
                TagBarVisibility = Visibility.Visible;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(TagBarVisibility)));
            }

            // The front page model only contains the first 3 images. If there's more, we would need to request them.
            if (vm.IsAlbum && vm.ImageCount > 3)
            {
                var fullAlbum = await ApiClient.Client.GetGalleryAlbumAsync(vm.Id);
                for (int i = 3; i < fullAlbum.ImageCount; i++)
                {
                    Images.Add(new ImageViewModel(fullAlbum.Images[i]));
                }
            }

            var comments = await ApiClient.Client.GetGalleryCommentsAsync(vm.Id);
            Comments = new ObservableCollection<CommentViewModel>(CommentViewModelFactory.BuildCommentViewModels(comments));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Comments)));
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OpenCommentBtn_Click(object sender, RoutedEventArgs e)
        {
            RootSplitView.IsPaneOpen = true;
        }

        private async void GalleryList_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (object.ReferenceEquals(e.ClickedItem, ViewModel)) { return; }
            var item = e.ClickedItem as GalleryItemViewModel;

            await PrepareViewModels(item);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            if(e.NavigationMode == NavigationMode.New)
            {
                PageStackEntry backStackEntry = new PageStackEntry(typeof(GalleryItemDetails), (ViewModel, GalleryVm), new Windows.UI.Xaml.Media.Animation.EntranceNavigationTransitionInfo());
                Navigation.ContentFrame.BackStack.RemoveAt(Navigation.ContentFrame.BackStack.Count - 1);
                Navigation.ContentFrame.BackStack.Add(backStackEntry);
            }
        }
    }

    public class SubItemTemplateSelector : DataTemplateSelector
    {
        public static DataTemplate ImageTemplate { get; set; }
        public static DataTemplate VideoTemplate { get; set; }

        protected override DataTemplate SelectTemplateCore(object item)
        {
            // item: ViewModel, container: ListViewItem
            if (item is ImageViewModel i)
            {
                if(i.IsAnimated && i.Type != "image/gif") { return VideoTemplate; }
                else { return ImageTemplate; }
            }
            else
            {
                return base.SelectTemplateCore(item);
            }
        }
    }
}
