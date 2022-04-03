using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json.Linq;
using System.Linq;


namespace DigitalAsset.Coins.NewsTicker
{
    class CustomParser
    {
        public News GetParsedNews(dynamic CoinNews)
        {
            News news = new News
            {
                count = CoinNews["count"] ?? 0,
                next = CoinNews["next"],
                previous = CoinNews["previous"]
            };
            JToken coinNews = CoinNews;

            if (news.count == 0)
                throw (new Exception("No Entries found"));

            List<Result> bodies = new List<Result>();
            var NewsArray = coinNews.Last.First.ToArray();

            foreach (JToken item in NewsArray)
            {
                try
                {
                    bodies.Add(new Result
                    {
                        kind = item.Value<string>("kind"),
                        title = item.Value<string>("title"),
                        published_at = item.Value<DateTime>("published_at"),
                        slug = item.Value<string>("slug"),
                        id = item.Value<int>("postid"),
                        created_at = item.Value<DateTime>("created_at"),
                        url = item.Value<string>("url")
                    }); 
                }catch
                {
                    bodies.Add(new Result());
                }

                JToken VoteToken;
                try
                {
                    VoteToken = item["votes"];
                    Votes vote = new Votes
                    {
                        positive = VoteToken.Value<int>("positive"),
                        negative = VoteToken.Value<int>("negative"),
                        toxic = VoteToken.Value<int>("toxic"),
                        disliked = VoteToken.Value<int>("disliked"),
                        important = VoteToken.Value<int>("important"),
                        liked = VoteToken.Value<int>("liked"),
                        lol = VoteToken.Value<int>("lol"),
                        saved = VoteToken.Value<int>("saved")
                    };
                    bodies[bodies.Count - 1].votes = vote;
                } catch
                {
                    bodies[bodies.Count - 1].votes = null;
                }

                JToken DomainToken;
                try
                {
                    DomainToken = item["source"];

                    Source domain = new Source
                    {
                        domain = DomainToken.Value<string>("domain"),
                        path = DomainToken.Value<string>("path"),
                        region = DomainToken.Value<string>("region"),
                        title = DomainToken.Value<string>("title"),
                    };
                    bodies[bodies.Count - 1].source = domain;
                }
                catch {
                    bodies[bodies.Count - 1].source = null;
                }

                JToken x;
                try
                {
                    x = item["currencies"];
                    if (x != null)
                    {
                        var CurrencieArray = item["currencies"].ToArray();
                        List<Currency> currencies = new List<Currency>();
                        foreach (JToken currencieToken in CurrencieArray)
                        {
                            currencies.Add(new Currency
                            {
                                code = currencieToken.Value<string>("code"),
                                slug = currencieToken.Value<string>("slug"),
                                title = currencieToken.Value<string>("title"),
                                url = currencieToken.Value<string>("url")
                            }
                            );
                        }
                        bodies[bodies.Count - 1].currencies = currencies;
                    }
                }
                catch
                {
                    bodies[bodies.Count - 1].currencies = null;
                }
            }
            news.results = bodies;

            return news;
        }
    }
}
