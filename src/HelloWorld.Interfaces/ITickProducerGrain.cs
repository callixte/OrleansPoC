using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Orleans;

namespace HelloWorld.Interfaces
{
    public interface ITickProducerGrain: IGrainWithGuidKey
    {
        Task PushTickAsync(string message);
    }
}
