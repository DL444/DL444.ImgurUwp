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
            ImageViewTemplateSelector.ImageTemplate = this.Resources["AccountImageTemplate"] as DataTemplate;
            ImageViewTemplateSelector.VideoTemplate = this.Resources["AccountVideoTemplate"] as DataTemplate;
        }

        private ItemViewModel _selectedItem;

        IncrementalLoadingCollection<MyImageIncrementalSource, ItemViewModel> Images { get; set; } = null;
        ItemViewModel SelectedItem
        {
            get => _selectedItem;
            set
            {
                if (object.ReferenceEquals(value, _selectedItem)) { return; }
                _selectedItem = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedItem)));
            }
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            if (e.Parameter is ValueTuple<IncrementalLoadingCollection<MyImageIncrementalSource, ItemViewModel>, ItemViewModel> navParam)
            {
                Images = navParam.Item1;
                SelectedItem = Images.FirstOrDefault(x => x.Id == navParam.Item2.Id);
            }
        }

        private async void Delete_Click(object sender, RoutedEventArgs e)
        {
            var index = Images.IndexOf(SelectedItem);
            await SelectedItem.DeleteCommand.ExecuteAsync(null);
            if(SelectedItem == null)
            {
                if (Images.Count == 0)
                {
                    Navigation.ContentFrame.GoBack();
                }
                else
                {
                    SelectedItem = Images[index];
                }
            }
        }

        private async void ImageScrollViewer_DoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
        {
            if(e.PointerDeviceType != Windows.Devices.Input.PointerDeviceType.Mouse)
            {
                // Give user enough time to lift finger or pen.
                await System.Threading.Tasks.Task.Delay(100);
            }
            var scrollViewer = sender as ScrollViewer;
            var offset = e.GetPosition(scrollViewer);
            var factor = scrollViewer.ZoomFactor;
            if(Math.Abs(factor - 1.0) > 1e-2)
            {
                scrollViewer.ChangeView(offset.X, offset.Y, 1.0f);
                return;
            }
            scrollViewer.ChangeView(offset.X, offset.Y, 2.0f);
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }

    public class ImageViewTemplateSelector : DataTemplateSelector
    {
        public static DataTemplate ImageTemplate { get; set; }
        public static DataTemplate VideoTemplate { get; set; }

        protected override DataTemplate SelectTemplateCore(object item, DependencyObject container)
        {
            if(item is ItemViewModel i)
            {
                if(i.DisplayImage.Animated)
                {
                    return VideoTemplate;
                }
                else
                {
                    return ImageTemplate;
                }
            }
            return base.SelectTemplateCore(item, container);
        }
    }
}
