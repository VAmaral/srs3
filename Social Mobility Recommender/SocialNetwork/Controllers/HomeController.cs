using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SocialNetwork.Models;
using HtmlAgilityPack;
using System.Threading;



namespace SocialNetwork.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            

            return View();
        }

        public ActionResult SimulateTotal() {

            Thread background = new Thread(() => {
                Dictionary<string, HtmlNode> crawled = AbotCrawler.Crawler.RunToLucene();
                LuceneController.LuceneUsage.TreatMultiUrl(crawled);
                ControlModule.PeriodicMaintenance();
            });
            background.Start();           
            return RedirectToAction("Index");
        }


        public ActionResult Simulate() {

            
            ControlModule.PeriodicMaintenance();
            return RedirectToAction("Index");
            
        
        }

        public ActionResult About()
        {

            return RedirectToAction("Index");
            
        }

        public ActionResult Contact()
        {
            

            return RedirectToAction("Index");
        }

        public ActionResult Search() {


            return View();
        
        }
        [HttpPost]
        public ActionResult BeginSearch(string term) {

            string[] terms = term.Split(' ');
            IEnumerable<string> result= RepositoryLocator.GetRepository().FindTerms(terms);
            ViewData["list"]= result;
            return View();
        }

        public ActionResult Profile()
        {
           return View(RepositoryLocator.GetRepository().GetUser(0));
        }

        public ActionResult ChangeTerms(int id) {

            return View(RepositoryLocator.GetRepository().GetUser(id));
        }
        [HttpPost]
        public ActionResult ChangeTerms(int id, string terms) {
            LinkedList<string> newTermList = new LinkedList<string>();
            IUserRepository repo= RepositoryLocator.GetRepository();

            foreach (string s in terms.Split(' ')) newTermList.AddLast(s);
            repo.GetUser(id).ResetTastes(newTermList);
            return RedirectToAction("Profile");
        
        }
    }
}
