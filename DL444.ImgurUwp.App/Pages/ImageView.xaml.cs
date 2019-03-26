using DL444.ImgurUwp.App.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace DL444.ImgurUwp.App.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ImageView : Page, INotifyPropertyChanged
    {
        public ImageView()
        {
            this.InitializeComponent();
        }

        private ItemViewModel _selectedItem;

        IncrementalLoadingCollection<MyImageIncrementalSource, ItemViewModel> Images { get; set; } = null;
        public ItemViewModel SelectedItem
        {
            get => _selectedItem;
            set
            {
                if(object.ReferenceEquals(value, _selectedItem)) { return; }
                _selectedItem = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedItem)));
            }
        }


        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            if(e.Parameter is ValueTuple<IncrementalLoadingCollection<MyImageIncrementalSource, ItemViewModel>, ItemViewModel> navParam)
            {
                Images = navParam.Item1;
                SelectedItem = navParam.Item2;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
