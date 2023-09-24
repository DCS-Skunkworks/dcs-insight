using Newtonsoft.Json.Serialization;
using System;
using System.Diagnostics;

namespace DCSInsight
{
    public class ConsoleTraceWriter : ITraceWriter
    {
        public TraceLevel LevelFilter
        {
            // trace all messages (Verbose and above)
            get { return TraceLevel.Verbose; }
        }

        public void Trace(TraceLevel level, string message, Exception ex)
        {
            if (ex != null)
            {
                Debug.WriteLine(level.ToString() + ": " + message + " Ex: " + ex.Message);
            }
            else
            {
                Debug.WriteLine(level.ToString() + ": " + message);
            }
        }
    }
}
