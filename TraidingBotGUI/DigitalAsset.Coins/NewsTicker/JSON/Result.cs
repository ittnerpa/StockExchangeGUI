using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace DigitalAsset.Coins.NewsTicker
{
    public class Result
    {
        public string kind { get; set; }
        public string domain { get; set; }
        public Source source { get; set; }
        public string title { get; set; }
        public DateTime published_at { get; set; }
        public string slug { get; set; }
        public List<Currency> currencies { get; set; }
        public int id { get; set; }
        public DateTime created_at { get; set; }
        public string url { get; set; }
        public Votes votes { get; set; }
    }
}
