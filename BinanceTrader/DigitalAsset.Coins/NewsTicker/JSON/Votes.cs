using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;


namespace DigitalAsset.Coins.NewsTicker
{
    public class Votes
    {
        public int negative { get; set; }
        public int positive { get; set; }
        public int important { get; set; }
        public int liked { get; set; }
        public int disliked { get; set; }
        public int lol { get; set; }
        public int toxic { get; set; }
        public int saved { get; set; }
    }
}
