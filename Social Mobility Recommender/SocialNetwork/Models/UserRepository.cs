using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using LuceneController;

namespace SocialNetwork.Models
{
    public class UserRepository : IUserRepository
    {
        //dictionary<id, user>
        private static Dictionary<int, User> users = new Dictionary<int,User>();
        private int id = 0;

        public User GetUser(int id) {
            User u;
            users.TryGetValue(id, out u);
            return u;
        }

        public Dictionary<int, LinkedList<string>> GetAllUserTerms(bool isEnglish) {
            Dictionary<int, LinkedList<string>> result = new Dictionary<int, LinkedList<string>>();
            foreach (int key in users.Keys) {
                if (users[key]._isEnglish == isEnglish)
                    result.Add(key, users[key].GetAllTerms());
            }
            return result;
        }

        public void AddUser(string username, string email, bool isEnglish) {
            int idx = id++;
            User u = new User(username, email, id, isEnglish);
            users.Add(idx, u);
        
        }

        public void AddTerm(int id, string term) {
            users[id].AddTerm(term);        
        }

        public void AddMultiTerms(int id, LinkedList<string> terms) {
            foreach (string t in terms) {
                AddTerm(id, t);
            }
        
        }

        public Dictionary<string, LinkedList<int>> GetAllTerms(bool isEnglish)
        {
            LinkedList<string> terms = new LinkedList<string>();
            //dictionary<term, users that contain the term>
            Dictionary<string, LinkedList<int>> connect = new Dictionary<string, LinkedList<int>>();

            foreach (int key in users.Keys) {

                if (!users[key]._isEnglish == isEnglish) continue;

                LinkedList<string> userTerms= users[key].GetAllTerms();
                LinkedList<string> newTerms = CollectTerms(terms, userTerms);
                foreach (string term in newTerms) {
                    connect.Add(term, new LinkedList<int>());
                    terms.AddLast(term);
                }
                foreach (string term in userTerms) { connect[term].AddLast(key);}
            }

            return connect;

        }

        public LinkedList<string> CollectTerms(LinkedList<string> terms, LinkedList<string> userTerms) { 
            LinkedList<string> newTerms= new LinkedList<string>();
            foreach (string term in userTerms) {

                if (!terms.Contains(term)) newTerms.AddLast(term);

            }
            return newTerms;
        }

        public void SearchLuceneDatabase(bool isEnglish) {

            //dictionary<term, users that have that term in preferences>
            Dictionary<string, LinkedList<int>> terms = GetAllTerms(isEnglish);
            //dictionary<term, urls that have that term>
            Dictionary<string, LinkedList<string>> results = new Dictionary<string, LinkedList<string>>();
            //dictionary<user id, terms in profile>
            Dictionary<int, LinkedList<string>> userTerms = GetAllUserTerms(isEnglish);

            //filling in results
            foreach (string term in terms.Keys) 
                results.Add(term, LuceneController.LuceneUsage.FindTerm(term, isEnglish));
            

            //distributing the information among the users
            foreach (int id in userTerms.Keys)
            {
                //Ensuring that the same url contains all the keywords that the user wanted
                
                LinkedList<string> final=null;
                foreach (string term in userTerms[id]) {
                    if (final == null) final = results[term];
                    if (results[term] == null) continue;
                    final.Intersect<string>(results[term]);
                
                }

                //sending the information to the user
                users[id].tryAddMultiUrl(final);
            }


                    
        }

        public void SearchLuceneDatabase() {

            SearchLuceneDatabase(true);
            SearchLuceneDatabase(false);
        }

        public IEnumerable<string> FindTerm(string term) {

            return LuceneUsage.FindTerm(term, false);
        
        
        }

        public IEnumerable<string> FindTerms(string[] terms) {

            IEnumerable<string> result = FindTerm(terms[0]);
            for (int i = 1; i < terms.Length; ++i) {
                result = result.Intersect<string>(FindTerm(terms[i]));
            }

            return result;
        }


        public Dictionary<string, LinkedList<string>> updateUsers() {

            Dictionary<string, LinkedList<string>> emails_urls = new Dictionary<string, LinkedList<string>>();

            foreach (int id in users.Keys)
                emails_urls.Add(users[id]._email, users[id].UpdateUser());
            
            return emails_urls;
        
        }

    }
}