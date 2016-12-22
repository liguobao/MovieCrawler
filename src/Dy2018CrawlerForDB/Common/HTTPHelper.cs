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
       
        /// <summary>
        /// 实现WebProxy接口
        /// </summary>
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

        /// <summary>
        /// 代理IP池子
        /// </summary>
        private static AvailableProxy availableProxy = AvailableProxyHepler.GetAvailableProxy();

        public static HttpClient Client { get; } = new HttpClient();

        /// <summary>
        /// 通过HTTP获取HTML（默认使用代理）
        /// </summary>
        /// <param name="url"></param>
        /// <param name="isUseProxy"></param>
        /// <returns></returns>
        public static string GetHTMLByURL(string url,bool isUseProxy=false)
        {
            ProxyInfo proxyInfo = null;
            try
            {
                System.Net.WebRequest wRequest = System.Net.WebRequest.Create(url);
                CrawlerProxyInfo crawlerProxyInfo = null;
                //测试中发现使用代理会跳转到中转页，解决方案暂时不明确，先屏蔽代理
                if (url.Contains(SoureceDomainConsts.BTdytt520)&& isUseProxy)
                {
                    var index = new Random(DateTime.Now.Millisecond).Next(0, 20);
                    proxyInfo = availableProxy.btdytt520[index];
                    crawlerProxyInfo = new CrawlerProxyInfo($"http://{proxyInfo.ip}:{proxyInfo.port}");

                }
                else if(url.Contains(SoureceDomainConsts.Dy2018Domain)&& isUseProxy)
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
                string proxyIP = isUseProxy ? proxyInfo.ip : "No Use Proxy";
                LogHelper.Error("GetHTMLByURL Exception", ex,$"URL:{url},Proxy:{proxyIP}");
                return string.Empty;
            }
        }
      
        
    }

}