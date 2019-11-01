using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace decelerate.Utils.JobScheduler
{
    public class Job
    {
        public Job(string fileName, string arguments, TimeSpan interval)
        {
            _fileName = fileName;
            _arguments = arguments;
            _interval = interval;
            _nextExecution = DateTime.UtcNow + interval;
            TryExecute();
        }

        public void TryExecute()
        {
            if (DateTime.UtcNow >= _nextExecution)
            {
                /* Execute the job: */
                Process.Start(_fileName, _arguments);
                /* Set next execution: */
                _nextExecution = DateTime.UtcNow + _interval;
            }
        }

        private readonly string _fileName;
        private readonly string _arguments;
        private readonly TimeSpan _interval;
        private DateTime _nextExecution;
    }
}
