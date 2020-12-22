using Angon.common.comprotocols.requests;
using Angon.common.headers;

namespace Angon.common.runner.runners
{
    class JobRequestRunner
    {
        public static void Run(RequestWithHeader<JobHeader> jr)
        {
            // After the JobHeader the master will send the job
            OrderReciever.RecieveJob(jr);

            
        }
    }
}
