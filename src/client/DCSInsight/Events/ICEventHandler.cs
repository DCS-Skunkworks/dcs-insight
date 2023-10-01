using System;
using DCSInsight.Interfaces;
using DCSInsight.JSON;

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
            OnSendCommand?.Invoke(new SendCommandEventArgs {APIObject = api});
        }


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
    }
}
