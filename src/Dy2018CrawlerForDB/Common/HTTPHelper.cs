using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace Dy2018CrawlerWithDB
{
    public class HTTPHelper
    {
        public static List<string> lstProxyIP = new List<string>() { };

        public static HttpClient Client { get; } = new HttpClient();

        public static string GetHTMLByURL(string url)
        {
            try
            {
                System.Net.WebRequest wRequest = System.Net.WebRequest.Create(url);
                wRequest.Proxy = WebRequest.DefaultWebProxy;
                wRequest.ContentType = "text/html; charset=gb2312";

                wRequest.Method = "get";
                wRequest.UseDefaultCredentials = true;
                // Get the response instance.
                var task = wRequest.GetResponseAsync();
                System.Net.WebResponse wResp = task.Result;
                System.IO.Stream respStream = wResp.GetResponseStream();
                using (System.IO.StreamReader reader = new System.IO.StreamReader(respStream, Encoding.GetEncoding("GB2312")))
                {
                    return reader.ReadToEnd();
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error("GetHTMLByURL Exception", ex,new { Url=url});
                return string.Empty;
            }
        }


      
    }


}