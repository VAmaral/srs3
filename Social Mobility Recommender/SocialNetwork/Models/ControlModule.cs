using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Helpers;
using HtmlAgilityPack;
//using System.Web.Mail;
using LuceneController;

namespace SocialNetwork.Models
{
    public static class ControlModule
    {


        public static LinkedList<string> log = new LinkedList<string>();

       
        public static void PeriodicMaintenance() {
                       
             Task t= new Task(()=>{
                log.AddLast("Entrei na manutenção periodica");
                Dictionary<string, HtmlNode> crawled = AbotCrawler.Crawler.RunToLucene();
                log.AddLast("Terminei o crawling");
                LuceneController.LuceneUsage.TreatMultiUrl(crawled);
                log.AddLast("Terminei o indexamento");
                DatabaseUpdate();

             });
             t.Start();

          
        }


        public static void DatabaseUpdate(){

           Task t= new Task(()=>{
            
                RepositoryLocator.GetRepository().SearchLuceneDatabase();
                log.AddLast("Terminei o update aos utilizadores");
                DispatchEmails();
            
            });
            t.Start();

        
        }

        public static void DispatchEmails() {

            Task t = new Task(() =>
            {                
                Dictionary<string, Dictionary<string, string>> emails_newUrls = RepositoryLocator.GetRepository().updateUsers();

                foreach (string email in emails_newUrls.Keys)
                    SendEmail(email, emails_newUrls[email]);
                log.AddLast("Enviei os emails");

            });
            t.Start();
        
        }



        public static void SendEmail(string email, Dictionary<string, string> urls)
        {

            string body= "";

            foreach(string url in urls.Keys){body += "<a href=" + url + ">" + urls[url] +"</a>"+"<br>";}

            MailMessage mm = new MailMessage(new MailAddress("socialmobilityrecommender@gmail.com"), new MailAddress(email));
            mm.Subject = " Social Mobility Recommender: New urls for you to explore";
            mm.Body = body;
            mm.IsBodyHtml = true;

            SmtpClient smtp = new SmtpClient();
            smtp.Host = "smtp.gmail.com";
            smtp.EnableSsl = true;
            System.Net.NetworkCredential NetworkCred = new System.Net.NetworkCredential();
            NetworkCred.UserName = "socialmobilityrecommender@gmail.com";
            NetworkCred.Password = "travelalong";
            smtp.UseDefaultCredentials = true;
            smtp.Credentials = NetworkCred;
            smtp.Port = 587;
            smtp.Send(mm);                      
        
        }
    }
}