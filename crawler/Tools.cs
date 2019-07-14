

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MovieCrawler
{
    public static class Tools
    {

        public static string GetSimpleCanonicalName(string name)
        {
            var canonicalName = string.Join(" ",
                Regex.Matches(name.Trim(), "([a-zA-Z0-9]+)").Select(g => g.Value));
            // discourse category 最长不超过50个字符, 使用场景下这里生成 canonicalName 时可能会带上Discussion,
            // 格式为 Warm Wedding, CEO Loves Me - Discussion 所以这里限制到38个字符
            if (canonicalName.Length > 38)
            {
                return canonicalName.Substring(0, 38);
            }
            return canonicalName;
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