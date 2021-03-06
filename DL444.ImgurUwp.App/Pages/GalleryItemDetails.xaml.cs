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
using System.Threading;
using System.Threading.Tasks;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace DL444.ImgurUwp.App.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class GalleryItemDetails : Page, INotifyPropertyChanged
    {
        readonly Func<ViewModels.MessageBus.CommentPostMessage, bool> commentPostHandler;
        readonly Func<ViewModels.MessageBus.CommentDeleteMessage, bool> commentDeleteHandler;

        GalleryItemViewModel ViewModel { get; set; }
        ObservableCollection<ImageViewModel> Images { get; } = new ObservableCollection<ImageViewModel>();
        ObservableCollection<CommentViewModel> Comments { get; set; } = new ObservableCollection<CommentViewModel>();
        ObservableCollection<TagViewModel> Tags { get; set; } = new ObservableCollection<TagViewModel>();
        Visibility TagBarVisibility { get; set; } = Visibility.Collapsed;
        IncrementalLoadingCollection<GalleryDetailsSidebarItemSource, GalleryItemViewModel> GalleryVm { get; set; }
        
        public GalleryItemDetails()
        {
            this.InitializeComponent();
            SubItemTemplateSelector.ImageTemplate = this.Resources["ImageTemplate"] as DataTemplate;
            SubItemTemplateSelector.VideoTemplate = this.Resources["VideoTemplate"] as DataTemplate;
            CommentReactionImageTemplateSelector.ImageTemplate = this.Resources["CommentImageTemplate"] as DataTemplate;
            CommentReactionImageTemplateSelector.VideoTemplate = this.Resources["CommentVideoTemplate"] as DataTemplate;

            commentPostHandler = new Func<ViewModels.MessageBus.CommentPostMessage, bool>(x =>
            {
                if (x.Comment.ImageId != ViewModel.Id) { return false; }
                if (x.Comment.ParentId == 0)
                {
                    Comments.Insert(0, new CommentViewModel(x.Comment));
                    var listControl = FindChildByName(CommentTree, "ListControl") as ListViewBase;
                    listControl.ScrollIntoView(CommentTree.RootNodes.FirstOrDefault());
                    return true;
                }
                else
                {
                    var comment = CommentFindRecursive(Comments, c => c.Id == x.Comment.ParentId);
                    if(comment == null) { return false; }
                    comment.Children.Insert(0, new CommentViewModel(x.Comment));
                    return true;
                }
            });
            ViewModels.MessageBus.ViewModelMessageBus.Instance.RegisterListener(new ViewModels.MessageBus.CommentPostMessageListener(commentPostHandler));
            commentDeleteHandler = new Func<ViewModels.MessageBus.CommentDeleteMessage, bool>(x =>
            {
                if(x.ImageId != ViewModel.Id) { return false; }
                if(!x.IsVirtualDelete)
                {
                    return CommentDeleteRecursive(Comments, i => i.Id == x.Id);
                }
                else
                {
                    return false;
                }
            });
            ViewModels.MessageBus.ViewModelMessageBus.Instance.RegisterListener(new ViewModels.MessageBus.CommentDeleteMessageListener(commentDeleteHandler));
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            if(e.Parameter is GalleryItemDetailsNavigationParameter navParams)
            {
                GalleryDetailsSidebarItemSource itemSource = new GalleryDetailsSidebarItemSource(navParams.GalleryItems);
                GalleryVm = new IncrementalLoadingCollection<GalleryDetailsSidebarItemSource, GalleryItemViewModel>(itemSource);
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(GalleryVm)));
                await PrepareViewModels(navParams.Item);
            }
            else if(e.Parameter is string id && !string.IsNullOrWhiteSpace(id))
            {
                GalleryItemViewModel vm = new GalleryItemViewModel(await ApiClient.Client.GetGalleryItemAsync(id));
                GalleryDetailsSidebarItemSource itemSource = new GalleryDetailsSidebarItemSource(null);
                GalleryVm = new IncrementalLoadingCollection<GalleryDetailsSidebarItemSource, GalleryItemViewModel>(itemSource);
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(GalleryVm)));
                await PrepareViewModels(vm);
            }
        }

        async Task PrepareViewModels(GalleryItemViewModel vm)
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
            // Turns out not always.
            if (vm.IsAlbum && vm.ImageCount > Images.Count)
            {
                var fullAlbum = await ApiClient.Client.GetGalleryAlbumAsync(vm.Id);
                for (int i = Images.Count; i < fullAlbum.ImageCount; i++)
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

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            if(e.NavigationMode == NavigationMode.New)
            {
                PageStackEntry backStackEntry = new PageStackEntry(typeof(GalleryItemDetails), new GalleryItemDetailsNavigationParameter(ViewModel, GalleryVm.Source.Source), new Windows.UI.Xaml.Media.Animation.EntranceNavigationTransitionInfo());
                Navigation.ContentFrame.BackStack.RemoveAt(Navigation.ContentFrame.BackStack.Count - 1);
                Navigation.ContentFrame.BackStack.Add(backStackEntry);
            }
        }

        private async void GalleryList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var item = e.AddedItems[0] as GalleryItemViewModel;

            await PrepareViewModels(item);
        }

        CommentViewModel CommentFindRecursive(IEnumerable<CommentViewModel> comments, Predicate<CommentViewModel> predicate)
        {
            if (comments == null) { return null; }
            foreach (var c in comments)
            {
                if (predicate(c) == true) { return c; }
                var childCandidate = CommentFindRecursive(c.Children, predicate);
                if (childCandidate == null) { continue; }
                else { return childCandidate; }
            }
            return null;
        }
        bool CommentDeleteRecursive(IList<CommentViewModel> comments, Func<CommentViewModel, bool> predicate)
        {
            if(comments == null) { return false; }
            var item = comments.FirstOrDefault(predicate);
            if (item != null)
            {
                comments.Remove(item);
                return true;
            }
            else
            {
                foreach(var c in comments)
                {
                    if(CommentDeleteRecursive(c.Children, predicate) == true) { return true; }
                }
                return false;
            }
        }
        DependencyObject FindChildByName(DependencyObject parant, string controlName)
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parant); i++)
            {
                var child = VisualTreeHelper.GetChild(parant, i);
                if (child is FrameworkElement && ((FrameworkElement)child).Name == controlName)
                {
                    return child;
                }

                var result = FindChildByName(child, controlName);
                if (result != null) { return result; }
            }
            return null;
        }
    }

    class GalleryDetailsSidebarItemSource : Microsoft.Toolkit.Collections.IIncrementalSource<GalleryItemViewModel>
    {
        public IncrementalItemsSource<GalleryItemViewModel> Source { get; }
        public GalleryDetailsSidebarItemSource(IncrementalItemsSource<GalleryItemViewModel> source)
        {
            if(source == null)
            {
                Source = new GalleryIncrementalSource(ImgurUwp.ApiClient.DisplayParams.Section.Hot, ImgurUwp.ApiClient.DisplayParams.Sort.Viral);
            }
            else
            {
                this.Source = source;
            }
        }

        public async Task<IEnumerable<GalleryItemViewModel>> GetPagedItemsAsync(int pageIndex, int pageSize, CancellationToken cancellationToken = default(CancellationToken))
        {
            if(Source == null) { return null; }
            return await Source.GetPagedItemsAsync(pageIndex, pageSize, cancellationToken);
        }
    }

    class GalleryItemDetailsNavigationParameter
    {
        public GalleryItemDetailsNavigationParameter(GalleryItemViewModel item, IncrementalItemsSource<GalleryItemViewModel> galleryItems)
        {
            Item = item;
            GalleryItems = galleryItems;
        }

        public GalleryItemViewModel Item { get; set; }
        public IncrementalItemsSource<GalleryItemViewModel> GalleryItems { get; set; }
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
                if(i.IsAnimated) { return VideoTemplate; }
                else { return ImageTemplate; }
            }
            else
            {
                return base.SelectTemplateCore(item);
            }
        }
    }

    public class CommentReactionImageTemplateSelector : DataTemplateSelector
    {
        public static DataTemplate ImageTemplate { get; set; }
        public static DataTemplate VideoTemplate { get; set; }

        protected override DataTemplate SelectTemplateCore(object item, DependencyObject container)
        {
            if (item is CommentReactionImage i)
            {
                if (i.IsVideo) { return VideoTemplate; }
                return ImageTemplate;
            }
            return base.SelectTemplateCore(item);
        }
    }
}
