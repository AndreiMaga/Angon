using Angon.common.comprotocols.requests;

namespace Angon.common.runner.runners
{
    class JobRequestRunner
    {
        public static void Run(JobRequest jr)
        {
            // After the JobHeader the master will send the job
            OrderReciever.RecieveJob(jr);

            
        }
    }
}
