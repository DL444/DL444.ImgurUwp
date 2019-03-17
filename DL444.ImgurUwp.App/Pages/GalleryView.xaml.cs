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
using DL444.ImgurUwp.App.ViewModels;
using System.ComponentModel;
using DL444.ImgurUwp.ApiClient;
using System.Threading;
using System.Threading.Tasks;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace DL444.ImgurUwp.App.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class GalleryView : Page, INotifyPropertyChanged
    {
        //private GalleryCollectionViewModel _viewModel;
        private IncrementalLoadingCollection<GalleryIncrementalSource, GalleryItemViewModel> _viewModelInc;
        bool init;

        //GalleryCollectionViewModel ViewModel
        //{
        //    get => _viewModel;
        //    set
        //    {
        //        _viewModel = value;
        //        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ViewModel)));
        //    }
        //}


        IncrementalLoadingCollection<GalleryIncrementalSource, GalleryItemViewModel> ViewModelInc
        {
            get => _viewModelInc;
            set
            {
                _viewModelInc = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ViewModelInc)));
            }
        }

        private DisplayParams.Section _section;
        public DisplayParams.Section Section
        {
            get => _section;
            set
            {
                if(value != _section)
                {
                    _section = value;
                    if(init)
                    {
                        var source = new GalleryIncrementalSource(value, Sort);
                        ViewModelInc = new IncrementalLoadingCollection<GalleryIncrementalSource, GalleryItemViewModel>(source);
                    }
                }
            }
        }
        private DisplayParams.Sort _sort;
        public DisplayParams.Sort Sort
        {
            get => _sort;
            set
            {
                if(value != _sort)
                {
                    _sort = value;
                    if(init)
                    {
                        var source = new GalleryIncrementalSource(Section, value);
                        ViewModelInc = new IncrementalLoadingCollection<GalleryIncrementalSource, GalleryItemViewModel>(source);
                    }
                }
            }
        }

        public GalleryView()
        {
            this.InitializeComponent();
            // Universal Windows Platform does not allow custom RoutedEvent. So have to use code-behind.
            //FrontpageLayout.ItemClicked += FrontpageLayout_ItemClicked;
            //this.NavigationCacheMode = NavigationCacheMode.Enabled;
        }

        //private void FrontpageLayout_ItemClicked(object sender, RoutedEventArgs e)
        //{
        //    GalleryItemViewModel item = (sender as Button).Tag as GalleryItemViewModel;
        //    Navigation.Navigate(typeof(GalleryItemDetails), (item, ViewModel));
        //}

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (e.Parameter is DisplayParams.Section sect && !init /*&& ViewModel == null*/)
            {
                //var galleryItems = await ApiClient.Client.GetGalleryItemsAsync(DisplayParams.Sort.Viral, 0, sect);
                //ViewModel = new GalleryCollectionViewModel(galleryItems);
                Section = sect;
                var source = new GalleryIncrementalSource(Section, Sort);
                ViewModelInc = new IncrementalLoadingCollection<GalleryIncrementalSource, GalleryItemViewModel>(source);
                init = true;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void FrontPageGrid_ItemClick(object sender, ItemClickEventArgs e)
        {
            var item = e.ClickedItem as GalleryItemViewModel;
            GalleryIncrementalSource source = new GalleryIncrementalSource(Section, Sort, ViewModelInc, ViewModelInc.Source.Page);
            Navigation.Navigate(typeof(GalleryItemDetails), new GalleryItemDetailsNavigationParameter(item, source));
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
