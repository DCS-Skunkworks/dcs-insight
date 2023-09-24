using DCSInsight.Events;

namespace DCSInsight.Interfaces
{
    public interface ICommandListener
    {
        void SendCommand(object sender, SendCommandEventArgs args);
    }

    public interface IErrorListener
    {
        void ErrorMessage(object sender, ErrorEventArgs args);
    }
}
