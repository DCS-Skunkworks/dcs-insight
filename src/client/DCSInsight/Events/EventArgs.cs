using System;
using DCSInsight.JSON;
using System.Collections.Generic;

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

    public class ConnectionEventArgs : EventArgs
    {
        public bool IsConnected { get; set; }
    }

    public class DataEventArgs : EventArgs
    {
        public DCSAPI DCSApi { get; set; }

        public List<DCSAPI> DCSAPIS { get; set; }
    }
}
