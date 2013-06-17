using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Runtime.Serialization.Formatters.Binary;
using Abot.Crawler;
using Abot.Poco;
using AbotCrawler.Policies;
using HtmlAgilityPack;

namespace AbotCrawler
{
    public static class Crawler
    {
        //Dictionary that contains all the crawled webpages
        public static Dictionary<String, CrawledWebPage> _crawledPages;

        //Crawl Configuration Object
        private static CrawlConfiguration _crawlConfig;

        public static void CrawlerInit()
        {
            _crawledPages = new Dictionary<string, CrawledWebPage>();
            _crawlConfig = new CrawlConfiguration
                            {
                                CrawlTimeoutSeconds = 100,
                                MaxConcurrentThreads = 10,
                                MaxPagesToCrawl = 1000,
                                UserAgentString = "abot v1.0 http://code.google.com/p/abot",
                                DownloadableContentTypes = "text/html, text/plain",
                                IsUriRecrawlingEnabled = false,
                                IsExternalPageCrawlingEnabled = true,
                                IsExternalPageLinksCrawlingEnabled = true,
                                HttpServicePointConnectionLimit = 200,
                                HttpRequestTimeoutInSeconds = 15,
                                HttpRequestMaxAutoRedirects = 7,
                                IsHttpRequestAutoRedirectsEnabled = true,
                                IsHttpRequestAutomaticDecompressionEnabled = true,
                                MinAvailableMemoryRequiredInMb = 0,
                                MaxMemoryUsageInMb = 200,
                                MaxMemoryUsageCacheTimeInSeconds = 2,
                                MaxCrawlDepth = 10,
                                IsRespectRobotsDotTextEnabled = true
                            };
        }

        /// <summary>
        /// Asynchronous event that is fired before a page is crawled.
        /// </summary>
        public static void ProcessPageCrawlStarting(object sender, PageCrawlStartingArgs e)
        {
            PageToCrawl pageToCrawl = e.PageToCrawl;
            String uri = e.PageToCrawl.Uri.AbsoluteUri;
            //Console.WriteLine("About to crawl link {0} which was found on page {1}", pageToCrawl.Uri.AbsoluteUri, pageToCrawl.ParentUri.AbsoluteUri);

            //Check if the page has been modified since the last crawl

            if (_crawledPages.ContainsKey(uri) && !ReVisit.ShouldReVisit(_crawledPages, uri))
            {
                PageCrawlDisallowed(pageToCrawl, null);
            }
        }


        /// <summary>
        /// Asynchronous event that is fired when an individual page has been crawled.
        /// </summary>
        public static void ProcessPageCrawlCompleted(object sender, PageCrawlCompletedArgs e)
        {
            if (e.CrawledPage.WebException != null || e.CrawledPage.HttpWebResponse.StatusCode != HttpStatusCode.OK)
                Console.WriteLine("Crawl of page failed: {0}\n", e.CrawledPage.Uri.AbsoluteUri);
            else if (string.IsNullOrEmpty(e.CrawledPage.RawContent))
            {
                Console.WriteLine("Page Crawled had no content: {0}\n", e.CrawledPage.Uri.AbsoluteUri);
            }
            else
            {
                String uri = e.CrawledPage.Uri.AbsoluteUri;
                //String date = new DateTime(e.CrawledPage.HttpWebResponse.Headers["Last-Modified"]);
                DateTime lastModified = e.CrawledPage.HttpWebResponse.LastModified;
                CrawledWebPage page = new CrawledWebPage(lastModified, e.CrawledPage.HtmlDocument.DocumentNode);
                _crawledPages.Add(uri, page);
                //Console.WriteLine("Crawled page saved: {0}", crawledPage.Uri.AbsoluteUri);
            }
        }


        /// <summary>
        /// Asynchronous event that is fired when the ICrawlDecisionMaker.ShouldCrawlLinks impl returned false. This means the page's links were not crawled.
        /// </summary>
        public static void PageLinksCrawlDisallowed(object sender, PageLinksCrawlDisallowedArgs e)
        {
            CrawledPage crawledPage = e.CrawledPage;
            Console.WriteLine("Did not crawl the links on page {0} due to {1}\n", crawledPage.Uri.AbsoluteUri, e.DisallowedReason);
        }


        /// <summary>
        /// Asynchronous event that is fired when the ICrawlDecisionMaker.ShouldCrawl impl returned false. This means the page or its links were not crawled.
        /// </summary>
        public static void PageCrawlDisallowed(object sender, PageCrawlDisallowedArgs e)
        {
            PageToCrawl pageToCrawl = e.PageToCrawl;
            //DateTime lastModified = oldPage.HttpWebResponse.LastModified;


            Console.WriteLine("Did not crawl page {0} due to {1}\n", pageToCrawl.Uri.AbsoluteUri, e.DisallowedReason);
        }

        public static Dictionary<String, CrawledWebPage> Run()
        {
            PoliteWebCrawler crawler = new PoliteWebCrawler(_crawlConfig, null, null, null, null, null, null, null, null);

            //Registers on the events
            crawler.PageCrawlStartingAsync += ProcessPageCrawlStarting;
            crawler.PageCrawlCompletedAsync += ProcessPageCrawlCompleted;
            crawler.PageCrawlDisallowedAsync += PageCrawlDisallowed;
            crawler.PageLinksCrawlDisallowedAsync += PageLinksCrawlDisallowed;

            UriBag uris = new UriBag();
            uris.Add("carris", "http://www.carris.pt");
            uris.Add("tfl", "http://www.tfl.gov.uk/");
            uris.Add("publico", "http://www.publico.pt");
            uris.Add("CP", "http://www.cp.pt");
            uris.Add("GVB", "http://en.gvb.nl/");

            crawler.Crawl(uris.Get("CP"));

            return _crawledPages;

            //Tries to save the webpages information
            //Utils.Save(CrawledPages);
        }

        public static Dictionary<string, HtmlNode> RunToLucene()
        {

            Dictionary<String, CrawledWebPage> crawled = Run();

            Dictionary<string, HtmlNode> result = new Dictionary<string, HtmlNode>();
            foreach (string key in crawled.Keys)
                result.Add(key, crawled[key].WebPage);
            return result;

        }

        
    }
}