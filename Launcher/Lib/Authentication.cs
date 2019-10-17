using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Threading.Tasks;
using DeviceId;
using Launcher.Models;
using Newtonsoft.Json;
using static System.Text.Encoding;

namespace Launcher.Lib
{
    internal class LoginSettings
    {
        public string GoogleToken = "104372892978312113975";
        public string RefreshToken;
    }

    internal class TokensClass
    {
        public string access_token;
        public string expires_in;
        public string refresh_token;
        public string token_type;
    }

    public static class Authentication
    {
        private static readonly byte[] _iv =
            ASCII.GetBytes(new DeviceIdBuilder().AddMotherboardSerialNumber().ToString());

        private static readonly byte[] _key =
            ASCII.GetBytes(new DeviceIdBuilder().AddProcessorId().AddSystemUUID().ToString());

        private static readonly string dataPath =
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);

        private static string configPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        private static readonly string _loginSettingsFileLocation = dataPath + "/fubr/login.setings";

        private static void OpenBrowser(string url)
        {
            Process browser;
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                browser = Process.Start(new ProcessStartInfo("cmd",
                    $"/c start {url.Replace("&", "^&")}")); // Works ok on windows and escape need for cmd.exe
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                browser = Process.Start("xdg-open", url); // Works ok on linux
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                browser = Process.Start("open", url); // Not tested
        }

        public static async Task<User> Login()
        {
            LoginSettings loginSettings;
            //check if user has logged in before so the browser doesnt have to open
            if (HasUserLoggedInBefore())
            {
                // open encryted file that contains the refresh token
                var encryptedFileStream = File.Open(_loginSettingsFileLocation, FileMode.Open);

                var decryptedLoginSettings = DecryptData(StreamToByteArray(encryptedFileStream));
                encryptedFileStream.Close();
                loginSettings = JsonConvert.DeserializeObject<LoginSettings>(decryptedLoginSettings);
            }
            else
            {
                // open brower to authenticate
                loginSettings = new LoginSettings();
                OpenBrowser(@"https://lumen.arankieskamp.com");
            }


            // store access token in APIAccessor singelton
            var tokens = await GetTokens(loginSettings);
            var accessor = ApiAccessor.GetApiAccessor;
            accessor.AccessToken = tokens.access_token;

            // update refresh token with new token and store safely back in encrypted file
            loginSettings.RefreshToken = tokens.refresh_token;
            var loginSettingsJson = JsonConvert.SerializeObject(loginSettings);
            var encryptedLoginSettings = EncryptData(loginSettingsJson);
            var loginSettingsStream = File.Open(_loginSettingsFileLocation, FileMode.OpenOrCreate);
            if (loginSettingsStream.CanWrite) loginSettingsStream.Write(encryptedLoginSettings);
            loginSettingsStream.Close();

            return await ApiAccessor.GetCurrentUser();
        }

        private static async Task<TokensClass> GetAccessTokenWithRefreshToken(string refreshToken)
        {
            TokensClass tokens;
            using (var httpClient = new HttpClient())
            {
                var url = @"https://lumen.arankieskamp.com";
                httpClient.BaseAddress = new Uri(url);
                httpClient.DefaultRequestHeaders.Accept.Clear();

                var content = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("client_id", "4"),
                    new KeyValuePair<string, string>("client_secret", "9k5rZHjCAbN6QIXHYW9zaodQ2sqtxGItZAqpp2YO"),
                    new KeyValuePair<string, string>("refresh_token", refreshToken),
                    new KeyValuePair<string, string>("grant_type", "refresh_token"),
                    new KeyValuePair<string, string>("scope", "*")
                });
                var result = await httpClient.PostAsync("/oauth/token", content);
                var resultJson = await result.Content.ReadAsStringAsync();
                tokens = JsonConvert.DeserializeObject<TokensClass>(resultJson);
            }

            return tokens;
        }

        private static async Task<TokensClass> GetAccessTokenWithGoogletoken(string googleToken)
        {
            TokensClass tokens;
            using (var httpClient = new HttpClient())
            {
                var url = @"https://lumen.arankieskamp.com";
                httpClient.BaseAddress = new Uri(url);
                httpClient.DefaultRequestHeaders.Accept.Clear();
                var content = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("client_id", "4"),
                    new KeyValuePair<string, string>("client_secret", "9k5rZHjCAbN6QIXHYW9zaodQ2sqtxGItZAqpp2YO"),
                    new KeyValuePair<string, string>("username", googleToken),
                    new KeyValuePair<string, string>("password", "-"),
                    new KeyValuePair<string, string>("grant_type", "password"),
                    new KeyValuePair<string, string>("scope", "*")
                });
                var result = await httpClient.PostAsync("/oauth/token", content);
                var resultJson = await result.Content.ReadAsStringAsync();
                tokens = JsonConvert.DeserializeObject<TokensClass>(resultJson);
            }

            return tokens;
        }

        private static async Task<TokensClass> GetTokens(LoginSettings loginSettings)
        {
            if (string.IsNullOrEmpty(loginSettings.RefreshToken))
                return await GetAccessTokenWithGoogletoken(loginSettings.GoogleToken);
            return await GetAccessTokenWithRefreshToken(loginSettings.RefreshToken);
        }

        private static byte[] StreamToByteArray(Stream input)
        {
            var buffer = new byte[16 * 1024];
            using (var ms = new MemoryStream())
            {
                int read;
                while ((read = input.Read(buffer, 0, buffer.Length)) > 0) ms.Write(buffer, 0, read);

                return ms.ToArray();
            }
        }

        private static bool HasUserLoggedInBefore()
        {
            return File.Exists(_loginSettingsFileLocation);
        }

        private static byte[] EncryptData(string plainText)
        {
            // Check arguments.
            if (plainText == null || plainText.Length <= 0)
                throw new ArgumentNullException("plainText");
            byte[] encrypted;

            // Create an Aes object
            // with the specified key and IV.
            using (var aesAlg = Aes.Create())
            {
                aesAlg.Key = _key;
                aesAlg.IV = _iv;

                // Create an encryptor to perform the stream transform.
                var encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                // Create the streams used for encryption.
                using (var msEncrypt = new MemoryStream())
                {
                    using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (var swEncrypt = new StreamWriter(csEncrypt))
                        {
                            //Write all data to the stream.
                            swEncrypt.Write(plainText);
                        }

                        encrypted = msEncrypt.ToArray();
                    }
                }
            }


            // Return the encrypted bytes from the memory stream.
            return encrypted;
        }

        private static string DecryptData(byte[] cipherText)
        {
            // Check arguments.
            if (cipherText == null || cipherText.Length <= 0)
                throw new ArgumentNullException("cipherText");

            // Declare the string used to hold
            // the decrypted text.
            string plaintext = null;

            // Create an Aes object
            // with the specified key and IV.
            using (var aesAlg = Aes.Create())
            {
                aesAlg.Key = _key;
                aesAlg.IV = _iv;

                // Create a decryptor to perform the stream transform.
                var decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                // Create the streams used for decryption.
                using (var msDecrypt = new MemoryStream(cipherText))
                {
                    using (var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (var srDecrypt = new StreamReader(csDecrypt))
                        {
                            // Read the decrypted bytes from the decrypting stream
                            // and place them in a string.
                            plaintext = srDecrypt.ReadToEnd();
                        }
                    }
                }
            }

            return plaintext;
        }
    }
}