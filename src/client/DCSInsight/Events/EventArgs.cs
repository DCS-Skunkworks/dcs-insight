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

    public class CommsErrorEventArgs : EventArgs
    {
        public CommsErrorEventArgs(string shortMessage, Exception ex)
        {
            ShortMessage = shortMessage;
            Ex = ex;
        }

        public string ShortMessage { get; }
        
        public Exception Ex { get; }
    }

    public class ConnectionEventArgs : EventArgs
    {
        public bool IsConnected { get; init; }
    }

    public class CommandDataEventArgs : EventArgs
    {
        public CommandDataEventArgs(DCSAPI dcsApi)
        {
            DCSApi = dcsApi;
        }

        public DCSAPI? DCSApi { get; }
    }

    public class APIDataEventArgs : EventArgs
    {
        public APIDataEventArgs(List<DCSAPI> dcsapis)
        {
            DCSAPIS = dcsapis;
        }

        public List<DCSAPI>? DCSAPIS { get; }
    }
}
