using System;
using System.Collections.Generic;
using System.Text;

namespace DigitalAsset.Coins.Events.Utilitys
{
    public static class EndPoints
    {
        //Endpoint to the Authentification Token
        public static readonly string AuthToken = "/oauth/v2/token";
        //Endpoint to receive a List of all supported coins
        public static readonly string Coin = "/v1/coins";
        //Endpoint to receive a List of all events
        public static readonly string Events = "/v1/events";
        //Endpoint to receive a List of all categories
        public static readonly string Category = "/v1/categroies";
    }
}
