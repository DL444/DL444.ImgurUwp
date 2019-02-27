using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

namespace DL444.ImgurUwp.App.Settings
{
    class SettingsPageModel
    {
        public string Title { get; set; }
        public string Description { get; set; }

        public Type ContentPageType { get; set; }
    }

    class HeaderCaptionModel
    {
        public string Header { get; set; }
        public string Caption { get; set; }
    }
}
