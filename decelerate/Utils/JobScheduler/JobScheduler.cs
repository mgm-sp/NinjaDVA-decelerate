using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace decelerate.Utils.JobScheduler
{
    public class JobScheduler
    {
        public JobScheduler(IEnumerable<Job> jobs)
        {
            _jobs = jobs.ToList();
        }

        public void AddJob(Job job)
        {
            _jobs.Add(job);
        }

        public void Run()
        {
            foreach (var job in _jobs)
            {
                job.TryExecute();
            }
        }

        private List<Job> _jobs;
    }
}
