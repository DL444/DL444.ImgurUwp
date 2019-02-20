using System;
using Newtonsoft.Json;

namespace DL444.ImgurUwp.ApiClient
{
    public static class DisplayParams
    {
        public enum Sort
        {
            Viral, Time
        }
        public enum Section
        {
            Hot, Top, User
        }
        public enum Window
        {
            Day, Week, Month, Year, All
        }
    }
    public static class CommentDisplayParams
    {
        public enum Sort
        {
            Best, Top, New
        }
    }
    public enum GalleryItemType
    {
        Image, Album
    }
    public enum Vote
    {
        Up, Down
    }
    public enum ReportReason
    {
        Unspecified = 0, DoesNotBelong = 1, Spam = 2, Abusive = 3, UnmarkedMature = 4, Porn = 5
    }

    class PostCommentParams
    {
        public PostCommentParams(string imageId, string content, string parentId = null)
        {
            ImageId = imageId ?? throw new ArgumentNullException(nameof(imageId));
            Content = content ?? throw new ArgumentNullException(nameof(content));
            ParentId = parentId;
        }

        [JsonProperty(PropertyName = "image_id")]
        public string ImageId { get; set; }
        [JsonProperty(PropertyName = "comment")]
        public string Content { get; set; }
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore, PropertyName = "parent_id")]
        public string ParentId { get; set; }
    }
}
