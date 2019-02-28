using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DL444.ImgurUwp.Models;
using Microsoft.Toolkit.Collections;

namespace DL444.ImgurUwp.App.ViewModels
{
    public class GalleryCollectionViewModel : INotifyPropertyChanged
    {
        private ObservableCollection<GalleryItemViewModel> _galleryItems;

        public ObservableCollection<GalleryItemViewModel> GalleryItems
        {
            get => _galleryItems;
            set
            {
                _galleryItems = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(GalleryItems)));
            }
        }

        public GalleryCollectionViewModel() : this(new List<GalleryItemViewModel>()) { }
        public GalleryCollectionViewModel(IEnumerable<GalleryItemViewModel> items)
        {
            GalleryItems = new ObservableCollection<GalleryItemViewModel>(items);
        }
        public GalleryCollectionViewModel(IEnumerable<IGalleryItem> items)
        {
            List<GalleryItemViewModel> vms = new List<GalleryItemViewModel>();
            foreach (var i in items)
            {
                vms.Add(new GalleryItemViewModel(i));
            }
            GalleryItems = new ObservableCollection<GalleryItemViewModel>(vms);
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }

    public class GalleryItemsSource : IIncrementalSource<GalleryItemViewModel>
    {
        List<GalleryItemViewModel> items = new List<GalleryItemViewModel>();
        int page = 0;

        public async Task<IEnumerable<GalleryItemViewModel>> GetPagedItemsAsync(int pageIndex, int pageSize, CancellationToken cancellationToken = default(CancellationToken))
        {
            int lowerBound = pageIndex * pageSize; // Including
            int higherBound = lowerBound + pageSize; // Excluding

            if(items.Count < higherBound)
            {
                // TODO: Specify parameters!
                var newItems = await ApiClient.Client.GetGalleryItemsAsync(page: this.page);
                foreach (var i in newItems)
                {
                    items.Add(new GalleryItemViewModel(i));
                }
                page++;
            }

            return items.Skip(lowerBound).Take(pageSize);
        }
    }
}
