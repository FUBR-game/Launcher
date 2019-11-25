using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Launcher.Models;

namespace Launcher.Lib
{
    public class ApiAccessor
    {
        private static ApiAccessor _apiAccessor;

        private ApiAccessor()
        {
        }

        public string AccessToken { get; set; }

        public static ApiAccessor GetApiAccessor => _apiAccessor ?? (_apiAccessor = new ApiAccessor());

        public static async Task<User> GetCurrentUser()
        {
            User user;
            var resultJson = await CallApi("user");
            user = User.UserFromJsonString(resultJson);
            return user;
        }

        private static async Task<string> CallApi(string path)
        {
            using (var httpClient = new HttpClient())
            {
                const string url = @"https://lumen.arankieskamp.com/api/";
                httpClient.BaseAddress = new Uri(url);
                httpClient.DefaultRequestHeaders.Accept.Clear();
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                httpClient.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", GetApiAccessor.AccessToken);

                var result = await httpClient.GetAsync(url + path);
                var resultJson = await result.Content.ReadAsStringAsync();
                return resultJson;
            }
        }

        public async Task<List<User>> GetFriends()
        {
            var friendsListJson = await CallApi("users/friends");
            var friends = User.UsersFromJsonString(friendsListJson);

            return friends;
        }
    }
}