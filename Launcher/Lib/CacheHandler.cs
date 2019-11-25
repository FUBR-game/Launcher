using System;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using Avalonia.Media.Imaging;
using Launcher.Models;

namespace Launcher.Lib
{
    public class CacheHandler
    {
        private static readonly string CacheLocation = AppDomain.CurrentDomain.BaseDirectory + "cache\\";
        private static readonly string ImageCacheLocation = CacheLocation + "Images\\";
        private static readonly string UserIconLocation = ImageCacheLocation + "UserIcons\\";
        private static Random random = new Random();

        public CacheHandler()
        {
            Directory.CreateDirectory(UserIconLocation);
        }

        public Bitmap GetUserIcon(User user)
        {
            var emailHash = CreateMD5(user.Email);
            var gravatarLink = "https://www.gravatar.com/avatar/" + emailHash + "?s=50";

            return ImageStringToBitMap(gravatarLink, UserIconLocation);
        }

        private static string CreateMD5(string input)
        {
            // Use input string to calculate MD5 hash
            using (var md5 = MD5.Create())
            {
                var inputBytes = Encoding.ASCII.GetBytes(input);
                var hashBytes = md5.ComputeHash(inputBytes);

                // Convert the byte array to hexadecimal string
                var sb = new StringBuilder();
                foreach (var t in hashBytes)
                {
                    sb.Append(t.ToString("X2"));
                }

                return sb.ToString().ToLower();
            }
        }

        public Bitmap ImageStringToBitMap(string imageUrl)
        {
            return ImageStringToBitMap(imageUrl, null);
        }

        private Bitmap ImageStringToBitMap(string imageUrl, string location)
        {
            if (location == null)
            {
                location = ImageCacheLocation;
            }

            var fileName = CreateMD5(imageUrl);
            var filePath = location + fileName + ".jpg";
            var request = WebRequest.Create(new Uri(imageUrl, UriKind.Absolute));
            var response = request.GetResponse();
            var stream = response.GetResponseStream();
            stream.Flush();

            var fileTest = File.Create(filePath);
            stream.CopyTo(fileTest);

            fileTest.Close();
            stream.Close();
            response.Close();

            var image = new Bitmap(filePath);

            return image;
        }
    }
}