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
        public static TextBlock GetRichContentBox(IEnumerable<RichTextComponent> components)
        {
            List<Inline> inlines = new List<Inline>();

            foreach (var c in components)
            {
                if (c is TextComponent t && !string.IsNullOrWhiteSpace(t.Text))
                {
                    inlines.Add(new Run() { Text = t.Text });
                }
                else if (c is UriComponent u && u.Type == UriType.Normal)
                {
                    var link = new Hyperlink() { NavigateUri = new Uri(u.Text) };
                    //link.Inlines.Add(new Run() { Text = $"{u.Text} " });
                    link.Inlines.Add(new Run() { Text = $"{u.Text}" });
                    inlines.Add(link);
                }
            }

            if(inlines.Count == 0) { return null; }

            TextBlock text = new TextBlock();
            foreach (var i in inlines)
            {
                text.Inlines.Add(i);
            }
            text.TextWrapping = TextWrapping.Wrap;
            return text;
        }
        public static TextBlock GetRichContentBox(string comment)
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
            // TODO: ImageThumbnail
            Text = uri;
            var ext = uri.Substring(uri.LastIndexOf('.') + 1).ToLower();
            switch (ext)
            {
                case "jpg":
                case "jpeg":
                case "png":
                case "apng":
                case "tif":
                case "tiff":
                    Type = UriType.Image;
                    Text = $"{Text.Remove(Text.LastIndexOf('.'))}r.{ext}";
                    break;
                case "mp4":
                case "gifv":
                case "gif":
                    Type = UriType.Video;
                    Text = $"{Text.Remove(Text.LastIndexOf('.'))}_lq.mp4";
                    break;
                //case "mp4":
                //case "gifv":
                //    break;
                //case "gif":
                //    Type = UriType.Image;
                //    Text = $"{Text.Remove(Text.LastIndexOf('.') + 1)}gif";
                //    break;
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
