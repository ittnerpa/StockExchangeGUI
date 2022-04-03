using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using DigitalAsset.Coins.Events.JSON;

namespace DigitalAsset.Coins.Events
{
    interface IMarketEventsAPI
    {
        Task<List<Coin>> GetCoinList();

        Task<List<DigitalAsset.Coins.Events.JSON.Event>> GetEventList();


    }
}
