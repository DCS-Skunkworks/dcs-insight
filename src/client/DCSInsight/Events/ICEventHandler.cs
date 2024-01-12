using System;
using System.Collections.Generic;
using DCSInsight.Interfaces;
using DCSInsight.JSON;
using DCSInsight.Misc;

namespace DCSInsight.Events
{
    internal static class ICEventHandler
    {
        public delegate void SendCommandEventHandler(SendCommandEventArgs e);
        public static event SendCommandEventHandler OnSendCommand;
        
        public static void AttachCommandListener(ICommandListener listener)
        {
            OnSendCommand += listener.SendCommand;
        }

        public static void DetachCommandListener(ICommandListener listener)
        {
            OnSendCommand -= listener.SendCommand;
        }

        public static void SendCommand(DCSAPI api)
        {
            // remove earlier result, no need to send that to server
            var command = api.CloneJson();
            command.Result = "";
            OnSendCommand?.Invoke(new SendCommandEventArgs {APIObject = command});
        }
        /*
         *
         */
        public delegate void ErrorEventHandler(ErrorEventArgs e);
        public static event ErrorEventHandler OnError;

        public static void AttachErrorListener(IErrorListener listener)
        {
            OnError += listener.ErrorMessage;
        }

        public static void DetachErrorListener(IErrorListener listener)
        {
            OnError -= listener.ErrorMessage;
        }

        public static void SendErrorMessage(string message, Exception ex)
        {
            OnError?.Invoke(new ErrorEventArgs { Message = message, Ex = ex});
        }
        /*
         *
         */
        public delegate void ConnectionStatusEventHandler(ConnectionEventArgs e);
        public static event ConnectionStatusEventHandler OnConnection;

        public static void AttachConnectionListener(IConnectionListener listener)
        {
            OnConnection += listener.ConnectionStatus;
        }

        public static void DetachConnectionListener(IConnectionListener listener)
        {
            OnConnection -= listener.ConnectionStatus;
        }

        public static void SendConnectionStatus(bool isConnected)
        {
            OnConnection?.Invoke(new ConnectionEventArgs { IsConnected = isConnected});
        }
        /*
         *
         */
        public delegate void DataEventHandler(DataEventArgs e);
        public static event DataEventHandler OnData;

        public static void AttachDataListener(IDataListener listener)
        {
            OnData += listener.DataReceived;
        }

        public static void DetachDataListener(IDataListener listener)
        {
            OnData -= listener.DataReceived;
        }

        public static void SendData(List<DCSAPI> dcsAPIs)
        {
            OnData?.Invoke(new DataEventArgs { DCSAPIS = dcsAPIs });
        }
        public static void SendData(DCSAPI dcsAPI)
        {
            OnData?.Invoke(new DataEventArgs { DCSApi = dcsAPI });
        }
    }
}
