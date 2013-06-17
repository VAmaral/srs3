using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuceneController
{
    class Program
    {
        static void Main(string[] args)
        {
           
            /*string text1 = "when i was small, i could sneak up on people";
            string url1 = "www.aivoueu.com";
            LuceneUsage.Arquive(text1, url1);
            
            Console.WriteLine("why hello there;");*/

            /*LinkedList<string> list1 = new LinkedList<string>();
            LinkedList<string> list2 = new LinkedList<string>();
            

            list1.AddLast("a");
            list1.AddLast("b");
            list1.AddLast("c");
            list2.AddLast("c");
            list2.AddLast("b");

            IEnumerable<string> list4 = list2.Intersect<string>(list1);
            foreach(string s in list4)
            Console.WriteLine(s);*/

            LuceneUsage.Arquive("Today was a good day.", "www.todaygood.com");
            LuceneUsage.Arquive("Today was a bad day.", "www.todaybad.com");
            




        }
    }
}
