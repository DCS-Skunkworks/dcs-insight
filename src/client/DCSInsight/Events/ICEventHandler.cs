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
        public static event SendCommandEventHandler? OnSendCommand;
        
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
            var command = api.CloneJson() ?? throw new Exception("Failed to clone DCSAPI");

            command.Result = "";
            OnSendCommand?.Invoke(new SendCommandEventArgs(command));
        }
        /*
         *
         */
        public delegate void ErrorEventHandler(ErrorEventArgs e);
        public static event ErrorEventHandler? OnError;

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
            OnError?.Invoke(new ErrorEventArgs (message, ex));
        }
        /*
         *
         */
        public delegate void ConnectionStatusEventHandler(ConnectionEventArgs e);
        public static event ConnectionStatusEventHandler? OnConnection;

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
         * For handling API list
         */
        public delegate void APIDataEventHandler(APIDataEventArgs e);
        public static event APIDataEventHandler? OnAPIData;

        public static void AttachAPIDataListener(IAPIDataListener listener)
        {
            OnAPIData += listener.APIDataReceived;
        }

        public static void DetachAPIDataListener(IAPIDataListener listener)
        {
            OnAPIData -= listener.APIDataReceived;
        }

        public static void SendAPIData(List<DCSAPI> dcsAPIs)
        {
            OnAPIData?.Invoke(new APIDataEventArgs(dcsAPIs));
        }
        /*
         * For handling back and forth command data
         */
        public delegate void CommandDataEventHandler(CommandDataEventArgs e);
        public static event CommandDataEventHandler? OnCommandData;

        public static void AttachCommandDataListener(ICommandDataListener listener)
        {
            OnCommandData += listener.CommandDataReceived;
        }

        public static void DetachCommandDataListener(ICommandDataListener listener)
        {
            OnCommandData -= listener.CommandDataReceived;
        }

        public static void SendCommandData(DCSAPI dcsAPI)
        {
            OnCommandData?.Invoke(new CommandDataEventArgs(dcsAPI));
        }
        /*
         *
         */
        public delegate void CommsErrorEventHandler(CommsErrorEventArgs e);
        public static event CommsErrorEventHandler? OnCommsError;

        public static void AttachCommsErrorListener(ICommsErrorListener listener)
        {
            OnCommsError += listener.CommsErrorMessage;
        }

        public static void DetachCommsErrorListener(ICommsErrorListener listener)
        {
            OnCommsError -= listener.CommsErrorMessage;
        }

        public static void SendCommsErrorMessage(string shortMessage, Exception ex)
        {
            OnCommsError?.Invoke(new CommsErrorEventArgs(shortMessage, ex));
        }
    }
}
