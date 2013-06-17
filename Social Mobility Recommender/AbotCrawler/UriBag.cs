using System;
using System.Collections.Generic;
using System.Linq;

namespace AbotCrawler
{
    public class UriBag
    {
        private readonly Dictionary<string, Uri> _bag;

        public UriBag()
        {
            _bag = new Dictionary<string, Uri>();
        }

        public void Add(string name, string uri)
        {
            _bag.Add(name, new Uri(uri));
        }

        public void Remove(string name)
        {
            _bag.Remove(name);
        }

        public Uri Get(string name)
        {
            return _bag[name];
        }

        public void Edit(string name, string uri)
        {
            _bag[name] = new Uri(uri);
        }

        public IEnumerable<KeyValuePair<string, Uri>> AsEnumerable()
        {
            return _bag.AsEnumerable();
        }
    }
}
