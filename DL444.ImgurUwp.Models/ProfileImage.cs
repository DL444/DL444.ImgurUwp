using System;
using System.Collections.Generic;
using System.Text;

namespace DL444.ImgurUwp.Models
{
    public class ProfileImage
    {
        public ProfileImageType Type { get; set; }
        public string Name { get; set; }
        public string Location { get; set; }
    }

    public enum ProfileImageType
    {
        Avatar, Cover
    }
}
