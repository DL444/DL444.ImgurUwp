using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace DL444.ImgurUwp.Models
{
    public class AccountSettings
    {
        public string Bio { get; set; }

        [JsonProperty(PropertyName = "accepted_gallery_terms")]
        public bool GalleryTermsAccepted { get; set; }

        [JsonProperty(PropertyName = "account_url")]
        public string Username { get; set; }

        [JsonProperty(PropertyName = "active_emails")]
        public List<string> ActiveEmails { get; set; }

        [JsonProperty(PropertyName = "album_privacy")]
        public AlbumPrivacyOptions AlbumPrivacy { get; set; }

        public string Avatar { get; set; }

        // TODO: Figure out and implement blocked users property.

        [JsonProperty(PropertyName = "comment_replies")]
        public bool CommentReplyNotify { get; set; }

        public string Cover { get; set; }
        public string Email { get; set; }

        [JsonProperty(PropertyName = "first_party")]
        public bool FirstPartyLogin { get; set; }

        [JsonProperty(PropertyName = "messaging_enabled")]
        public bool MessagingEnabled { get; set; }

        [JsonProperty(PropertyName = "newsletter_subscribed")]
        public bool NewsletterSubscribed { get; set; }

        [JsonProperty(PropertyName = "public_images")]
        public bool PublicImagesByDefault { get; set; }

        [JsonProperty(PropertyName = "show_mature")]
        public bool ShowMature { get; set; }
    }

    public enum AlbumPrivacyOptions
    {
        Public, Hidden, Secret
    }
}
