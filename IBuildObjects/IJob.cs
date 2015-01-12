using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IBuildObjects
{
    public interface IJob
    {
        Guid JobId { get; set; }
        int ThreadId { get; set; }

        void DoWork();

        Action WorkStarted { get; set; }
        Action WorkCompleted { get; set; }
        Action<JobProgress> ProgressReported { get; set; }
    }
}
