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

namespace DL444.ImgurUwp.App.Settings
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class GeneralSettingsPage : Page, INotifyPropertyChanged
    {
        public GeneralSettingsPage()
        {
            this.InitializeComponent();
            this.NavigationCacheMode = NavigationCacheMode.Enabled;
        }

        bool _isMehVoteOn;

        bool IsMehVoteOn
        {
            get => _isMehVoteOn;
            set
            {
                _isMehVoteOn = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsMehVoteOn)));
                AppSettingsManager.UpdateEntry("mehvote", value);
            }
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            IsMehVoteOn = AppSettingsManager.RetrieveEntry<bool>("mehvote");
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
