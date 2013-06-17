using System;
using System.Collections.Generic;
using HtmlAgilityPack;

namespace AbotCrawler
{
    [Serializable()]
    public class CrawledWebPage
    {
        public DateTime LastModified { get; set; }
        public HtmlNode WebPage { get; set; }

        public CrawledWebPage(DateTime lastMod, HtmlNode webp)
        {
            LastModified = lastMod;
            WebPage = webp;
        }
    }
}
