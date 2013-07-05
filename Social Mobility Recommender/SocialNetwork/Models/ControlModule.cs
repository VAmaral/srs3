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

        //public static class Email
        //{



        //    public static void sendEmail(string email, string title, string body)
        //    {
        //        WebMail.Send(email, title, body);
        //    }


        //}

        public static Thread ad_infinitum = new Thread(()=>
            {
                LuceneUsage.TreatMultiUrl(AbotCrawler.Crawler.RunToLucene());
                
            
            
            
            
            
            });


        

        public static void PeriodicMaintenance() {

           


            //Thread background = new Thread(() => {
             Task t= new Task(()=>{

                Dictionary<string, HtmlNode> crawled = AbotCrawler.Crawler.RunToLucene();
                LuceneController.LuceneUsage.TreatMultiUrl(crawled);
                DatabaseUpdate();

             });
             t.Start();

            //});
            //background.Start();  
          
        }


        public static void DatabaseUpdate(){

           Task t= new Task(()=>{
            
                RepositoryLocator.GetRepository().SearchLuceneDatabase();
                Dictionary<string, LinkedList<string>> emails_newUrls = RepositoryLocator.GetRepository().updateUsers();

                foreach (string email in emails_newUrls.Keys)
                    SendEmail(email, emails_newUrls[email]);
            
            });
            t.RunSynchronously();

            //    RepositoryLocator.GetRepository().SearchLuceneDatabase();
            //    Dictionary<string, LinkedList<string>> emails_newUrls = RepositoryLocator.GetRepository().updateUsers();

            //    foreach (string email in emails_newUrls.Keys)
            //        SendEmail(email, emails_newUrls[email]);

            //});
            //background.Start();  
        
        
        }
       

       
        public static void SendEmail(string email, LinkedList<string> urls) {

            string body= "";

            foreach(string url in urls){
            body=body + "<a href=" + url + ">" + url +"</a>"+"<br>";
            }

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

            //WebMail.SmtpServer = "smtp.gmail.com";
            //WebMail.SmtpPort = 587;
            //WebMail.EnableSsl = true;
            //WebMail.UserName = "socialmobilityrecommender@gmail.com";
            //WebMail.Password = "travelalong";
            //WebMail.From = "socialmobilityrecommender@gmail.com";
            //Email.sendEmail(email, " Social Mobility Recommender: New urls for you to explore", body);
        
        
        }



    }
}