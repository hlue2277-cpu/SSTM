using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataService.Helpers
{
    public static class ImageMappingHelper
    {
        private static IDictionary<string, string> bannerImageUrlDictionary = new Dictionary<string, string>() { { "pack://siteoforigin:,,,/Banner/header.png", "pack://siteoforigin:,,,/Banner/header.png" } };
        private static IDictionary<string, DateTime> bannerImageModifiedDictionary = new Dictionary<string, DateTime>();

        public static IDictionary<string, DateTime> BannerImageModifiedDictionary
        {
            get { return bannerImageModifiedDictionary; }
            set { bannerImageModifiedDictionary = value; }
        }

        public static IDictionary<string, string> BannerImageUrlDictionary
        {
            get { return bannerImageUrlDictionary; }
            set { bannerImageUrlDictionary = value; }
        }

        public static string GetBannerImageUrl(string bannerIdentity)
        {
            string bannerUrl = string.Empty;
            bannerImageUrlDictionary.TryGetValue(bannerIdentity, out bannerUrl);

            return bannerUrl;
        }

        public static string GetBannerName(string picPath)
        {
            string bannerFile = picPath.ToLowerInvariant();
            if (bannerFile.StartsWith("http") || bannerFile.StartsWith("https") || bannerFile.StartsWith("ftp"))
            {
                return bannerFile.Substring(bannerFile.LastIndexOf("/") + 1);
            }
            else
            {
                return picPath;
            }
        }
    }
}
