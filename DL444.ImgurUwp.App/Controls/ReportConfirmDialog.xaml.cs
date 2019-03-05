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

// The Content Dialog item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace DL444.ImgurUwp.App.Controls
{
    public sealed partial class ReportConfirmDialog : ContentDialog
    {
        public ReportConfirmDialog()
        {
            this.InitializeComponent();
            ReportTemplateSelector.GalleryItemTemplate = this.Resources["GalleryItemTemplate"] as DataTemplate;
            ReportTemplateSelector.CommentTemplate = this.Resources["CommentTemplate"] as DataTemplate;
        }
        public ReportConfirmDialog(ViewModels.IReportable item) : this()
        {
            ViewModel = item;
            // Somehow ContentTemplateSelector does not work as expected. So workaround.
            ItemPreview.ContentTemplate = new ReportTemplateSelector().SelectTemplate(ViewModel);
        }

        ViewModels.IReportable ViewModel { get; }

        private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            if (Reason1.IsChecked == true) { SelectedReason = ImgurUwp.ApiClient.ReportReason.DoesNotBelong; }
            else if (Reason2.IsChecked == true) { SelectedReason = ImgurUwp.ApiClient.ReportReason.Spam; }
            else if (Reason3.IsChecked == true) { SelectedReason = ImgurUwp.ApiClient.ReportReason.Abusive; }
            else if (Reason4.IsChecked == true) { SelectedReason = ImgurUwp.ApiClient.ReportReason.UnmarkedMature; }
            else if (Reason5.IsChecked == true) { SelectedReason = ImgurUwp.ApiClient.ReportReason.Porn; }
            else { SelectedReason = ImgurUwp.ApiClient.ReportReason.Unspecified; }
        }

        public ImgurUwp.ApiClient.ReportReason SelectedReason { get; private set; }
    }

    public class ReportTemplateSelector : DataTemplateSelector
    {
        public static DataTemplate GalleryItemTemplate { get; set; }
        public static DataTemplate CommentTemplate { get; set; }

        protected override DataTemplate SelectTemplateCore(object item)
        {
            if(item is ViewModels.GalleryItemViewModel) { return GalleryItemTemplate; }
            else if(item is ViewModels.CommentViewModel) { return CommentTemplate; }
            else { return base.SelectTemplateCore(item); }
        }
    }
}
