using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace DigitalAsset.Coins.NewsTicker
{
    public class Currency
    {
        public string code { get; set; }
        public string title { get; set; }
        public string slug { get; set; }
        public string url { get; set; }
    }
}
