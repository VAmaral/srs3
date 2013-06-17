using System;
using System.Collections.Generic;
using System.Net;
using Abot.Poco;
using AbotCrawler.Exceptions;

namespace AbotCrawler.Policies
{
    class ReVisit
    {
        public static Boolean ShouldReVisit(Dictionary<String, CrawledWebPage> webPages, String uri)
        {
            CrawledWebPage p = webPages[uri];

            if (p == null)
                throw new PageNotFoundException();
            else
            {
                HttpWebRequest req = (HttpWebRequest) WebRequest.Create(uri);
                HttpWebResponse res = (HttpWebResponse) req.GetResponse();
                DateTime lastModified = res.LastModified;

                return (p.LastModified.CompareTo(lastModified) <= 0);
            }
        }
    }
}
