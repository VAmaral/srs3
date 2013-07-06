using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using HtmlAgilityPack;
using Lucene.Net.Analysis;
using Lucene.Net.Analysis.BR;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.QueryParsers;
using Lucene.Net.Search;
using Lucene.Net.Store;
using System.Windows;



namespace LuceneController
{
    public class LuceneUsage
    {
        public static BrazilianAnalyzer brazilAnalyzer = new BrazilianAnalyzer(Lucene.Net.Util.Version.LUCENE_30);
        public static StandardAnalyzer englishAnalyzer = new StandardAnalyzer(Lucene.Net.Util.Version.LUCENE_30);
        public static string basePath;
        static  Object _monEn = new Object();
        static Object _monPt = new Object();


        public static void TreatMultiUrl(Dictionary<string, HtmlNode> files) {

            if (files == null) return;

            foreach (string url in files.Keys) 

                TreatAUrl(files[url], url);
            
            
        
        }



        public static void TreatAUrl(HtmlNode info, string url ) {
            string title;
            string text = HtmlTextExtractor.ExtractViewableTextCleaned(info, out title);
            Arquive(text, title,url,IsEnglish(text));

        
        }


        


        public static bool IsEnglish(string text)
        {
                     
            Lucene.Net.Store.Directory directoryBR = new RAMDirectory();
            Lucene.Net.Store.Directory directoryEN = new RAMDirectory();

            IndexWriter iwriterBr = new IndexWriter(directoryBR, brazilAnalyzer, true, new IndexWriter.MaxFieldLength(25000));
            IndexWriter iwriterEn = new IndexWriter(directoryEN, englishAnalyzer, true, new IndexWriter.MaxFieldLength(25000));

            Document doc = new Document();

            doc.Add(new Field("detectLang", text,Field.Store.NO, Field.Index.ANALYZED));

            iwriterBr.AddDocument(doc);
            iwriterEn.AddDocument(doc);

            iwriterBr.Dispose();
            iwriterEn.Dispose();
            
            //SE O Nº DE TERMOS ANALIZADOS EM INGLÊS FOR MAIOR QUE EM PORTUGUÊS QUER DIZER QUE NÃO ELIMINOU AS STOPWORDS, LOGO É PORTUGUÊS
            TermEnum termsBr = new IndexSearcher(directoryBR, true).IndexReader.Terms();
            string term;
            int x=0;

            while (termsBr.Next()) { term= termsBr.Term.ToString() ; ++x; }

            TermEnum termsEn = new IndexSearcher(directoryEN, true).IndexReader.Terms();
            int y=0;
            while (termsEn.Next()) { term = termsEn.Term.ToString(); ++y; }

            return y < x;    
        }

        public static void Arquive(string text, string title, string url, bool isEnglish)
        {
            

            //INICIALIZAR VARIÁVEIS CONSOANTE SER EM INGLÊS OU EM PORTUGUÊS
            string directory;
            Lucene.Net.Analysis.Analyzer analyzer;
            Object _mon= isEnglish? _monEn : _monPt;
            InitPathAndAnalyzer(out directory, out analyzer, isEnglish);
                      
            //CRIAR UM NOVO DOCUMENTO COM A INFORMAÇÃO NECESSÁRIA
            string hash= text.GetHashCode().ToString();
            Document document = new Document();
            document.Add(new Field("title", title,  Field.Store.YES,    Field.Index.NOT_ANALYZED));
            document.Add(new Field("url",   url,    Field.Store.YES,    Field.Index.NOT_ANALYZED));
            document.Add(new Field("text",  text,   Field.Store.NO,     Field.Index.ANALYZED));
            document.Add(new Field("hash",  hash,   Field.Store.YES,    Field.Index.NOT_ANALYZED));
            
                      
            Monitor.Enter(_mon);
            Lucene.Net.Store.Directory dir = Lucene.Net.Store.FSDirectory.Open(new DirectoryInfo(directory));
            if (System.IO.Directory.GetFiles(directory).Length == 0)
            {
                IndexWriter init = new IndexWriter(dir, analyzer, true, new IndexWriter.MaxFieldLength(25000));
                init.Dispose();
            }

            //VERIFICAR SE O DOCUMENTO JÁ EXISTE E SE CONTEM A MESMA INFORMAÇÃO QUE JÁ LÁ SE ENCONTRA

            IndexSearcher isearcher = new IndexSearcher(dir, false);
            //O CAMPO DE PROCURA SERA O URL
            QueryParser parser = new QueryParser(Lucene.Net.Util.Version.LUCENE_30, "url", analyzer);
            Query query = parser.Parse(url);
            int s = isearcher.IndexReader.NumDocs();
            //Console.WriteLine(isearcher.IndexReader.NumDocs());

            ScoreDoc[] hits = isearcher.Search(query, null, 1000).ScoreDocs;
            if(hits.Length>0){
                 Document doc = isearcher.Doc(hits[0].Doc);

                //É IGUAL
                 if (doc.Get("hash").Equals(hash))
                 {
                     Monitor.Exit(_mon);
                     return;
                 }
                 //É DIFERENTE
                 else
                 {
                     isearcher.IndexReader.DeleteDocument(hits[0].Doc);
                    
                 }
            }
            isearcher.Dispose();
            //ACEDER AO INDEX COM A INFORMAÇÃO INDEXADA DA LINGUAGEM CORRECTA E ADICIONAR O NOVO DOCUMENTO
            IndexWriter iwriter = new IndexWriter(dir, analyzer, false, new IndexWriter.MaxFieldLength(25000));
            iwriter.AddDocument(document);
            iwriter.Optimize();
            iwriter.Dispose();
            Monitor.Exit(_mon);           
        }

