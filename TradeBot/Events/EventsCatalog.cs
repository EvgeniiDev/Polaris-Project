using System;
using System.Collections.Generic;
using TradeBot.Data;
using static DataTypes.TimeFrames;
using static TradeBot.Algorithms.MA.MovingAverage;

namespace TradeBot.Events
{
    public static class EventsCatalog
    {
        internal static event Action<Dot> PP;
        internal static event Action<Dot> Slom;
        internal static event Action<List<Dot>> NewZigZag;
        internal static event Action<Accumulation> NewAccumulation;
        internal static event Action<string, decimal, TimeFrame, MAType> MovingAverage;
        internal static event Action<NewCandleEvent> NewCandle;
        //internal static event Action<Dot> ReboundFromTheLevel;

        internal static void InvokeNewZigZag(List<Dot> arg) => NewZigZag(arg);
        internal static void InvokeNewAccumulation(Accumulation arg) => NewAccumulation(arg);
        //internal static void InvokeReboundFromTheLevel(List<Dot> arg) => NewZigZag(arg);
        internal static void InvokePP(Dot arg) => PP(arg);
        internal static void InvokeSlom(Dot arg) => Slom(arg);
        internal static void InvokeMovingAverage(string pair, decimal val, TimeFrame tf, MAType type) => MovingAverage(pair, val, tf, type);
        internal static void InvokeNewCandle(NewCandleEvent arg) => NewCandle(arg);
    }
}
