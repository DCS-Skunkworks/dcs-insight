using System;
using DCSInsight.JSON;

namespace DCSInsight.Events
{
    public class SendCommandEventArgs : System.EventArgs
    {
        public object Sender { get; set; }
        public DCSAPI APIObject { get; set; }
    }

    public class ErrorEventArgs : System.EventArgs
    {
        public object Sender { get; set; }
        public string Message { get; set; }

        public Exception Ex { get; set; }
    }
}
