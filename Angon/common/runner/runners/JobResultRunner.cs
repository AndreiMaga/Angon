using Angon.common.comprotocols.requests;
using Angon.common.headers;

namespace Angon.common.runner.runners
{
    class JobResultRunner
    {
        public static void Run(RequestWithHeader<JobResultHeader> jr)
        {
            OrderReciever.RecieveFinishedJob(jr);
        }
    }
}
