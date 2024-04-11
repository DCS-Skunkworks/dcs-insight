using DCSInsight.Events;

namespace DCSInsight.Interfaces
{
    public interface ICommandListener
    {
        void SendCommand(SendCommandEventArgs args);
    }

    public interface IErrorListener
    {
        void ErrorMessage(ErrorEventArgs args);
    }

    public interface ICommsErrorListener
    {
        void CommsErrorMessage(CommsErrorEventArgs args);
    }

    public interface IConnectionListener
    {
        void ConnectionStatus(ConnectionEventArgs args);
    }

    public interface IAPIDataListener
    {
        void APIDataReceived(APIDataEventArgs args);
    }

    public interface ICommandDataListener
    {
        void CommandDataReceived(CommandDataEventArgs args);
    }
}
