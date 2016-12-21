using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using Dy2018Crawler.Helper;
using System.Linq;

namespace Dy2018CrawlerWithDB
{
    public class HTTPHelper
    {
       

        public class CrawlerProxyInfo : IWebProxy
        {
            public CrawlerProxyInfo(string proxyUri)
                : this(new Uri(proxyUri))
            {
            }

            public CrawlerProxyInfo(Uri proxyUri)
            {
                this.ProxyUri = proxyUri;
            }

            public Uri ProxyUri { get; set; }

            public ICredentials Credentials { get; set; }

            public Uri GetProxy(Uri destination)
            {
                return this.ProxyUri;
            }

            public bool IsBypassed(Uri host)
            {
                return false; /* Proxy all requests */
            }
        }

        private static AvailableProxy availableProxy = AvailableProxyHepler.GetAvailableProxy();

        public static HttpClient Client { get; } = new HttpClient();

        public static string GetHTMLByURL(string url)
        {
            ProxyInfo proxyInfo = null;
            try
            {
                System.Net.WebRequest wRequest = System.Net.WebRequest.Create(url);
                CrawlerProxyInfo crawlerProxyInfo = null;
                if (url.Contains(SoureceDomainConsts.BTdytt520))
                {
                    var index = new Random(DateTime.Now.Millisecond).Next(0, 20);
                    proxyInfo = availableProxy.btdytt520[index];
                    crawlerProxyInfo = new CrawlerProxyInfo($"http://{proxyInfo.ip}:{proxyInfo.port}");

                }
                else if(url.Contains(SoureceDomainConsts.Dy2018Domain))
                {
                    var index = new Random(DateTime.Now.Millisecond).Next(0, 20);
                    proxyInfo = availableProxy.dy2018[index];
                    crawlerProxyInfo = new CrawlerProxyInfo($"http://{proxyInfo.ip}:{proxyInfo.port}");
                }
              
                wRequest.Proxy = crawlerProxyInfo;
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