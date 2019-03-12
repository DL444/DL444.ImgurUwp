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
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Toolkit.Uwp;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace DL444.ImgurUwp.App.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Tag : Page, INotifyPropertyChanged
    {
        public Tag()
        {
            this.InitializeComponent();
        }

        TagViewModel ViewModel { get; set; }

        //ObservableCollection<GalleryItemViewModel> Items { get; } = new ObservableCollection<GalleryItemViewModel>();

        private IncrementalLoadingCollection<TagItemsSource, GalleryItemViewModel> _items;
        IncrementalLoadingCollection<TagItemsSource, GalleryItemViewModel> Items
        {
            get => _items;
            set
            {
                _items = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Items)));
            }
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            if(e.Parameter is TagViewModel vm)
            {
                ViewModel = vm;
                var tag = await ApiClient.Client.GetTagAsync(ViewModel.Name, ImgurUwp.ApiClient.DisplayParams.Sort.Time);
                // The tag info returned in gallery request may not reflect actual value (e.g. Following)
                ViewModel = new TagViewModel(tag);
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ViewModel)));
                FollowBtn.IsEnabled = true;

                var images = new List<GalleryItemViewModel>(tag.Items.Count);
                var items = tag.Items;
                foreach(var i in items)
                {
                    images.Add(new GalleryItemViewModel(i));
                }

                Items = new IncrementalLoadingCollection<TagItemsSource, GalleryItemViewModel>(new TagItemsSource(ViewModel.Name, images, 1));
            }
        }

        private void TagList_ItemClick(object sender, ItemClickEventArgs e)
        {
            var item = e.ClickedItem as GalleryItemViewModel;
            GalleryCollectionViewModel collection = new GalleryCollectionViewModel(Items);
            Navigation.Navigate(typeof(GalleryItemDetails), (item, collection));
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }

    class TagItemsSource : IncrementalItemsSource<GalleryItemViewModel>
    {
        public string Name { get; }
        public int Page { get; private set; }

        public TagItemsSource(string name) => Name = name ?? throw new ArgumentNullException(nameof(name));
        public TagItemsSource(string name, IEnumerable<GalleryItemViewModel> items, int startPage) : base(items)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            if (items == null) { throw new ArgumentNullException(nameof(items)); }
            Page = startPage;
        }

        protected override async Task<IEnumerable<GalleryItemViewModel>> GetItemsFromSourceAsync(CancellationToken cancellationToken)
        {
            var tag = await ApiClient.Client.GetTagAsync(Name, ImgurUwp.ApiClient.DisplayParams.Sort.Time, page: Page);
            List<GalleryItemViewModel> result = new List<GalleryItemViewModel>(tag.Items.Count);
            foreach(var i in tag.Items)
            {
                result.Add(new GalleryItemViewModel(i));
            }
            Page++;
            return result;
        }
    }
}