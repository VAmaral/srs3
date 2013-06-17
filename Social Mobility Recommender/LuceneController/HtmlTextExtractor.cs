using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using HtmlAgilityPack;


namespace LuceneController
{
    class HtmlTextExtractor
    {
        private static Regex _removeRepeatedWhitespaceRegex = new Regex(@"(\s|\n|\r){2,}", RegexOptions.Singleline | RegexOptions.IgnoreCase);

        public static void Extract_all_text_from_webpage(String path)
        {
            HtmlDocument document = new HtmlDocument();
            document.Load(new MemoryStream(File.ReadAllBytes(path)));
            Console.WriteLine(ExtractViewableTextCleaned(document.DocumentNode));
        }

        public static string ExtractViewableTextCleaned(HtmlNode root)
        {
            var chunks = new List<string>();

            foreach (var item in root.DescendantsAndSelf())
            {
                if (item.NodeType == HtmlNodeType.Text)
                {
                    if (item.InnerText.Trim() != "")
                    {
                        chunks.Add(item.InnerText.Trim());
                    }
                }
            }
            return String.Join(" ", chunks);
        }
        //    string s = "";
        //    foreach (var node in root.DescendantsAndSelf())
        //    {
        //        if (!node.HasChildNodes)
        //        {
        //            string text = node.InnerText;
        //            if (!string.IsNullOrEmpty(text))
        //            s += text.Trim() + " ";                     
        //        }
        //    }
        //    return s.Trim();
        //}
        

        public static string ExtractViewableText(HtmlNode node)
        {
            StringBuilder sb = new StringBuilder();
            ExtractViewableTextHelper(sb, node);
            return sb.ToString();
        }

        private static void ExtractViewableTextHelper(StringBuilder sb, HtmlNode node)
        {
            if (node.Name != "script" && node.Name != "style")
            {
                if (node.NodeType == HtmlNodeType.Text)
                {
                    AppendNodeText(sb, node);
                }

                foreach (HtmlNode child in node.ChildNodes)
                {
                    ExtractViewableTextHelper(sb, child);
                }
            }
        }

        private static void AppendNodeText(StringBuilder sb, HtmlNode node)
        {
            string text = ((HtmlTextNode)node).Text;
            if (string.IsNullOrWhiteSpace(text) == false)
            {
                sb.Append(text);

                // If the last char isn't a white-space, add a white space
                // otherwise words will be added ontop of each other when they're only separated by
                // tags
                if (text.EndsWith("\t") || text.EndsWith("\n") || text.EndsWith(" ") || text.EndsWith("\r"))
                {
                    // We're good!
                }
                else
                {
                    sb.Append(" ");
                }
            }
        }

    }
}
