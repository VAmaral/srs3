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
        LinkedList<string> oldUrl;
        public bool _isEnglish;
        LinkedList<string> newUrl;


        public User(string name, string email, int id, bool isEnglish) {
            _name = name;
            _email = email;
            terms = new LinkedList<string>();
            oldUrl = new LinkedList<string>();
            newUrl = new LinkedList<string>();
            _isEnglish = isEnglish;
        }

        public void AddTerm(string term) { if(!terms.Contains(term)) terms.AddLast(term); }

        public void RemoveTerm(string term) { if(term!=null)terms.Remove(term);}

        public LinkedList<string> GetAllTerms() { return terms; }

        public void TryAddUrl(string url) { if(!oldUrl.Contains(url))newUrl.AddLast(url);}

        public void tryAddMultiUrl(LinkedList<string> urls) { 

            if (urls==null) return;
            foreach (string url in urls) TryAddUrl(url); 
        }

        public LinkedList<string> UpdateUser() {

            LinkedList<string> result = new LinkedList<string>();

            foreach (string url in newUrl)
            {
                result.AddLast(url);
                oldUrl.AddLast(url);
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