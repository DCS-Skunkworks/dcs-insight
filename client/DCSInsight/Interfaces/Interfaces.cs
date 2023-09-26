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
}
