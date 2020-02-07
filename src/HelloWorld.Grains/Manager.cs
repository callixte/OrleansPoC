using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using HelloWorld.Interfaces;

namespace HelloWorld.Grains
{
    public class Manager
    {
        private Tick _lastTick;

        public void OnReceiveTick(Tick item)
        {
            this._lastTick = item;
        }

        internal Task<ITick> GetLastTickAsync()
        {
            return Task.FromResult<ITick>(this._lastTick);
        }
    }
}
