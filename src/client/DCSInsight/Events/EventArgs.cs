using System;
using DCSInsight.JSON;
using System.Collections.Generic;

namespace DCSInsight.Events
{
    public class SendCommandEventArgs : EventArgs
    {
        public SendCommandEventArgs(DCSAPI apiObject)
        {
            APIObject = apiObject;
        }

        public DCSAPI APIObject { get; }
    }

    public class ErrorEventArgs : EventArgs
    {
        public ErrorEventArgs(string message, Exception ex)
        {
            Message = message;
            Ex = ex;
        }

        public string Message { get; }

        public Exception Ex { get; }
    }

    public class ConnectionEventArgs : EventArgs
    {
        public bool IsConnected { get; init; }
    }

    public class DataEventArgs : EventArgs
    {
        public DataEventArgs(DCSAPI? dcsApi, List<DCSAPI>? dcsapis)
        {
            DCSApi = dcsApi;
            DCSAPIS = dcsapis;
        }

        public DCSAPI? DCSApi { get; }

        public List<DCSAPI>? DCSAPIS { get; }
    }
}
