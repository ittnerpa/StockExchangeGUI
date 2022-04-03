using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace DigitalAsset.Coins.NewsTicker
{
    public class Source
    {
        public string domain { get; set; }
        public string title { get; set; }
        public string region { get; set; }
        public string path { get; set; }
    }
}
