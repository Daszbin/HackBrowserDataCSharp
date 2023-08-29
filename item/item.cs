using System.Collections.Generic;

namespace YourNamespace
{
    public enum Item
    {
        ChromiumKey,
        ChromiumPassword,
        ChromiumCookie,
        ChromiumBookmark,
        ChromiumHistory,
        ChromiumDownload,
        ChromiumCreditCard,
        ChromiumLocalStorage,
        ChromiumSessionStorage,
        ChromiumExtension,
        YandexPassword,
        YandexCreditCard,
        FirefoxKey4,
        FirefoxPassword,
        FirefoxCookie,
        FirefoxBookmark,
        FirefoxHistory,
        FirefoxDownload,
        FirefoxCreditCard,
        FirefoxLocalStorage,
        FirefoxSessionStorage,
        FirefoxExtension
    }

    public static class ItemExtensions
    {
        public static string FileName(this Item item)
        {
            return item switch
            {
                Item.ChromiumKey => "Local State",
                Item.ChromiumPassword => "Login Data",
                Item.ChromiumCookie => "Cookies",
                Item.ChromiumBookmark => "Bookmarks",
                Item.ChromiumDownload => "History",
                Item.ChromiumLocalStorage => "Local Storage/leveldb",
                Item.ChromiumSessionStorage => "Session Storage",
                Item.ChromiumCreditCard => "Web Data",
                Item.ChromiumExtension => "Extensions",
                Item.ChromiumHistory => "History",
                Item.YandexPassword => "Ya Passman Data",
                Item.YandexCreditCard => "Ya Credit Cards",
                Item.FirefoxKey4 => "key4.db",
                Item.FirefoxPassword => "logins.json",
                Item.FirefoxCookie => "cookies.sqlite",
                Item.FirefoxBookmark => "places.sqlite",
                Item.FirefoxDownload => "places.sqlite",
                Item.FirefoxLocalStorage => "webappsstore.sqlite",
                Item.FirefoxHistory => "places.sqlite",
                Item.FirefoxExtension => "extensions.json",
                Item.FirefoxCreditCard => "unsupported item",
                _ => "unknown item",
            };
        }

        public static string TempFileName(this Item item)
        {
            return item switch
            {
                Item.ChromiumKey => "chromiumKey",
                Item.ChromiumPassword => "password",
                Item.ChromiumCookie => "cookie",
                Item.ChromiumBookmark => "bookmark",
                Item.ChromiumDownload => "download",
                Item.ChromiumLocalStorage => "localStorage",
                Item.ChromiumSessionStorage => "sessionStorage",
                Item.ChromiumCreditCard => "creditCard",
                Item.ChromiumExtension => "extension",
                Item.ChromiumHistory => "history",
                Item.YandexPassword => "yandexPassword",
                Item.YandexCreditCard => "yandexCreditCard",
                Item.FirefoxKey4 => "firefoxKey4",
                Item.FirefoxPassword => "firefoxPassword",
                Item.FirefoxCookie => "firefoxCookie",
                Item.FirefoxBookmark => "firefoxBookmark",
                Item.FirefoxDownload => "firefoxDownload",
                Item.FirefoxHistory => "firefoxHistory",
                Item.FirefoxLocalStorage => "firefoxLocalStorage",
                Item.FirefoxSessionStorage => "firefoxSessionStorage",
                Item.FirefoxCreditCard => "unsupported item",
                Item.FirefoxExtension => "firefoxExtension",
                _ => "unknown item",
            };
        }

        public static bool IsSensitive(this Item item)
        {
            return item switch
            {
                Item.ChromiumKey or Item.ChromiumCookie or Item.ChromiumPassword or Item.ChromiumCreditCard or Item.FirefoxKey4 or Item.FirefoxPassword or Item.FirefoxCookie or Item.FirefoxCreditCard or Item.YandexPassword or Item.YandexCreditCard => true,
                _ => false,
            };
        }

        public static List<Item> FilterSensitiveItems(this List<Item> items)
        {
            List<Item> filtered = new();
            foreach (Item item in items)
            {
                if (item.IsSensitive())
                {
                    filtered.Add(item);
                }
            }
            return filtered;
        }

        public static List<Item> DefaultFirefoxItems()
        {
            return new List<Item>
            {
                Item.FirefoxKey4,
                Item.FirefoxPassword,
                Item.FirefoxCookie,
                Item.FirefoxBookmark,
                Item.FirefoxHistory,
                Item.FirefoxDownload,
                Item.FirefoxCreditCard,
                Item.FirefoxLocalStorage,
                Item.FirefoxSessionStorage,
                Item.FirefoxExtension
            };
        }

        public static List<Item> DefaultYandexItems()
        {
            return new List<Item>
            {
                Item.ChromiumKey,
                Item.ChromiumCookie,
                Item.ChromiumBookmark,
                Item.ChromiumHistory,
                Item.ChromiumDownload,
                Item.ChromiumExtension,
                Item.YandexPassword,
                Item.ChromiumLocalStorage,
                Item.ChromiumSessionStorage,
                Item.YandexCreditCard
            };
        }

        public static List<Item> DefaultChromiumItems()
        {
            return new List<Item>
            {
                Item.ChromiumKey,
                Item.ChromiumPassword,
                Item.ChromiumCookie,
                Item.ChromiumBookmark,
                Item.ChromiumHistory,
                Item.ChromiumDownload,
                Item.ChromiumCreditCard,
                Item.ChromiumLocalStorage,
                Item.ChromiumSessionStorage,
                Item.ChromiumExtension
            };
        }
    }
}
