using System;
using System.Collections.Generic;
using System.Text;

namespace DigitalAsset.Coins.Events.JSON
{
    public class Event
    {
        public int id { get; set; }
        public string title { get; set; }
        public List<Coin> coins { get; set; }
        public DateTime date_event { get; set; }
        public DateTime created_date { get; set; }
        public string description { get; set; }
        public string proof { get; set; }
        public string source { get; set; }
        public bool is_hot { get; set; }
        public int vote_count { get; set; }
        public int positive_vote_count { get; set; }
        public int percentage { get; set; }
        public List<Category> categories { get; set; }
        public string tip_symbol { get; set; }
        public string tip_adress { get; set; }
        public string twitter_account { get; set; }
        public bool can_occur_before { get; set; }
    }
}
