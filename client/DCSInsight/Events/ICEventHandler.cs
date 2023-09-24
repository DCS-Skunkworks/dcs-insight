using System;
using DCSInsight.Interfaces;
using DCSInsight.JSON;

namespace DCSInsight.Events
{
    internal static class ICEventHandler
    {
        public delegate void SendCommandEventHandler(object sender, SendCommandEventArgs e);
        public static event SendCommandEventHandler OnSendCommand;
        
        public static void AttachCommandListener(ICommandListener listener)
        {
            OnSendCommand += listener.SendCommand;
        }

        public static void DetachCommandListener(ICommandListener listener)
        {
            OnSendCommand -= listener.SendCommand;
        }

        public static void SendCommand(object sender, DCSAPI api)
        {
            OnSendCommand?.Invoke(sender, new SendCommandEventArgs {Sender  = sender, APIObject = api});
        }


        public delegate void ErrorEventHandler(object sender, ErrorEventArgs e);
        public static event ErrorEventHandler OnError;

        public static void AttachErrorListener(IErrorListener listener)
        {
            OnError += listener.ErrorMessage;
        }

        public static void DetachErrorListener(IErrorListener listener)
        {
            OnError -= listener.ErrorMessage;
        }

        public static void SendErrorMessage(object sender, string message, Exception ex)
        {
            OnError?.Invoke(sender, new ErrorEventArgs { Sender = sender, Message = message, Ex = ex});
        }
    }
}
