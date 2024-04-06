using DCSInsight.Events;
using DCSInsight.Misc;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace DCSInsight.UserControls
{
    /// <summary>
    /// Interaction logic for UserControlPulseLED.xaml
    /// </summary>
    public partial class UserControlPulseLED : UserControl, IDisposable, IAsyncDisposable
    {
        private Timer? _timerLoopPulse;

        public UserControlPulseLED()
        {
            InitializeComponent();

            _timerLoopPulse = new Timer(PulseTimerCallback);
            _timerLoopPulse.Change(Timeout.Infinite, Timeout.Infinite);
        }
        
        public void Dispose()
        {
            _timerLoopPulse?.Dispose();
            _timerLoopPulse = null;
            GC.SuppressFinalize(this);
        }

        public async ValueTask DisposeAsync()
        {
            if (_timerLoopPulse != null)
            {
                await _timerLoopPulse.DisposeAsync();
                _timerLoopPulse = null;
                GC.SuppressFinalize(this);
            }
        }

        private void SetPulseImage(bool setOn)
        {
            if (!setOn)
            {
                ImagePulse.Source = new BitmapImage(new Uri("/dcs-insight;component/Images/Icon_green_lamp_off.png", UriKind.Relative));
                return;
            }
            ImagePulse.Source = new BitmapImage(new Uri("/dcs-insight;component/Images/Icon_green_lamp_on.png", UriKind.Relative));
        }

        public void Pulse(int milliseconds = 300)
        {
            try
            {
                Dispatcher?.BeginInvoke((Action)(() => SetPulseImage(true)));

                _timerLoopPulse?.Change(milliseconds, milliseconds);
                //Dispatcher?.BeginInvoke((Action)(SetFormState));
            }
            catch (Exception ex)
            {
                Common.ShowErrorMessageBox(ex);
            }
        }

        private void PulseTimerCallback(object? state)
        {
            try
            {
                Dispatcher?.BeginInvoke((Action)(() => SetPulseImage(false)));
                //Dispatcher?.BeginInvoke((Action)(() => ToolBarMain.UpdateLayout()));
                _timerLoopPulse?.Change(Timeout.Infinite, Timeout.Infinite);
                //Dispatcher?.BeginInvoke((Action)(SetFormState));
            }
            catch (Exception ex)
            {
                ICEventHandler.SendErrorMessage("Timer Polling Error", ex);
            }
        }
    }
}
