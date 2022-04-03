using System;
using System.Collections.Generic;
using System.Text;

namespace DigitalAsset.Coins.Events.JSON
{
    class EventHeader
    {
        public string access_token { get; set; }
        public int expires_in { get; set; }
        public string token_type { get; set; }
        public object scope { get; set; }
    }
}
