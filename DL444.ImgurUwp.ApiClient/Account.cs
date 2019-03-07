using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using DL444.ImgurUwp.Models;
using Newtonsoft.Json.Linq;

namespace DL444.ImgurUwp.ApiClient
{
    public partial class ApiClient
    {
        public async Task<Account> GetAccountAsync(string username)
        {
            if (username == null) { throw new ArgumentNullException(nameof(username)); }
            var response = await client.GetAsync($"/3/account/{username}");
            (bool success, int status, string dataJson) = GetDataToken(await response.Content.ReadAsStringAsync());
            if (success)
            {
                var account = JsonConvert.DeserializeObject<Account>(dataJson);
                JObject obj = JObject.Parse(dataJson);
                if(obj.ContainsKey("user_follow"))
                {
                    account.UserFollow = obj["user_follow"]["status"].ToObject<bool>();
                }
                return account;
            }
            else
            {
                throw new ApiRequestException(dataJson) { Status = status };
            }
        }

        public async Task<int> GetAccountAlbumCountAsync(string username)
        {
            if (username == null) { throw new ArgumentNullException(nameof(username)); }
            var response = await client.GetAsync($"/3/account/{username}/albums/count");
            (bool success, int status, string dataJson) = GetDataToken(await response.Content.ReadAsStringAsync());
            if (success)
            {
                return JsonConvert.DeserializeObject<int>(dataJson);
            }
            else
            {
                throw new ApiRequestException(dataJson) { Status = status };
            }
        }
        public async Task<IEnumerable<Album>> GetAccountAlbumsAsync(string username, int page = 0)
        {
            if (username == null) { throw new ArgumentNullException(nameof(username)); }
            var response = await client.GetAsync($"/3/account/{username}/albums/{page}");
            (bool success, int status, string dataJson) = GetDataToken(await response.Content.ReadAsStringAsync());
            if (success)
            {
                return JsonConvert.DeserializeObject<List<Album>>(dataJson);
            }
            else
            {
                throw new ApiRequestException(dataJson) { Status = status };
            }
        }
        public async Task<Album> GetAccountAlbumAsync(string username, string id)
        {
            if (username == null) { throw new ArgumentNullException(nameof(username)); }
            if (id == null) { throw new ArgumentNullException(nameof(id)); }
            var response = await client.GetAsync($"/3/account/{username}/album/{id}");
            (bool success, int status, string dataJson) = GetDataToken(await response.Content.ReadAsStringAsync());
            if (success)
            {
                return JsonConvert.DeserializeObject<Album>(dataJson);
            }
            else
            {
                throw new ApiRequestException(dataJson) { Status = status };
            }
        }
        public async Task<IEnumerable<string>> GetAccountAlbumIdsAsync(string username)
        {
            if (username == null) { throw new ArgumentNullException(nameof(username)); }
            var response = await client.GetAsync($"/3/account/{username}/albums/ids");
            (bool success, int status, string dataJson) = GetDataToken(await response.Content.ReadAsStringAsync());
            if (success)
            {
                return JsonConvert.DeserializeObject<List<string>>(dataJson);
            }
            else
            {
                throw new ApiRequestException(dataJson) { Status = status };
            }
        }

        public async Task<int> GetAccountCommentCountAsync(string username)
        {
            if (username == null) { throw new ArgumentNullException(nameof(username)); }
            var response = await client.GetAsync($"/3/account/{username}/comments/count");
            (bool success, int status, string dataJson) = GetDataToken(await response.Content.ReadAsStringAsync());
            if (success)
            {
                return JsonConvert.DeserializeObject<int>(dataJson);
            }
            else
            {
                throw new ApiRequestException(dataJson) { Status = status };
            }
        }
        public async Task<IEnumerable<Comment>> GetAccountCommentsAsync(string username)
        {
            if (username == null) { throw new ArgumentNullException(nameof(username)); }
            var response = await client.GetAsync($"/3/account/{username}/comments");
            (bool success, int status, string dataJson) = GetDataToken(await response.Content.ReadAsStringAsync());
            if (success)
            {
                return JsonConvert.DeserializeObject<List<Comment>>(dataJson);
            }
            else
            {
                throw new ApiRequestException(dataJson) { Status = status };
            }
        }
        public async Task<IEnumerable<int>> GetAccountCommentIdsAsync(string username)
        {
            if (username == null) { throw new ArgumentNullException(nameof(username)); }
            var response = await client.GetAsync($"/3/account/{username}/comments/ids");
            (bool success, int status, string dataJson) = GetDataToken(await response.Content.ReadAsStringAsync());
            if (success)
            {
                return JsonConvert.DeserializeObject<List<int>>(dataJson);
            }
            else
            {
                throw new ApiRequestException(dataJson) { Status = status };
            }
        }

