using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

namespace DL444.ImgurUwp.App.ViewModels
{
    class TagPageViewModel : CachingViewModel, INotifyPropertyChanged, IListViewPersistent
    {
        public TagViewModel ViewModel { get; set; }

        private IncrementalLoadingCollection<TagItemsSource, GalleryItemViewModel> _items;

        public IncrementalLoadingCollection<TagItemsSource, GalleryItemViewModel> Items
        {
            get => _items;
            set
            {
                _items = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Items)));
            }
        }

        public static async Task<TagPageViewModel> CreateFromTag(TagViewModel tag)
        {
            // The tag info returned in gallery request may not reflect actual value (e.g. Following)
            return await CreateFromTag(tag.Name);
        }
        public static async Task<TagPageViewModel> CreateFromTag(string tagName)
        {
            if (tagName == null) { throw new ArgumentNullException(nameof(tagName)); }
            var t = await ApiClient.Client.GetTagAsync(tagName, ImgurUwp.ApiClient.DisplayParams.Sort.Time);
            TagPageViewModel vm = new TagPageViewModel();
            vm.ViewModel = new TagViewModel(t);
            var images = new List<GalleryItemViewModel>(t.Items.Count);
            foreach (var i in t.Items)
            {
                images.Add(new GalleryItemViewModel(i));
            }
            vm.Items = new IncrementalLoadingCollection<TagItemsSource, GalleryItemViewModel>(new TagItemsSource(tagName, images, 1));
            return vm;
        }

        public string ScrollPosition { get; private set; }
        public string AnchorItemId => scrollViewAnchorItem == null ? null : scrollViewAnchorItem.Id;
        GalleryItemViewModel scrollViewAnchorItem;
        public void SetScrollPosition(ListViewBase listView)
        {
            ScrollPosition = ListViewPersistenceHelper.GetRelativeScrollPosition(listView, x =>
            {
                scrollViewAnchorItem = x as GalleryItemViewModel;
                return scrollViewAnchorItem.Id;
            });
        }
        public async Task RecoverScrollPosition(ListViewBase listView)
        {
            if(ScrollPosition == null) { return; }
            ListViewKeyToItemHandler keyToItemHandler = key =>
            {
                Func<CancellationToken, Task<object>> task = x =>
                {
                    object result;
                    if (scrollViewAnchorItem.Id == key)
                    {
                        result = scrollViewAnchorItem;
                    }
                    else
                    {
                        result = Items.FirstOrDefault(i => i.Id == key);
                    }
                    return Task.FromResult(result);
                };
                return System.Runtime.InteropServices.WindowsRuntime.AsyncInfo.Run(task);
            };
            await ListViewPersistenceHelper.SetRelativeScrollPositionAsync(listView, ScrollPosition, keyToItemHandler);
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
            Page = startPage;
        }

        protected override async Task<IEnumerable<GalleryItemViewModel>> GetItemsFromSourceAsync(CancellationToken cancellationToken)
        {
            var tag = await ApiClient.Client.GetTagAsync(Name, ImgurUwp.ApiClient.DisplayParams.Sort.Time, page: Page);
            List<GalleryItemViewModel> result = new List<GalleryItemViewModel>(tag.Items.Count);
            foreach (var i in tag.Items)
            {
                result.Add(new GalleryItemViewModel(i));
            }
            Page++;
            return result;
        }
    }
}
