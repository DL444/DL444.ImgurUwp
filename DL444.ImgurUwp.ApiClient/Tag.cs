using DL444.ImgurUwp.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using static DL444.ImgurUwp.ApiClient.DisplayParams;

namespace DL444.ImgurUwp.ApiClient
{
    public partial class ApiClient
    {
        public async Task<Tag> GetTagAsync(string name, Sort sort = Sort.Viral, Window window = Window.Week, int page = 0)
        {
            if (name == null) { throw new ArgumentNullException(nameof(name)); }
            var response = await client.GetAsync($"/3/gallery/t/{name}/{$"{sort}/{window}/{page}".ToLower()}");
            (bool success, int status, string dataJson) = GetDataToken(await response.Content.ReadAsStringAsync());
            if (success)
            {
                return ParseTagJson(dataJson);
            }
            else
            {
                throw new ApiRequestException(dataJson) { Status = status };
            }
        }
        public async Task<IEnumerable<Tag>> GetGalleryItemTagsAsync(string id)
        {
            if (id == null) { throw new ArgumentNullException(nameof(id)); }
            var response = await client.GetAsync($"/3/gallery/{id}/tags");
            (bool success, int status, string dataJson) = GetDataToken(await response.Content.ReadAsStringAsync());
            if (success)
            {
                JArray tags = JArray.Parse(dataJson);
                List<Tag> result = new List<Tag>();
                foreach(var t in tags)
                {
                    result.Add(ParseTagJson(t.ToString()));
                }
                return result;
            }
            else
            {
                throw new ApiRequestException(dataJson) { Status = status };
            }
        }
        //public async Task<IEnumerable<Tag>> GetDefaultTagsAsync()
        //{
        //    var response = await client.GetAsync("/3/tags");
        //    (bool success, int status, string dataJson) = GetDataToken(await response.Content.ReadAsStringAsync());
        //    if (success)
        //    {
        //        JObject obj = JObject.Parse(dataJson);
        //        return JsonConvert.DeserializeObject<List<Tag>>(obj["tags"].ToString());
        //    }
        //    else
        //    {
        //        throw new ApiRequestException(dataJson) { Status = status };
        //    }
        //}

        public async Task<bool> FollowTagAsync(string name)
        {
            if(name == null) { throw new ArgumentNullException(nameof(name)); }

            HttpRequestMessage msg = new HttpRequestMessage(HttpMethod.Post, $"/3/account/me/follow/tag/{name}");
            var response = await client.SendAsync(msg);
            (bool success, int status, string dataJson) = GetDataToken(await response.Content.ReadAsStringAsync());
            if (success)
            {
                JObject obj = JObject.Parse(dataJson.ToLower());
                return obj["status"].ToObject<bool>();
            }
            else
            {
                throw new ApiRequestException(dataJson) { Status = status };
            }
        }
        public async Task<bool> UnfollowTagAsync(string name)
        {
            if (name == null) { throw new ArgumentNullException(nameof(name)); }

            HttpRequestMessage msg = new HttpRequestMessage(HttpMethod.Delete, $"/3/account/me/follow/tag/{name}");
            var response = await client.SendAsync(msg);
            (bool success, int status, string dataJson) = GetDataToken(await response.Content.ReadAsStringAsync());
            if (success)
            {
                JObject obj = JObject.Parse(dataJson.ToLower());
                return obj["status"].ToObject<bool>();
            }
            else
            {
                throw new ApiRequestException(dataJson) { Status = status };
            }
        }

        static Tag ParseTagJson(string dataJson)
        {
            JObject obj = JObject.Parse(dataJson);
            Tag result = new Tag();
            result.Followers = obj["followers"].ToObject<int>();
            result.Following = obj["following"].ToObject<bool>();
            result.Name = obj["name"].ToObject<string>();
            result.TotalItems = obj["total_items"].ToObject<int>();
            result.DisplayName = obj["display_name"].ToObject<string>();
            result.Accent = obj["accent"].ToObject<string>();
            result.BackgroundImageHash = obj["background_hash"].ToObject<string>();
            result.Description = obj["description"].ToObject<string>();
            result.Items = new List<IGalleryItem>();
            JArray items = JArray.Parse(obj["items"].ToString());
            foreach (var i in items)
            {
                if (i["is_album"].ToObject<bool>())
                {
                    result.Items.Add(i.ToObject<GalleryAlbum>());
                }
                else
                {
                    result.Items.Add(i.ToObject<GalleryImage>());
                }
            }
            return result;
        }
    }
}
