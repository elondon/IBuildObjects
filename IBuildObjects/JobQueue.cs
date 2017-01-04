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
        }

        public void StopQueueAbruptly()
        {
            if (_jobQueueCancellationTokenSource == null) return;
            _jobQueueCancellationTokenSource.Cancel();
        }

        public void StopQueueWhenWorkFinishes()
        {
           _jobQueue.CompleteAdding();
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
                var jobRunning = false;
                while (!_jobQueueCancellationTokenSource.Token.IsCancellationRequested)
                {
                    if (jobRunning) continue;

                    T job;
                    _jobQueue.TryTake(out job);

                    if (job == null) continue;
                    jobRunning = true;
                    job.ThreadId = threadNumber;

                    job.WorkCompleted = () => jobRunning = false;
                    job.DoWork();
                }
            });

            consumerTask.Start();
        }
    }
}