        public async Task<int> GetAccountImageCountAsync(string username)
        {
            if (username == null) { throw new ArgumentNullException(nameof(username)); }
            var response = await client.GetAsync($"/3/account/{username}/images/count");
            (bool success, int status, string dataJson) = GetDataToken(await response.Content.ReadAsStringAsync());
            if (success)
            {
                return JsonConvert.DeserializeObject<int>(dataJson);
            }
            else
            {
                throw new ApiRequestException(dataJson) { Status = status };
            }
        }
        public async Task<IEnumerable<Image>> GetAccountImagesAsync(string username, int page = 0)
        {
            if (username == null) { throw new ArgumentNullException(nameof(username)); }
            var response = await client.GetAsync($"/3/account/{username}/images/{page}");
            (bool success, int status, string dataJson) = GetDataToken(await response.Content.ReadAsStringAsync());
            if (success)
            {
                return JsonConvert.DeserializeObject<List<Image>>(dataJson);
            }
            else
            {
                throw new ApiRequestException(dataJson) { Status = status };
            }
        }
        public async Task<Image> GetAccountImageAsync(string username, string id)
        {
            if (username == null) { throw new ArgumentNullException(nameof(username)); }
            if (id == null) { throw new ArgumentNullException(nameof(id)); }
            var response = await client.GetAsync($"/3/account/{username}/image/{id}");
            (bool success, int status, string dataJson) = GetDataToken(await response.Content.ReadAsStringAsync());
            if (success)
            {
                return JsonConvert.DeserializeObject<Image>(dataJson);
            }
            else
            {
                throw new ApiRequestException(dataJson) { Status = status };
            }
        }
        public async Task<IEnumerable<string>> GetAccountImageIdsAsync(string username)
        {
            if (username == null) { throw new ArgumentNullException(nameof(username)); }
            var response = await client.GetAsync($"/3/account/{username}/images/ids");
            (bool success, int status, string dataJson) = GetDataToken(await response.Content.ReadAsStringAsync());
            if (success)
            {
                return JsonConvert.DeserializeObject<List<string>>(dataJson);
            }
            else
            {
                throw new ApiRequestException(dataJson) { Status = status };
            }
        }

        public async Task<IEnumerable<IGalleryItem>> GetAccountLikesAsync(string username)
        {
            if (username == null) { throw new ArgumentNullException(nameof(username)); }
            List<IGalleryItem> result = new List<IGalleryItem>();
            var response = await client.GetAsync($"/3/account/{username}/likes");
            (bool success, int status, string dataJson) = GetDataToken(await response.Content.ReadAsStringAsync());
            if (success)
            {
                var items = JArray.Parse(dataJson);
                foreach (var i in items)
                {
                    if (i["is_album"].ToObject<bool>())
                    {
                        result.Add(i.ToObject<GalleryAlbum>());
                    }
                    else
                    {
                        result.Add(i.ToObject<GalleryImage>());
                    }
                }
                return result;
            }
            else
            {
                throw new ApiRequestException(dataJson) { Status = status };
            }
        }

        public async Task<IEnumerable<IItem>> GetAccountFavoritesAsync(string username)
        {
            if (username == null) { throw new ArgumentNullException(nameof(username)); }
            List<IItem> result = new List<IItem>();
            var response = await client.GetAsync($"/3/account/{username}/favorites");
            (bool success, int status, string dataJson) = GetDataToken(await response.Content.ReadAsStringAsync());
            if (success)
            {
                var items = JArray.Parse(dataJson);
                foreach (var i in items)
                {
                    if (i["is_album"].ToObject<bool>())
                    {
                        result.Add(i.ToObject<Album>());
                    }
                    else
                    {
                        result.Add(i.ToObject<Image>());
                    }
                }
                return result;
            }
            else
            {
                throw new ApiRequestException(dataJson) { Status = status };
            }
        }
        public async Task<IEnumerable<IGalleryItem>> GetAccountGalleryFavoritesAsync(string username)
        {
            if (username == null) { throw new ArgumentNullException(nameof(username)); }
            List<IGalleryItem> result = new List<IGalleryItem>();
            var response = await client.GetAsync($"/3/account/{username}/gallery_favorites");
            (bool success, int status, string dataJson) = GetDataToken(await response.Content.ReadAsStringAsync());
            if (success)
            {
                var items = JArray.Parse(dataJson);
                foreach (var i in items)
                {
                    if (i["is_album"].ToObject<bool>())
                    {
                        result.Add(i.ToObject<GalleryAlbum>());
                    }
                    else
                    {
                        result.Add(i.ToObject<GalleryImage>());
                    }
                }
                return result;
            }
            else
            {
                throw new ApiRequestException(dataJson) { Status = status };
            }
        }

        public async Task<GalleryProfile> GetAccountGalleryProfileAsync(string username)
        {
            if (username == null) { throw new ArgumentNullException(nameof(username)); }
            var response = await client.GetAsync($"/3/account/{username}/gallery_profile");
            (bool success, int status, string dataJson) = GetDataToken(await response.Content.ReadAsStringAsync());
            if (success)
            {
                return JsonConvert.DeserializeObject<GalleryProfile>(dataJson);
            }
            else
            {
                throw new ApiRequestException(dataJson) { Status = status };
            }
        }

        public async Task<IEnumerable<IGalleryItem>> GetAccountSubmissionsAsync(string username, int page = 0)
        {
            if (username == null) { throw new ArgumentNullException(nameof(username)); }
            List<IGalleryItem> result = new List<IGalleryItem>();
            var response = await client.GetAsync($"/3/account/{username}/submissions/{page}");
            (bool success, int status, string dataJson) = GetDataToken(await response.Content.ReadAsStringAsync());
            if (success)
            {
                var items = JArray.Parse(dataJson);
                foreach (var i in items)
                {
                    if (i["is_album"].ToObject<bool>())
                    {
                        result.Add(i.ToObject<GalleryAlbum>());
                    }
                    else
                    {
                        result.Add(i.ToObject<GalleryImage>());
                    }
                }
                return result;
            }
            else
            {
                throw new ApiRequestException(dataJson) { Status = status };
            }
        }
    }
}
