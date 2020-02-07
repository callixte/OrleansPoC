using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Orleans;

namespace HelloWorld.Interfaces
{
    public interface IPoCGrain: IGrainWithGuidKey
    {
        Task<ITick> GetLastTickAsync();

        Task StopAsync();
    }
}
