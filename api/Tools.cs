

using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MovieCrawler.API.Common
{
    public static class Tools
    {

        public static string ClearInvalidHTML(string htmlText)
        {
            if (string.IsNullOrEmpty(htmlText))
            {
                return htmlText;
            }
            if (htmlText.Contains("<!--") && htmlText.Contains("-->") && htmlText.Split("-->")[0].Length > 500)
            {
                htmlText = htmlText.Replace("<!--", "").Replace("-->", "");
            }
            return htmlText;
        }

        public static string AddEmptyLine(string htmlText)
        {
            if (string.IsNullOrEmpty(htmlText))
            {
                return htmlText;
            }
            var lineTexts = htmlText.Split("\n").Where(t => !string.IsNullOrEmpty(t)).ToList();
            StringBuilder sb = new StringBuilder();
            foreach (var text in lineTexts)
            {
                sb.Append(text);
                sb.Append("\n\n");
            }
            return sb.ToString();
        }

        public static string GetSha256(string text)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(text);
            var sha256 = SHA256.Create();
            byte[] hash = sha256.ComputeHash(bytes);
            StringBuilder hashString = new StringBuilder();
            foreach (byte x in hash)
            {
                hashString.Append(string.Format("{0:x2}", x));
            }
            return hashString.ToString();
        }


        public static string Base64Encode(string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }

        public static string Base64Decode(string content)
        {
            byte[] bytes = Convert.FromBase64String(content);
            return Encoding.UTF8.GetString(bytes);
        }

        public static string GetMD5(string observedText)
        {
            string result;
            using (MD5 hash = MD5.Create())
            {
                result = string.Join
                (
                    "",
                    from ba in hash.ComputeHash
                    (
                        Encoding.UTF8.GetBytes(observedText)
                    )
                    select ba.ToString("x2")
                );
            }
            return result;
        }

        public static long GetMillisecondTimestamp()
        {
            return (long)(DateTime.Now.ToLocalTime() - new DateTime(1970, 1, 1).ToLocalTime()).TotalMilliseconds;
        }


        public static string NewGuid()
        {
            return System.Guid.NewGuid().ToString();
        }

        public static long ToUnixTime(DateTime dt)
        {
            var timeSpan = (dt - new DateTime(1970, 1, 1, 0, 0, 0));
            return (long)timeSpan.TotalSeconds;
        }


        public static T AutoMapper<T>(Object obj)
        {
            return JsonConvert.DeserializeObject<T>(JsonConvert.SerializeObject(obj));
        }

        public static void CopyTo<T>(this Object source, T target) where T : class, new()
        {
            if (source == null)
                return;
            if (target == null)
            {
                target = new T();
            }
            var tgProperties = target.GetType().GetProperties();
            var sProperties = source.GetType().GetProperties();
            foreach (var sProperty in sProperties)
            {
                var value = sProperty?.GetValue(source);
                if (value != null)
                {
                    var tgProperty = tgProperties.FirstOrDefault(p => p.Name.ToUpper() == sProperty.Name.ToUpper());
                    if (tgProperty == null || tgProperty.SetMethod == null)
                    {
                        continue;
                    }
                    tgProperty.SetValue(target, value);
                }
            }
        }


        public static bool IsValidJson(this string stringValue)
        {
            if (string.IsNullOrWhiteSpace(stringValue))
            {
                return false;
            }

            var value = stringValue.Trim();

            if ((value.StartsWith("{") && value.EndsWith("}")) || //For object
                (value.StartsWith("[") && value.EndsWith("]"))) //For array
            {
                try
                {
                    var obj = JToken.Parse(value);
                    return true;
                }
                catch (JsonReaderException)
                {
                    return false;
                }
            }

            return false;
        }
    }

}