        public static void Arquive(string text, string title, string url) { Arquive(text, title, url, IsEnglish(text)); }

        public static void InitPathAndAnalyzer(out string directory, out Analyzer analyzer, bool english)
        {
           
           
            if (english)
            {
                directory = basePath + "LuceneController\\IndexedDB\\IndexEn";
                analyzer = englishAnalyzer;
            }
            else
            {
                directory = basePath + "LuceneController\\IndexedDB\\IndexPt";
                analyzer = brazilAnalyzer;
            }
        }

        public static Dictionary<string, string> FindTerm(string term, bool english)
        {

           

            string directory;
            Lucene.Net.Analysis.Analyzer analyzer;

            InitPathAndAnalyzer(out directory, out analyzer, english);

            Object _mon = english ? _monEn : _monPt;
            
            
            
            Lucene.Net.Store.Directory dir = Lucene.Net.Store.FSDirectory.Open(new DirectoryInfo(directory));
            IndexSearcher isearcher;
            Monitor.Enter(_mon);

            if (dir.ListAll().Length == 0) {
                IndexWriter iwriter = new IndexWriter(dir, analyzer, true , new IndexWriter.MaxFieldLength(25000));
                iwriter.Dispose();
            }

            isearcher = new IndexSearcher(dir,true);
            Monitor.Exit(_mon);
            //O CAMPO DE PROCURA SERA O URL
            QueryParser parser = new QueryParser(Lucene.Net.Util.Version.LUCENE_30, "text", analyzer);
            Query query = parser.Parse(term);
            int s= isearcher.IndexReader.NumDocs();
            ScoreDoc[] hits = isearcher.Search(query, null, 1000).ScoreDocs;

            Dictionary<string, string> result = new Dictionary<string, string>();
            

            for (int i = 0; i < hits.Length;++i)
            {
                Document doc = isearcher.Doc(hits[i].Doc);
                if(result.ContainsKey(doc.Get("url")))continue;
                result.Add(doc.Get("url"), doc.Get("title"));
            }
            isearcher.Dispose();
            return result;

        }

        public static Dictionary<string, string>[] FindTerms(string[] terms, bool english)
        {

            Dictionary<string, string>[] result = new Dictionary<string, string>[terms.Length];
            for (int i = 0; i < terms.Length; ++i) {
                result[i] = FindTerm(terms[i], english);
            }
            return result;        
        }


        public static void DeleteDocument(string url, bool english)
        {

            string directory;
            Lucene.Net.Analysis.Analyzer analyzer;

            InitPathAndAnalyzer(out directory, out analyzer, english);

            Object _mon = english ? _monEn : _monPt;

            Lucene.Net.Store.Directory dir = Lucene.Net.Store.FSDirectory.Open(new DirectoryInfo(directory));
            Monitor.Enter(_mon);
            IndexSearcher isearcher = new IndexSearcher(dir, false);
            //O CAMPO DE PROCURA SERA O URL
            QueryParser parser = new QueryParser(Lucene.Net.Util.Version.LUCENE_30, "url", analyzer);
            Query query = parser.Parse(url);

            //Console.WriteLine(isearcher.IndexReader.NumDocs());

            ScoreDoc[] hits = isearcher.Search(query, null, 1000).ScoreDocs;
            if (hits.Length > 0)
            {
                Document doc = isearcher.Doc(hits[0].Doc);
                isearcher.IndexReader.DeleteDocument(hits[0].Doc);
                                
            }
            Monitor.Exit(_mon);
        }

        

    }
}
