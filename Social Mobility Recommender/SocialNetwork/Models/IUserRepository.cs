using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace SocialNetwork.Models
{
    public interface IUserRepository
    {
         User GetUser(int id);
         Dictionary<int, LinkedList<string>> GetAllUserTerms(bool isEnglish);
         void AddUser(string username, string email, bool isEnglish);
         void AddTerm(int id, string term);
         void AddMultiTerms(int id, LinkedList<string> terms);
         Dictionary<string, LinkedList<int>> GetAllTerms(bool isEnglish);
         LinkedList<string> CollectTerms(LinkedList<string> terms, LinkedList<string> userTerms);
         void SearchLuceneDatabase(bool isEnglish);
         Dictionary<string, LinkedList<string>> updateUsers();
         IEnumerable<string> FindTerms(string[] terms); 
         void SearchLuceneDatabase();
    }
}
