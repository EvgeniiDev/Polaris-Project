using Core.Events.Objects;


namespace Core.Events;

public static class EventsCatalog
{
    public static event Action<Dot> PP;
    public static event Action<Dot> Slom;
    public static event Action<NewZigZagEvent> NewZigZag;
    public static event Action<Accumulation> NewAccumulation;
    public static event Action<NewMovingAverageEvent> MovingAverage;
    public static event Action<NewCandleEvent> NewCandle;
    public static event Action<Dot> ReboundFromTheLevel;

    internal static void InvokeNewZigZag(NewZigZagEvent arg) => NewZigZag?.Invoke(arg);
    internal static void InvokeNewAccumulation(Accumulation arg) => NewAccumulation(arg);
    //internal static void InvokeReboundFromTheLevel(List<Dot> arg) => NewZigZag(arg);
    internal static void InvokePP(Dot arg) => PP(arg);
    internal static void InvokeSlom(Dot arg) => Slom(arg);
    internal static void InvokeMovingAverage(NewMovingAverageEvent arg) => MovingAverage?.Invoke(arg);
    internal static void InvokeNewCandle(NewCandleEvent arg) => NewCandle?.Invoke(arg);
}
