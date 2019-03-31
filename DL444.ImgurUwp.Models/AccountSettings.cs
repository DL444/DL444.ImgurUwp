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

    /// <summary>
    /// Represents the privacy setting of an album.
    /// </summary>
    public enum AlbumPrivacyOptions
    {
        /// <summary>
        /// The album is available for all. 
        /// </summary>
        /// <remarks>
        /// Posting to gallery automatically makes an album public. 
        /// However, marking an album as public does not automatically post it into the gallery.
        /// Posting to gallery has to be done explicitly.
        /// An album marked as public but has not yet been posted to gallery is the same as Hidden.
        /// </remarks>
        Public,
        /// <summary>
        /// The album is only available for its author and people who have link.
        /// </summary>
        Hidden,
        /// <summary>
        /// The album is only available for its author.
        /// </summary>
        /// <remarks>
        /// Imgur is trying to remove this option.
        /// It's unclear if it will be deprecated in the future.
        /// </remarks>
        Secret
    }
}
