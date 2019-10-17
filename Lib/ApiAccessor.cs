using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Launcher.Models;

namespace Launcher.Lib
{
    public class ApiAccessor
    {
        private string _accessToken;

        public string AccessToken
        {
            get => _accessToken;
            set => _accessToken = value;
        }

        private static ApiAccessor _apiAccessor;

        public static ApiAccessor GetApiAccessor => _apiAccessor ?? (_apiAccessor = new ApiAccessor());

        private ApiAccessor()
        {
        }
        
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
                const string url = @"https://lumen.arankieskamp.com/api";
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
    }
}