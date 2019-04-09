using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DL444.ImgurUwp.App.ViewModels
{
    class AccountAlbumPickerViewModel : INotifyPropertyChanged
    {
        private bool _selected;
        private ItemViewModel _selectedItem;

        public IncrementalLoadingCollection<MyItemAlbumsIncrementalSource, ItemViewModel> Albums { get; }
            = new IncrementalLoadingCollection<MyItemAlbumsIncrementalSource, ItemViewModel>();

        public ItemViewModel SelectedItem
        {
            get => _selectedItem;
            set
            {
                _selectedItem = value;
                Selected = _selectedItem != null;
            }
        }
        public bool Selected
        {
            get => _selected;
            set
            {
                _selected = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Selected)));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }

    class MyItemAlbumsIncrementalSource : IncrementalItemsSource<ItemViewModel>
    {
        public int Page { get; private set; }
        protected override async Task<IEnumerable<ItemViewModel>> GetItemsFromSourceAsync(CancellationToken cancellationToken)
        {
            var items = await ApiClient.Client.GetAccountItemsAsync("me", page: Page);
            Page++;
            return from i in items where (i.IsAlbum == true && !i.InGallery) select (new ItemViewModel(i));
        }
    }
}
