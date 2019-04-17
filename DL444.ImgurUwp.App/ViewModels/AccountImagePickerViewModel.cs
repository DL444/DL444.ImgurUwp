using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DL444.ImgurUwp.App.ViewModels
{
    class AccountImagePickerViewModel
    {
        static readonly Action<Exception> loadFaultHandler = x => Navigation.ShowItemFetchError();

        public IncrementalLoadingCollection<MyImageIncrementalSource, ItemViewModel> Images { get; } 
            = new IncrementalLoadingCollection<MyImageIncrementalSource, ItemViewModel>(onError: loadFaultHandler);
        public List<ItemViewModel> SelectedImages { get; } = new List<ItemViewModel>();

        public void UpdateSelection(IEnumerable<ItemViewModel> addedItems, IEnumerable<ItemViewModel> removedItems)
        {
            if(addedItems != null)
            {
                SelectedImages.AddRange(addedItems);
            }
            if(removedItems != null)
            {
                foreach(var i in removedItems)
                {
                    SelectedImages.Remove(i);
                }
            }
        }

        public AccountImagePickerViewModel() { }
    }
}
