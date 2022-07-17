using ExchangeConnectors;
using System;
using System.Collections.Generic;
using TradeBot.Algorithms;
using static DataTypes.TimeFrames;

namespace TradeBot.Data;

internal class DataConcentrator
{

    private Dictionary<(Type, string, TimeFrame), DataWorker> dataSources = new();
    private DataDistributor distributor;
    public bool IsStarted;
    public int RunningWorkersAmount;
    private float _totalRps = 10;
    public float TotalRps
    {
        get => _totalRps;
        set
        {
            _totalRps = value;
            SetupRps();
        }
    }

    public DataConcentrator(DataDistributor distributor)
    {
        this.distributor = distributor;
    }

    public void RegisterDataSource(IExchange connector, Type exchangeType, string pair, TimeFrame timeFrame, DateTime? startDate = null)
    {
        var dataWorker = new DataWorker(connector, exchangeType, pair, timeFrame, startDate);
        dataWorker.RegisterDistributor(distributor);
        dataSources.Add((exchangeType, pair, timeFrame), dataWorker);
    }

    public void RunDataSource(Type exchangeType, string pair, TimeFrame timeFrame)
    {
        var key = (exchangeType, pair, timeFrame);
        if (dataSources.ContainsKey(key) && !dataSources[key].IsStarted)
        {
            dataSources[key].Start();
            RunningWorkersAmount++;
        }
        else
            throw new Exception("Нет такого источника инфы");
    }

    public void StopDataSource(Type exchangeType, string pair, TimeFrame timeFrame)
    {
        var key = (exchangeType, pair, timeFrame);
        if (dataSources.ContainsKey(key) && dataSources[key].IsStarted)
        {
            dataSources[key].Stop();
            RunningWorkersAmount--;
        }
        else
            throw new Exception("Нет такого источника инфы");
    }

    public void StartConcentrator()
    {
        foreach (var source in dataSources)
        {
            var (type, pair, timeFrame) = source.Key;
            RunDataSource(type, pair, timeFrame);
        }
        IsStarted = true;
    }

    public void StopConcentrator()
    {
        foreach (var source in dataSources)
        {
            var (type, pair, timeFrame) = source.Key;
            StopDataSource(type, pair, timeFrame);
        }
        IsStarted = false;
    }

    private void SetupRps()
    {
        if (RunningWorkersAmount == 0) return;
        foreach (var dataWorker in dataSources)
            dataWorker.Value.Rps = TotalRps / RunningWorkersAmount;
    }

    //private void Start()
    //{
    //    var amountCandelToGetPerOneReq = 2;
    //    while (IsStarted)
    //        foreach (var source in dataSources.Values)
    //        {
    //            source.CheckNewData(amountCandelToGetPerOneReq);
    //            //Thread.Sleep(100);
    //        }
    //}

}
