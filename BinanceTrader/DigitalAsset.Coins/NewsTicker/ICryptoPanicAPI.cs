using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Binance.API.Csharp.Client.Domain.Interfaces;
using System.Text;

namespace DigitalAsset.Coins.NewsTicker
{
    interface ICryptoPanicAPI
    {

        Task<News> GetCryptoNews(string filter = "", string currencies = "");

    }
}
