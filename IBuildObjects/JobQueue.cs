using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace IBuildObjects
{
    public class JobQueue<T> where T : class, IJob
    {
        public ILogger Logger { get; set; }
        
        public int NumberOfThreads { get; set; }

        private readonly BlockingCollection<T> _jobQueue;
        private CancellationTokenSource _jobQueueCancellationTokenSource;

        public JobQueue()
        {
            _jobQueue = new BlockingCollection<T>();
            NumberOfThreads = 1;
        }

        public void StartQueue()
        {
            _jobQueueCancellationTokenSource = new CancellationTokenSource();
            Task.Factory.StartNew(RunQueue,
                _jobQueueCancellationTokenSource.Token,
                TaskCreationOptions.LongRunning, TaskScheduler.Default);

            if(Logger != null) Logger.Debug("Started Job Queue.");
        }

        public void StopQueueAbruptly()
        {
            if (_jobQueueCancellationTokenSource == null) return;
            _jobQueueCancellationTokenSource.Cancel();
            if (Logger != null) Logger.Debug("Stopped Job Queue abruptly.");
        }

        public void StopQueueWhenWorkFinishes()
        {
           _jobQueue.CompleteAdding();
           if (Logger != null) Logger.Debug("Stopping job queue after processing.");
        }

        public void AddJob(T job)
        {
            job.JobId = Guid.NewGuid();
            _jobQueue.Add(job);
        }

        private void RunQueue()
        {
           for(var i = 0; i < NumberOfThreads; i++)
               StartConsumerThread(i);
        }

        private void StartConsumerThread(int count)
        {
            var threadNumber = count;
            var consumerTask = new Task(() =>
            {
                foreach (var job in _jobQueue.GetConsumingEnumerable(_jobQueueCancellationTokenSource.Token))
                {
                    if (_jobQueueCancellationTokenSource.Token.IsCancellationRequested) return;
                    if (Logger != null) Logger.Debug("Thread " + threadNumber + " picked up a job.");

                    job.ThreadId = threadNumber;
                    
                    job.DoWork();
                }
            });

            consumerTask.Start();
        }
    }
}
