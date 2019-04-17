using DL444.ImgurUwp.ApiClient;
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
    class GalleryViewPageViewModel : CachingViewModel, INotifyPropertyChanged, IListViewPersistent
    {
        readonly Func<MessageBus.FavoriteChangedMessage, bool> favoriteChangedHandler;
        readonly Func<MessageBus.GalleryItemVoteMessage, bool> itemVoteHandler;
        static readonly Action<Exception> loadFaultHandler = x => Navigation.ShowItemFetchError();

        private IncrementalLoadingCollection<GalleryIncrementalSource, GalleryItemViewModel> _items;
        public IncrementalLoadingCollection<GalleryIncrementalSource, GalleryItemViewModel> Items
        {
            get => _items;
            set
            {
                _items = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Items)));
            }
        }

        private DisplayParams.Section _section;
        public DisplayParams.Section Section
        {
            get => _section;
            set
            {
                if (value != _section)
                {
                    _section = value;
                    var source = new GalleryIncrementalSource(value, Sort);
                    Items = new IncrementalLoadingCollection<GalleryIncrementalSource, GalleryItemViewModel>(source);
                }
            }
        }
        private DisplayParams.Sort _sort;
        public DisplayParams.Sort Sort
        {
            get => _sort;
            set
            {
                if (value != _sort)
                {
                    _sort = value;
                    var source = new GalleryIncrementalSource(Section, value);
                    Items = new IncrementalLoadingCollection<GalleryIncrementalSource, GalleryItemViewModel>(source);
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SortByPopularity)));
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SortByTime)));
                }
            }
        }

        public bool SortByPopularity => Sort == DisplayParams.Sort.Viral;
        public bool SortByTime => Sort == DisplayParams.Sort.Time;

        public Command SortByPopularityCommand { get; }
        public Command SortByTimeCommand { get; }

        public GalleryViewPageViewModel() : this(DisplayParams.Section.Hot, DisplayParams.Sort.Viral) { }
        public GalleryViewPageViewModel(DisplayParams.Section sect, DisplayParams.Sort sort)
        {
            _section = sect;
            _sort = sort;
            var source = new GalleryIncrementalSource(Section, Sort);
            Items = new IncrementalLoadingCollection<GalleryIncrementalSource, GalleryItemViewModel>(source, onError: loadFaultHandler);
            SortByPopularityCommand = new Command(() => Sort = DisplayParams.Sort.Viral);
            SortByTimeCommand = new Command(() => Sort = DisplayParams.Sort.Time);
            favoriteChangedHandler = new Func<MessageBus.FavoriteChangedMessage, bool>(x =>
            {
                var item = Items.FirstOrDefault(i => i.Id == x.Id && i.IsAlbum == x.IsAlbum);
                if (item == null) { return false; }
                item.Favorite = true;
                return true;
            });
            MessageBus.ViewModelMessageBus.Instance.RegisterListener(new MessageBus.FavoriteChangedMessageListener(favoriteChangedHandler));
            itemVoteHandler = new Func<MessageBus.GalleryItemVoteMessage, bool>(x =>
            {
                var item = Items.FirstOrDefault(i => i.Id == x.Id);
                if (item == null) { return false; }
                item.Upvoted = x.Upvoted;
                item.Downvoted = x.Downvoted;
                return true;
            });
            MessageBus.ViewModelMessageBus.Instance.RegisterListener(new MessageBus.GalleryItemVoteMessageListener(itemVoteHandler));
        }

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

    }
    public class GalleryIncrementalSource : IncrementalItemsSource<GalleryItemViewModel>
    {
        public int Page { get; private set; }

        public DisplayParams.Section Section { get; private set; }
        public DisplayParams.Sort Sort { get; private set; }

        public GalleryIncrementalSource(DisplayParams.Section section, DisplayParams.Sort sort)
        {
            Section = section;
            Sort = sort;
        }
        public GalleryIncrementalSource(DisplayParams.Section section, DisplayParams.Sort sort, IEnumerable<GalleryItemViewModel> items, int startPage) : base(items)
        {
            Section = section;
            Sort = sort;
            Page = startPage;
        }

        protected override async Task<IEnumerable<GalleryItemViewModel>> GetItemsFromSourceAsync(CancellationToken cancellationToken)
        {
            var gallery = await ApiClient.Client.GetGalleryItemsAsync(Sort, Page, Section);
            List<GalleryItemViewModel> result = new List<GalleryItemViewModel>();
            foreach (var g in gallery)
            {
                result.Add(new GalleryItemViewModel(g));
            }
            Page++;
            return result;
        }
    }
}
