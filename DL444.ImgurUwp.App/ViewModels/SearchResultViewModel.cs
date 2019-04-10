using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DL444.ImgurUwp.ApiClient;
using Windows.UI.Xaml.Controls;

namespace DL444.ImgurUwp.App.ViewModels
{
    class SearchResultViewModel : CachingViewModel, INotifyPropertyChanged, IListViewPersistent
    {
        private DisplayParams.Sort _sort;

        public IncrementalLoadingCollection<SearchResultIncrementalSource, GalleryItemViewModel> Items { get; private set; }
        public string Terms { get; }
        public DisplayParams.Sort Sort
        {
            get => _sort;
            set
            {
                if(_sort == value) { return; }
                _sort = value;
                SearchResultIncrementalSource source = new SearchResultIncrementalSource(Terms, value);
                Items = new IncrementalLoadingCollection<SearchResultIncrementalSource, GalleryItemViewModel>(source);
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Sort)));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Items)));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SortByPopularity)));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SortByTime)));
            }
        }

        public bool SortByPopularity => Sort == DisplayParams.Sort.Viral;
        public bool SortByTime => Sort == DisplayParams.Sort.Time;

        public SearchResultViewModel(string terms, DisplayParams.Sort sort = DisplayParams.Sort.Viral)
        {
            Terms = terms ?? throw new ArgumentNullException(nameof(terms));
            Sort = sort;
            SearchResultIncrementalSource source = new SearchResultIncrementalSource(Terms, sort);
            Items = new IncrementalLoadingCollection<SearchResultIncrementalSource, GalleryItemViewModel>(source);
            SortByPopularityCommand = new Command(() => Sort = DisplayParams.Sort.Viral);
            SortByTimeCommand = new Command(() => Sort = DisplayParams.Sort.Time);
        }

        public Command SortByPopularityCommand { get; }
        public Command SortByTimeCommand { get; }

        public event PropertyChangedEventHandler PropertyChanged;

        public string ScrollPosition { get; private set; }
        GalleryItemViewModel scrollViewAnchorItem;
        public void SetScrollPosition(ListViewBase listView)
        {
            ScrollPosition = ListViewPersistenceHelper.GetRelativeScrollPosition(listView,
                x =>
                {
                    scrollViewAnchorItem = x as GalleryItemViewModel;
                    return scrollViewAnchorItem.Id;
                });
        }
        public async Task RecoverScrollPosition(ListViewBase listView)
        {
            if (ScrollPosition == null) { return; }
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
    }

    class SearchResultIncrementalSource : IncrementalItemsSource<GalleryItemViewModel>
    {
        public string SearchTerms { get; }
        public DisplayParams.Sort Sort { get; }
        public int Page { get; private set; }
        protected override async Task<IEnumerable<GalleryItemViewModel>> GetItemsFromSourceAsync(CancellationToken cancellationToken)
        {
            if(SearchTerms == null) { return null; }

            var items = new List<GalleryItemViewModel>();
            var results = await ApiClient.Client.GallerySearchAsync(SearchTerms, Sort, page: Page);
            items.AddRange(from r in results select (new GalleryItemViewModel(r)));
            Page++;
            return items;
        }

        public SearchResultIncrementalSource() { }
        public SearchResultIncrementalSource(string terms, DisplayParams.Sort sort = DisplayParams.Sort.Viral)
        {
            SearchTerms = terms ?? throw new ArgumentNullException(nameof(terms));
            Sort = sort;
        }
    }
}
