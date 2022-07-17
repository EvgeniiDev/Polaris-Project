using DataTypes;
using ExchangeConnectors.Connectors;
using Log;
using static DataTypes.TimeFrames;

namespace TestExchange
{
    class Program
    {
        static async Task Main()
        {
            var key = "UEmwbCVtgeRDa3By0d0lsyeh1l1vwSs15TPfLHH7pRmkymWAcOVEAHPxBCBiOYum";
            var secret = "OZFArrDVZK8y9XpT90RnO8UEUgBDteiobjKzFH4FvoGQusrypvvTnTsryGKZ7MjL";
            var bConnector = new BinanceConnector(key, secret);

                var gg = bConnector.CreateOrder("BNBBUSD", OrderType.BuyLimit, 180, 0.1m);


                await bConnector.CancelOrder("BNBBUSD", await gg);


                bConnector.SubscibeOnNewKlines("ETHUSDT", TimeFrame.m1, BConnector_Notify);
                bConnector.SubscibeOnNewKlines("ETHUSDT", TimeFrame.m5, BConnector_Notify);
                bConnector.SubscibeOnNewKlines("ETHUSDT", TimeFrame.m15, BConnector_Notify);


                var t = await bConnector.GetPrices();
                Thread.Sleep(10000);

            
            Console.WriteLine();




            // var exch = new Exchange(bConnector);
            // //var account = new Account();
            // var candles = await bConnector.GetCandles("ETHUSDT", TimeFrame.D1,
            //                                    new DateTime(2014, 8, 24), new DateTime(2022, 2, 15));


            // //JoinBoxes(accumulations);
            // var allowedPair =  new [] { "BTCUSDT", "ETHUSDT" };
            // var allowedTimeFrame = new[] { KlineInterval.FourHour, KlineInterval.OneDay, KlineInterval.OneWeek };

            // foreach(var a in allowedPair)
            //     foreach(var b in allowedTimeFrame)
            //         new DataWorker { Pair = a, TimeFrame = b }.Run();


            // //var a = new DataWorker { Pair ="BTCUSDT", TimeFrame= KlineInterval.FiveMinutes };
            // //a.Run();
            // //Thread.Sleep(15000);
            // //a.Exit();

            while (true)
            {
                Thread.Sleep(1000);
            }
        }

        private static void BConnector_Notify(Kline candle)
        {
            Console.WriteLine($"{candle.TimeStamp} {candle.TimeFrame} {candle}");
        }

    }
}
