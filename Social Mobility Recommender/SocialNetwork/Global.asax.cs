using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using HtmlAgilityPack;
using SocialNetwork.Models;

namespace SocialNetwork
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            string path1 = Server.MapPath("~/./");
            LuceneController.LuceneUsage.basePath = path1.Substring(0, path1.Length - 14);
            RepositoryLocator.GetRepository().AddUser("Utilizador", "srsuser@hotmail.com", false);//password:iamaUser
            LinkedList<string> terms= new LinkedList<string>();
            terms.AddLast("museu");
            RepositoryLocator.GetRepository().AddMultiTerms(0, terms);
            AbotCrawler.Crawler.CrawlerInit();
           

            AreaRegistration.RegisterAllAreas();
           
           
            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            AuthConfig.RegisterAuth();
        }
    }
}