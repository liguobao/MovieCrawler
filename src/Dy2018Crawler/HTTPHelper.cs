using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace Dy2018Crawler
{
    public class HTTPHelper
    {

        public static HttpClient Client { get; } = new HttpClient();

        public static string GetHTMLByURL(string url)
        {
            try
            {
                System.Net.WebRequest wRequest = System.Net.WebRequest.Create(url);
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
                Console.WriteLine(ex.ToString());
                return string.Empty;
            }
        }


        public static string GetHTML(string url)
        {
            try
            {
                Client.DefaultRequestHeaders.Add("Accept-Encoding", "gzip, deflate");
                Client.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("Mozilla", "4.0"));
                Client.DefaultRequestHeaders.ExpectContinue = true;
                var task = Client.GetStringAsync(url);
                return task.Result; 
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return string.Empty;
            }
        }
       
    }


}