using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DL444.ImgurUwp.App.ViewModels
{
    class NonGalleryItemDetailsPageViewModel
    {
        public ItemViewModel Item { get; set; }

        public ObservableCollection<ImageViewModel> Images { get; } = new ObservableCollection<ImageViewModel>();

        public static async Task<NonGalleryItemDetailsPageViewModel> CreateFromItem(ItemViewModel vm)
        {
            var result = new NonGalleryItemDetailsPageViewModel();
            result.Item = vm;
            if(vm.IsAlbum)
            {
                var album = vm.Item as Models.Album;
                album.Images.ForEach(x => result.Images.Add(new ImageViewModel(x)));
                if(album.Images.Count < album.ImageCount)
                {
                    var completeAlbum = await ApiClient.Client.GetAlbumAsync(album.Id);
                    for(int i = album.Images.Count; i < album.ImageCount; i++)
                    {
                        result.Images.Add(new ImageViewModel(completeAlbum.Images[i]));
                    }
                }
            }
            else
            {
                result.Images.Add(new ImageViewModel(vm.DisplayImage));
            }
            return result;
        }
    }
}
