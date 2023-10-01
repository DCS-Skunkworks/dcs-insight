using System;
using DCSInsight.JSON;

namespace DCSInsight.Events
{
    public class SendCommandEventArgs : EventArgs
    {
        public object Sender { get; set; }
        public DCSAPI APIObject { get; set; }
    }

    public class ErrorEventArgs : EventArgs
    {
        public object Sender { get; set; }
        public string Message { get; set; }

        public Exception Ex { get; set; }
    }
}
