﻿using DL444.ImgurUwp.ApiClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DL444.ImgurUwp.App.ViewModels
{
    class GalleryViewPageViewModel : IManagedViewModel, INotifyPropertyChanged
    {
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
                }
            }
        }

        public GalleryViewPageViewModel() : this(DisplayParams.Section.Hot, DisplayParams.Sort.Viral) { }
        public GalleryViewPageViewModel(DisplayParams.Section sect, DisplayParams.Sort sort)
        {
            _section = sect;
            _sort = sort;
            var source = new GalleryIncrementalSource(Section, Sort);
            Items = new IncrementalLoadingCollection<GalleryIncrementalSource, GalleryItemViewModel>(source);
        }

        public event PropertyChangedEventHandler PropertyChanged;
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