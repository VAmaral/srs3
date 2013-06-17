using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.Helpers;
using HtmlAgilityPack;
using LuceneController;

namespace SocialNetwork.Models
{
    public static class ControlModule
    {

        public static Thread ad_infinitum = new Thread(()=>
            {
                LuceneUsage.TreatMultiUrl(AbotCrawler.Crawler.RunToLucene());
                
            
            
            
            
            
            });

        public static void PeriodicMaintenance() {

            
            RepositoryLocator.GetRepository().SearchLuceneDatabase();
            Dictionary<string, LinkedList<string>> emails_newUrls = RepositoryLocator.GetRepository().updateUsers();

            foreach (string email in emails_newUrls.Keys) {
                SendEmail(email, emails_newUrls[email]);
            }
                 
        }






        public static void SendEmail(string email, LinkedList<string> urls) {

            string body= "";

            foreach(string url in urls){
            body=body + "<a href=" + url + ">" + url +"</a>"+"<br>";
            }

            WebMail.SmtpServer = "smtp.gmail.com";
            WebMail.SmtpPort = 587;
            WebMail.EnableSsl = true;
            WebMail.UserName = "socialmobilityrecommender@gmail.com";
            WebMail.Password = "travelalong";
            WebMail.From = "socialmobilityrecommender@gmail.com";
            WebMail.Send(email, " Social Mobility Recommender: New urls for you to explore",body);
        
        
        }



    }
}