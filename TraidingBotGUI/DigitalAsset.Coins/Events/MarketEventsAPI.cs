using System;
using System.Collections.Generic;
using System.Text;
using Binance.API.Csharp.Client;
using Binance.API;
using System.Threading.Tasks;

namespace DigitalAsset.Coins.Events
{
    /// <summary>
    /// All information are coming from 
    /// </summary>
    public class MarketEventsAPI
    {
        private const string ClientID = "1196_3kc6i5h7yyo0g848s04cc8kcc40kw4so40wssw0gcgokw488go";
        private const string ClientSecret = "53adlf2t590c400kc00ko4cs0gwksk8ckg0skw4okcgk884k44";

        private string AccessToken = "MTNlZDM1ZDk5MWFkYzVjMDdkZmRlMTJmZDFjMTRlMGU4ZjkxYzM3MWYxY2FlZjg0NDE4OWZmOTViMDBiZjJmOA";

        ApiClient apiClient = new ApiClient(ClientID, ClientSecret, @"https://api.coinmarketcal.com", @"https://api.coinmarketcal.com", 3);

        public MarketEventsAPI()
        {
            this.AccessToken = Authenticate().Result;

        }

        private async Task<string> Authenticate()
        {
            string parameter = "?grant_type=client_credentials&client_id=" + ClientID + "&client_secret=" + ClientSecret;

            try
            {
                var result = await apiClient.CallAsyncWebClient<JSON.EventHeader>(Utilitys.EndPoints.AuthToken, parameter);

                return result.access_token;
            }
            catch(Exception)
            {
                return "Error";
            }

        }

        private bool CheckDateFormat(string date)
        {
            if (date.Contains("%2F"))
                return true;
            else
                return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="page"></param>
        /// <param name="max"></param>
        /// <param name="dateRangeStart"></param>
        /// <param name="dateRangeEnd"></param>
        /// <param name="coins"></param>
        /// <param name="categories"></param>
        /// <param name="sortBy">1. hot_events</param>
        /// <param name="showOnly">1. hot_events</param>
        /// <param name="showMetadata"></param>
        /// <returns></returns>
        public async Task<List<JSON.Event>> GetCryptoEvents(int page = 1, int max = 16, string dateRangeStart = "", string dateRangeEnd = "", string coins = "", string categories = "", string sortBy = "", string showOnly = "", string showMetadata = "")
        {
            //Standard Parameter
            string parameter = "?access_token=" + AccessToken + 
                               "&page=" + page + 
                               "&max=" + max;

            //Add filter as parameter
            if (string.IsNullOrWhiteSpace(dateRangeStart) || !CheckDateFormat(dateRangeStart))
                throw new ArgumentException("dateRangeStart is invald");
            else
                parameter += "&dateRangeStart=" + dateRangeStart;
            if (string.IsNullOrWhiteSpace(dateRangeEnd) || !CheckDateFormat(dateRangeStart))
                throw new ArgumentException("dateRangeEnd is invald");
            else
                parameter += "&dateRangeStart=" + dateRangeStart;
            if (!string.IsNullOrWhiteSpace(coins))
                parameter += "&coins" + coins;
            if (!string.IsNullOrWhiteSpace(sortBy))
                parameter += "&sortBy" + sortBy;
            if (!string.IsNullOrWhiteSpace(showOnly))
                parameter += "&showOnly" + showOnly;
            if (!string.IsNullOrWhiteSpace(sortBy))
                parameter += "&showMetadata" + showMetadata;

            //TODO: implement the NEWS-Class properly, instead of a own Parser
            //Get the news as JSON-format
            try
            {
                var result = await apiClient.CallAsyncWebClient<List<JSON.Event>>(Utilitys.EndPoints.Events, parameter);
                //Parse Resultsed
                return result;
            }
            catch(Exception exc)
            {
                throw (new Exception(exc.Message));
            }
        }
    }
}
