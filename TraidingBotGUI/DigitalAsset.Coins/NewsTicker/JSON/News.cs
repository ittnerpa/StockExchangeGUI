using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace DigitalAsset.Coins.NewsTicker
{
    public class News
    {
        public int count { get; set; }
        public string next { get; set; }
        public object previous { get; set; }
        public List<Result> results { get; set; }
    }
}
