using System;
using System.Collections.Generic;
using System.Text;
using Binance.API.Csharp.Client.Domain.Interfaces;
using Binance.API.Csharp.Client.Models.Enums;
using Binance.API.Csharp.Client;
using System.Threading.Tasks;

namespace DigitalAsset.Coins.NewsTicker
{
    public class CryptoPanicAPI : ICryptoPanicAPI
    {
        //Key to authenticate with the newschannel
        private readonly string authToken = "3c45cf764f29131e1fa61b913b6ab46351c43952";
        //API-Member
        private IApiClient apiClient;

        /// <summary>
        /// Initiliaze API to Cryptopanic
        /// </summary>
        public CryptoPanicAPI()
        {
            this.apiClient = new ApiClient(authToken, "", @"https://cryptopanic.com", @"https://cryptopanic.com", 2);
        }

        public async Task<News> GetCryptoNews(string filter = "", string currencies = "")
        {
            //
            string parameter = "?auth_token=" + authToken;

            //Add filter as parameter
            if (!string.IsNullOrWhiteSpace(filter))
                parameter += "&filter" + filter;
            if (!string.IsNullOrWhiteSpace(currencies))
                parameter += "&currencies" + currencies;

            try
            {
                var result = await apiClient.CallAsyncWebClient<News>(Endpoints.APIEndpoint, parameter);

                return result;
            }
            catch (Exception e) //Cant use JSON Parser -> use own Parser
            {
                var result = await apiClient.CallAsyncWebClient<dynamic>(Endpoints.APIEndpoint, parameter);
                CustomParser _parser = new CustomParser();
                var parsedResult = _parser.GetParsedNews(result);

                return parsedResult;
            }
        }
    }
}
