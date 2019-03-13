using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Media.Imaging;

namespace DL444.ImgurUwp.App.ViewModels
{
    static class RichTextParser
    {
        public static IEnumerable<RichTextComponent> ParseComment(string comment)
        {
            if(comment == null) { return null; }
            List<RichTextComponent> result = new List<RichTextComponent>();
            StringBuilder strBuilder = new StringBuilder();
            string[] words = comment.SplitWithDelimiter("\t\n ".ToCharArray()).ToArray();
            foreach (var w in words)
            {
                if (IsUri(w))
                {
                    if (strBuilder.Length != 0)
                    {
                        result.Add(new TextComponent(strBuilder.ToString()));
                        strBuilder.Clear();
                    }
                    result.Add(new UriComponent(w));
                    result.Add(new TextComponent(" "));
                }
                else
                {
                    strBuilder.Append($"{w}");
                }
            }
            result.Add(new TextComponent(strBuilder.ToString()));
            return result;
        }
        public static FrameworkElement GetRichContentBox(IEnumerable<RichTextComponent> components)
        {
            List<Inline> inlines = new List<Inline>();
            List<UriComponent> images = null;

            foreach (var c in components)
            {
                if (c is TextComponent t && !string.IsNullOrWhiteSpace(t.Text))
                {
                    inlines.Add(new Run() { Text = t.Text });
                }
                else if (c is UriComponent u)
                {
                    if (u.Type == UriType.Normal)
                    {
                        var link = new Hyperlink() { NavigateUri = new Uri(u.Text) };
                        link.Inlines.Add(new Run() { Text = $"{u.Text} " });
                        inlines.Add(link);
                    }
                    else
                    {
                        if (images == null) { images = new List<UriComponent>(); }
                        images.Add(u);
                    }
                }
            }

            if (images == null)
            {
                TextBlock text = new TextBlock();
                foreach (var i in inlines)
                {
                    text.Inlines.Add(i);
                }
                text.TextWrapping = TextWrapping.Wrap;
                return text;
            }
            else
            {
                InlineUIContainer container = new InlineUIContainer();
                var scrollViewer = new ScrollViewer();
                scrollViewer.VerticalScrollMode = ScrollMode.Disabled;
                scrollViewer.VerticalScrollBarVisibility = ScrollBarVisibility.Disabled;
                scrollViewer.HorizontalScrollMode = ScrollMode.Auto;

                var stackPanel = new StackPanel();
                stackPanel.Margin = new Thickness(0, 4, 0, 0);
                stackPanel.Spacing = 4;
                foreach (var i in images)
                {
                    if (i.Type == UriType.Image)
                    {
                        Windows.UI.Xaml.Controls.Image img = new Windows.UI.Xaml.Controls.Image();
                        img.Stretch = Windows.UI.Xaml.Media.Stretch.Uniform;
                        img.Height = 150;
                        img.Source = new BitmapImage(new Uri(i.Text));
                        stackPanel.Children.Add(img);
                    }
                    else
                    {
                        MediaElement vid = new MediaElement();
                        vid.Height = 150;
                        vid.Stretch = Windows.UI.Xaml.Media.Stretch.Uniform;
                        vid.IsMuted = true;
                        vid.AreTransportControlsEnabled = false;
                        vid.Source = new Uri(i.Text);
                        stackPanel.Children.Add(vid);
                    }
                }
                scrollViewer.Content = stackPanel;
                container.Child = scrollViewer;

                RichTextBlock rtb = new RichTextBlock();
                if (inlines.Count > 0)
                {
                    Paragraph text = new Paragraph();
                    foreach (var i in inlines)
                    {
                        text.Inlines.Add(i);
                    }
                    rtb.Blocks.Add(text);
                }
                Paragraph imgs = new Paragraph();
                imgs.Inlines.Add(container);
                rtb.Blocks.Add(imgs);
                rtb.TextWrapping = TextWrapping.Wrap;
                rtb.IsTextSelectionEnabled = false;
                return rtb;
            }
        }
        public static FrameworkElement GetRichContentBox(string comment)
        {
            if(comment == null) { return null; }
            return GetRichContentBox(ParseComment(comment));
        }

        static IEnumerable<string> SplitWithDelimiter(this string str, params char[] delimiter)
        {
            if (delimiter.Contains('\0')) { throw new ArgumentException("Argument cannot contain '\0'."); }
            List<string> result = new List<string>();
            StringBuilder builder = new StringBuilder();
            for(int i = 0; i < str.Length; i++)
            {
                if(delimiter.FirstOrDefault(x => x == str[i]) == '\0')
                {
                    builder.Append(str[i]);
                }
                else
                {
                    result.Add(builder.ToString());
                    result.Add(str[i].ToString());
                    builder.Clear();
                }
            }
            result.Add(builder.ToString());
            result.RemoveAll(x => x == string.Empty);
            return result;
        }

        static bool IsUri(string str)
        {
            if (Uri.TryCreate(str, UriKind.Absolute, out System.Uri uri) == true)
            {
                if (uri.Scheme != "http" && uri.Scheme != "https") { return false; }
                return true;
            }
            else { return false; }
        }
    }

    abstract class RichTextComponent
    {
        public string Text { get; set; }
    }
    class TextComponent : RichTextComponent
    {
        public override string ToString() => Text;
        public TextComponent(string text) => Text = text;
    }
    class UriComponent : RichTextComponent
    {
        public override string ToString() => $"{Type}: {Text}";
        public UriType Type { get; set; }
        public UriComponent(string uri)
        {
            Text = uri;
            var ext = uri.Substring(uri.LastIndexOf('.') + 1).ToLower();
            switch (ext)
            {
                case "jpg":
                case "jpeg":
                case "png":
                case "gif":
                case "apng":
                case "tif":
                case "tiff":
                    Type = UriType.Image;
                    Text = $"{Text}?maxwidth=400&fidelity=mid";
                    break;
                // For some reason MediaElement acts weirdly. So use GIF instead. Maybe good for memory as well?
                case "mp4":
                case "gifv":
                    Type = UriType.Image;
                    Text = $"{Text.Remove(Text.LastIndexOf('.') + 1)}gif?maxwidth=400&fidelity=mid";
                    break;
                default:
                    Type = UriType.Normal;
                    break;
            }
        }
    }
    enum UriType
    {
        Normal, Image, Video
    }
    // There are also @user, #tag, and #imageId.
}
