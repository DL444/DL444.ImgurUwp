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

        private GalleryItemViewModel _viewModel;
        GalleryItemViewModel ViewModel
        {
            get => _viewModel;
            set
            {
                _viewModel = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ViewModel)));
            }
        }

        private ObservableCollection<ImageViewModel> _images = new ObservableCollection<ImageViewModel>();
        ObservableCollection<ImageViewModel> Images
        {
            get => _images;
            set
            {
                _images = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Images)));
            }
        }

        private ObservableCollection<CommentViewModel> _comments = new ObservableCollection<CommentViewModel>();
        ObservableCollection<CommentViewModel> Comments
        {
            get => _comments;
            set
            {
                _comments = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Comments)));
            }
        }

        private ObservableCollection<TagViewModel> _tags = new ObservableCollection<TagViewModel>();
        ObservableCollection<TagViewModel> Tags
        {
            get => _tags;
            set
            {
                _tags = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Tags)));
            }
        }

        private Visibility _tagBarVisibility = Visibility.Collapsed;
        Visibility TagBarVisibility
        {
            get => _tagBarVisibility;
            set
            {
                _tagBarVisibility = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(TagBarVisibility)));
            }
        }

        GalleryCollectionViewModel GalleryVm { get; set; }
        
        public GalleryItemDetails()
        {
            this.InitializeComponent();
            SubItemTemplateSelector.ImageTemplate = this.Resources["ImageTemplate"] as DataTemplate;
            SubItemTemplateSelector.VideoTemplate = this.Resources["VideoTemplate"] as DataTemplate;
            this.NavigationCacheMode = NavigationCacheMode.Enabled;
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            if(e.Parameter is Tuple<GalleryItemViewModel, GalleryCollectionViewModel> vms)
            {
                var vm = vms.Item1;
                GalleryVm = vms.Item2;

                ViewModel = vm;
                await PopulateViewModels();
            }
        }

        private async System.Threading.Tasks.Task PopulateViewModels()
        {
            Images = new ObservableCollection<ImageViewModel>();
            Comments = new ObservableCollection<CommentViewModel>();
            Tags = new ObservableCollection<TagViewModel>();
            TagBarVisibility = Visibility.Collapsed;

            if (ViewModel.IsAlbum)
            {
                var album = ViewModel.Item as GalleryAlbum;
                foreach (var i in album.Images)
                {
                    Images.Add(new ImageViewModel(i));
                }
            }
            else
            {
                Images.Add(new ImageViewModel(ViewModel.DisplayImage));
            }

            // The front page model only contains the first 3 images. If there's more, we would need to request them.
            if (ViewModel.IsAlbum && ViewModel.ImageCount > 3)
            {
                var fullAlbum = await ApiClient.Client.GetGalleryAlbumAsync(ViewModel.Id);
                for (int i = 3; i < fullAlbum.ImageCount; i++)
                {
                    Images.Add(new ImageViewModel(fullAlbum.Images[i]));
                }
            }

            foreach (var t in ViewModel.Tags)
            {
                Tags.Add(new TagViewModel(t));
            }

            if (Tags.Count > 0)
            {
                TagBarVisibility = Visibility.Visible;
            }

            var comments = await ApiClient.Client.GetGalleryCommentsAsync(ViewModel.Id);
            Comments = new ObservableCollection<CommentViewModel>(CommentViewModelFactory.BuildCommentViewModels(comments));
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OpenCommentBtn_Click(object sender, RoutedEventArgs e)
        {
            RootSplitView.IsPaneOpen = true;
        }

        private async void GalleryList_ItemClick(object sender, ItemClickEventArgs e)
        {
            if(object.ReferenceEquals(e.ClickedItem, ViewModel)) { return; }
            ViewModel = e.ClickedItem as GalleryItemViewModel;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ViewModel)));
            await PopulateViewModels();
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
