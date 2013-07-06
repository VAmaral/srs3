using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SocialNetwork.Models
{
    public class User
    {

        public string _name;
        public string _email;
        public int _id;
        LinkedList<string> terms;
        Dictionary<string, string> oldUrl;
        public bool _isEnglish;
        Dictionary<string, string> newUrl;


        public User(string name, string email, int id, bool isEnglish) {
            _name = name;
            _email = email;
            terms = new LinkedList<string>();
            oldUrl = new Dictionary<string, string>();
            newUrl = new Dictionary<string, string>();
            _isEnglish = isEnglish;
        }

        public void AddTerm(string term) { if(!terms.Contains(term)) terms.AddLast(term); }

        public void RemoveTerm(string term) { if(term!=null)terms.Remove(term);}

        public LinkedList<string> GetAllTerms() { return terms; }

        public void TryAddUrl(string url, string title) { if(!oldUrl.ContainsKey(url))newUrl.Add(url, title);}

        public void tryAddMultiUrl(Dictionary<string,string> urls) { 

            if (urls==null) return;
            foreach (string url in urls.Keys) TryAddUrl(url, urls[url]); 
        }

        public Dictionary<string,string> UpdateUser() {

            Dictionary<string, string> result = new Dictionary<string, string>();

            foreach (string url in newUrl.Keys)
            {
                result.Add(url, newUrl[url]);
                oldUrl.Add(url, newUrl[url]);
            }
            newUrl.Clear();
            return result;
        }

        public void ResetTastes(LinkedList<string> newTerms) {

            oldUrl.Clear();
            terms = newTerms;
        
        }

    }
}