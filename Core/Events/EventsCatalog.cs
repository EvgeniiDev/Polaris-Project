using Core.Events.Objects;
using System.Diagnostics;

namespace Core.Events;

public static class EventsCatalog
{
    public static event Action<PP> PP;
    public static event Action<PP> Slom;
    public static event Action<NewZigZagEvent> NewZigZag;
    public static event Action<Accumulation> NewAccumulation;
    public static event Action<NewMovingAverageEvent> MovingAverage;
    public static event Action<NewCandleEvent> NewCandle;
    public static event Action<Dot> ReboundFromTheLevel;

    internal static void InvokeNewZigZag(NewZigZagEvent arg) => NewZigZag?.Invoke(arg);
    internal static void InvokeNewAccumulation(Accumulation arg) => NewAccumulation?.Invoke(arg);
    //internal static void InvokeReboundFromTheLevel(List<Dot> arg) => NewZigZag(arg);
    internal static void InvokePP(PP arg) => PP?.Invoke(arg);
    internal static void InvokeSlom(PP arg) => Slom?.Invoke(arg);
    internal static void InvokeMovingAverage(NewMovingAverageEvent arg) => MovingAverage?.Invoke(arg);
    internal static void InvokeNewCandle(NewCandleEvent arg) => NewCandle?.Invoke(arg);
}
