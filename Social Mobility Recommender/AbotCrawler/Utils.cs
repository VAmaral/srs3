using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace AbotCrawler
{
    class Utils
    {
        public static void Save(List<CrawledWebPage> pages)
        {
            try
            {
                using (Stream stream = File.Open("data.bin", FileMode.Create))
                {
                    BinaryFormatter bin = new BinaryFormatter();
                    bin.Serialize(stream, pages);
                }
            }
            catch (IOException)
            {
            }
        }

        public static List<CrawledWebPage> Load()
        {
            using (Stream stream = File.Open("data.bin", FileMode.Open))
            {
                BinaryFormatter bin = new BinaryFormatter();

                var webpages = (List<CrawledWebPage>)bin.Deserialize(stream);
                return webpages;
            }
        }
    }
}
