using DL444.ImgurUwp.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace DL444.ImgurUwp.ApiClient
{
    public partial class ApiClient
    {
        public async Task<Image> GetImageAsync(string id)
        {
            if (id == null) { throw new ArgumentNullException(nameof(id)); }
            var response = await client.GetAsync($"/3/image/{id}");
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
        public async Task<bool> FavoriteImageAsync(string id)
        {
            if (id == null) { throw new ArgumentNullException(nameof(id)); }

            HttpRequestMessage msg = new HttpRequestMessage(HttpMethod.Post, $"/3/image/{id}/favorite");
            var response = await client.SendAsync(msg);

            (bool success, int status, string dataJson) = GetDataToken(await response.Content.ReadAsStringAsync());
            if (success)
            {
                return string.Equals(dataJson, "favorited", StringComparison.OrdinalIgnoreCase);
            }
            else
            {
                throw new ApiRequestException(dataJson) { Status = status };
            }
        }

        public async Task<Image> UploadImageAsync(Stream image, string album = null, string title = null, string description = null)
        {
            // TODO: Implement anonymous upload.

            if(image == null) { throw new ArgumentNullException(nameof(image)); }
            if(image.Length > 10 * 1024 * 1024)
            {
                throw new ArgumentException("The specified image is larger than 10MB.");
            }
            byte[] imageBytes;
            image.Position = 0;
            using (MemoryStream str = new MemoryStream(image.Length > int.MaxValue ? int.MaxValue : (int)image.Length))
            {
                await image.CopyToAsync(str);
                imageBytes = str.ToArray();
            }
            string imageBase64 = Convert.ToBase64String(imageBytes);
            return await UploadImageAsync(imageBase64, album, title, description);
        }
        private async Task<Image> UploadImageAsync(string imageBase64, string album = null, string title = null, string description = null)
        {
            if(imageBase64 == null) { throw new ArgumentNullException(nameof(imageBase64)); }
            // TODO: Find a efficient way to determine the size of image before making this public.
            
            ImageUploadParams img = new ImageUploadParams(imageBase64, album, null, title, description);

            HttpRequestMessage msg = new HttpRequestMessage(HttpMethod.Post, "/3/image");
            msg.Content = new StringContent(JsonConvert.SerializeObject(img));
            msg.Content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/json");
            var response = await client.SendAsync(msg);

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

        public async Task<bool> UpdateImageInfoAsync(string id, string title = null, string description = null)
        {
            if (id == null) { throw new ArgumentNullException(nameof(id)); }
            ImageUpdateParams update = new ImageUpdateParams(id, title, description);

            HttpRequestMessage msg = new HttpRequestMessage(HttpMethod.Post, $"/3/image/{id}");
            msg.Content = new StringContent(JsonConvert.SerializeObject(update));
            msg.Content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/json");
            var response = await client.SendAsync(msg);

            (bool success, int status, string dataJson) = GetDataToken(await response.Content.ReadAsStringAsync());
            if (success)
            {
                return JsonConvert.DeserializeObject<bool>(dataJson.ToLower());
            }
            else
            {
                throw new ApiRequestException(dataJson) { Status = status };
            }
        }
        public async Task<bool> DeleteImageAsync(string id)
        {
            if(id == null) { throw new ArgumentNullException(nameof(id)); }
            HttpRequestMessage msg = new HttpRequestMessage(HttpMethod.Delete, $"/3/image/{id}");
            var response = await client.SendAsync(msg);
            (bool success, int status, string dataJson) = GetDataToken(await response.Content.ReadAsStringAsync());
            if (success)
            {
                return JsonConvert.DeserializeObject<bool>(dataJson.ToLower());
            }
            else
            {
                throw new ApiRequestException(dataJson) { Status = status };
            }
        }
    }
}
