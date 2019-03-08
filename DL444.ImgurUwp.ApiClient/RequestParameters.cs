using System;
using System.Collections.Generic;
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
    public enum AlbumPrivacy
    {
        Public, Hidden, Secret
    }
    public enum AlbumEditMode
    {
        Add, Replace, Remove
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

    class ImageUploadParams
    {
        public ImageUploadParams(string image, string album, string name, string title, string description)
        {
            Image = image ?? throw new ArgumentNullException(nameof(image));
            Album = album;
            Name = name;
            Title = title;
            Description = description;
        }

        [JsonProperty(PropertyName = "image")]
        public string Image { get; set; }

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore, PropertyName = "album")]
        public string Album { get; set; }
        [JsonProperty(PropertyName = "type")]
        public string Type { get; set; } = "base64";

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore, PropertyName = "name")]
        public string Name { get; set; }
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore, PropertyName = "title")]
        public string Title { get; set; }
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore, PropertyName = "description")]
        public string Description { get; set; }
    }

    class ImageUpdateParams
    {
        public ImageUpdateParams(string id, string title, string description)
        {
            Id = id ?? throw new ArgumentNullException(nameof(id));
            Title = title;
            Description = description;
        }

        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }
        [JsonProperty(PropertyName = "title", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string Title { get; set; }
        [JsonProperty(PropertyName = "description", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string Description { get; set; }
    }

    class AlbumCreateParams
    {
        public AlbumCreateParams(string title, string description, AlbumPrivacy? privacy, 
            IEnumerable<string> imageIds, IEnumerable<string> deleteHashes, string coverId)
        {
            Title = title;
            Description = description;
            Privacy = privacy;
            ImageIds = imageIds;
            DeleteHashes = deleteHashes;
            CoverId = coverId;
        }

        [JsonProperty(PropertyName = "title", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string Title { get; set; }
        [JsonProperty(PropertyName = "description", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string Description { get; set; }
        [JsonProperty(PropertyName = "privacy", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public AlbumPrivacy? Privacy { get; set; }
        [JsonProperty(PropertyName = "ids", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public IEnumerable<string> ImageIds { get; set; }
        [JsonProperty(PropertyName = "deletehashes", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public IEnumerable<string> DeleteHashes { get; set; }
        [JsonProperty(PropertyName = "cover", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string CoverId { get; set; }
    }

    class AccountSettingsParams
    {
        public AccountSettingsParams(bool? publicImagesByDefault, bool? messagingEnabled, AlbumPrivacy? albumDefaultPrivacy, 
            string username, bool? showMature, bool? newsletterSubscribe)
        {
            PublicImagesByDefault = publicImagesByDefault;
            MessagingEnabled = messagingEnabled;
            AlbumDefaultPrivacy = albumDefaultPrivacy;
            Username = username;
            ShowMature = showMature;
            NewsletterSubscribe = newsletterSubscribe;
        }

        public AccountSettingsParams(Models.AccountSettings settings)
            : this(settings.PublicImagesByDefault, settings.MessagingEnabled, null, settings.Username, settings.ShowMature, settings.NewsletterSubscribed)
        {
            switch(settings.AlbumPrivacy)
            {
                case Models.AlbumPrivacyOptions.Public:
                    AlbumDefaultPrivacy = AlbumPrivacy.Public;
                    break;
                case Models.AlbumPrivacyOptions.Hidden:
                    AlbumDefaultPrivacy = AlbumPrivacy.Hidden;
                    break;
                case Models.AlbumPrivacyOptions.Secret:
                    AlbumDefaultPrivacy = AlbumPrivacy.Secret;
                    break;
            }
        }

        [JsonProperty(PropertyName = "public_images", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public bool? PublicImagesByDefault { get; set; }
        [JsonProperty(PropertyName = "messaging_enabled", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public bool? MessagingEnabled { get; set; }
        [JsonProperty(PropertyName = "album_privacy", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public AlbumPrivacy? AlbumDefaultPrivacy { get; set; }
        [JsonProperty(PropertyName = "username", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string Username { get; set; }
        [JsonProperty(PropertyName = "show_mature", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public bool? ShowMature { get; set; }
        [JsonProperty(PropertyName = "newsletter_subscribed", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public bool? NewsletterSubscribe { get; set; }
    }
}
