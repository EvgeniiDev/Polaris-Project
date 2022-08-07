using Log;
using MethodTimer;
using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;

namespace ExchangeConnectors
{
    public class ResponseQueue
    {
        private static System.Timers.Timer _timer;
        //todo переписать этот костыль на TaskSheduler
        private readonly int _rps;
        private ConcurrentQueue<Task> _queue = new();
        //private static PriorityQueue<Task> _taskQueue = new();

        public ResponseQueue(int rps = 3)
        {
            _rps = rps;
            SetTimer();
        }

        private void SetTimer()
        {
            // Create a timer with a two second interval.
            _timer = new System.Timers.Timer(1000);
            // Hook up the Elapsed event for the timer. 
            _timer.Elapsed += OnTimedEvent;
            _timer.AutoReset = true;
            _timer.Enabled = true;
        }

        private void OnTimedEvent(object source, ElapsedEventArgs e) => StartExecute();

        private void StartExecute()
        {
            var amount = 0;
            for (var i = 0; i < _rps; i++)
            {
                if (_queue.TryDequeue(out var command))
                {
                    command.Start();
                    amount++;
                    Thread.Sleep(1000 - 200 / _rps);//для равномерного распределения запросов в рамках 1 секунды
                }
            }
            Logger.SendTimerData("CurrentRps", amount);
            Logger.SendTimerData("RequestedRps", _rps);
            Logger.SendTimerData("RequestQueueLag", _queue.Count);
        }

        [Time]
        public async Task<T> Send<T>(Func<Task<T>> command)
        {
            var task = new Task<Task<T>>(async () =>
            {
                var stopwatch = Stopwatch.StartNew();
                command().Wait();
                Logger.SendTimerData("QueueRequestDuration", stopwatch.ElapsedMilliseconds);
                return await command();
            }, TaskCreationOptions.PreferFairness);

            //var task = new Task<Task<T>>(command, TaskCreationOptions.PreferFairness);

            _queue.Enqueue(task);
            //_taskQueue.Enqueue(0, task);
            return await await task;
        }
    }
}
