﻿using DL444.ImgurUwp.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net.Http;
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
    }
}
