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

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace DL444.ImgurUwp.App.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class GeneralLoadingErrorView : Page
    {
        public GeneralLoadingErrorView()
        {
            this.InitializeComponent();
        }

        public Type OriginalPage { get; private set; }
        public object OriginalParameter { get; private set; }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            OriginalPage = e.SourcePageType;
            OriginalParameter = e.Parameter;
        }

        private void Retry_Click(object sender, RoutedEventArgs e)
        {
            (Window.Current.Content as Frame).Navigate(OriginalPage, OriginalParameter);
        }
    }
}
