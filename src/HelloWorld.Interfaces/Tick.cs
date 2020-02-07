using System;

namespace HelloWorld.Interfaces
{
    public class Tick: ITick
    {
        private readonly DateTime _timeStamp;
        private readonly string _message;
        public Tick(string message)
        {
            this._timeStamp = DateTime.UtcNow;
            this._message = message;
        }

        public string GetMessage()
        {
            return $"{this._timeStamp:u} - {this._message}";
        }
    }
}
