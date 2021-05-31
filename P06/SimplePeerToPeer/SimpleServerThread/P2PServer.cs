using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;

namespace SimpleServerThread {
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, UseSynchronizationContext = true)] // Use .NET Synch
    public class P2PServer : P2PServerContract {
        public static readonly Dictionary<uint, Job> Jobs = new Dictionary<uint, Job>();
        public static readonly Dictionary<uint, JobResult> CompleteJobs = new Dictionary<uint, JobResult>();
        private static readonly Random _rand = new Random();
        public List<Job> GetJobs() {
            try {
                List<Job> unshuffledJobs = Jobs.Values.ToList();
                return Shuffle(unshuffledJobs);
            } catch (Exception e) {
                throw new FaultException(e.Message);
            }
        }
        private List<Job> Shuffle(List<Job> unshuffled) { // https://en.wikipedia.org/wiki/Fisher%E2%80%93Yates_shuffle
            for (int i = unshuffled.Count - 1; i > 1; i--) {
                int j = _rand.Next(0, i); // 0 <= j <= i
                Job original = unshuffled[j];
                unshuffled[j] = unshuffled[i];
                unshuffled[i] = original;
            }
            return unshuffled;
        }
        public void SubmitJob(JobResult job) {
            try {
                bool removed = Jobs.Remove(job.JobID);
                CompleteJobs.Add(job.JobID, job);
            } catch (Exception e) {
                throw new FaultException(e.Message);
            }
        }
    }
}
