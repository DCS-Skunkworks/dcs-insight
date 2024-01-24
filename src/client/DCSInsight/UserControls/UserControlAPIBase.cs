using System;
using System.Collections.Generic;
using System.Media;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using DCSInsight.Events;
using DCSInsight.JSON;
using DCSInsight.Misc;
using NLog;

namespace DCSInsight.UserControls
{
    /// <summary>
    /// Interaction logic for UserControlAPIBase.xaml
    /// </summary>
    public abstract partial class UserControlAPIBase : UserControl, IDisposable, IAsyncDisposable
    {
        protected static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        protected readonly DCSAPI DCSAPI;
        protected bool IsControlLoaded;
        protected readonly List<TextBox> TextBoxParameterList = new();
        protected bool IsConnected;
        protected readonly Timer PollingTimer;
        protected bool CanSend;
        protected bool KeepResults;
        protected Button ButtonSend;
        protected Label LabelKeepResults;
        protected CheckBox CheckBoxKeepResults;
        protected Label LabelPolling;
        protected CheckBox CheckBoxPolling;
        protected Label LabelPollingInterval;
        protected ComboBox ComboBoxPollTimes;
        protected static readonly AutoResetEvent AutoResetEventPolling = new(false);
        protected const string LuaConsole = "LuaConsole";

        public int Id { get; protected set; }
        protected abstract void BuildUI();
        public abstract void SetResult(DCSAPI dcsApi);
        protected abstract void SendCommand();
        protected abstract void SetFormState();

        protected UserControlAPIBase(DCSAPI dcsAPI, bool isConnected)
        {
            DCSAPI = dcsAPI;
            Id = DCSAPI.Id;
            IsConnected = isConnected;
            PollingTimer = new Timer(PollingTimerCallback);
            PollingTimer.Change(Timeout.Infinite, 10000);
        }

        public void Dispose()
        {
            AutoResetEventPolling.Set();
            AutoResetEventPolling.Set();
            PollingTimer?.Dispose();
            AutoResetEventPolling.Dispose();
            GC.SuppressFinalize(this);
        }

        public async ValueTask DisposeAsync()
        {
            if (PollingTimer != null)
            {
                AutoResetEventPolling.Set();
                AutoResetEventPolling.Set();
                await PollingTimer.DisposeAsync();
                AutoResetEventPolling.Dispose();
                GC.SuppressFinalize(this);
            }
        }
        
        public void SetConnectionStatus(bool connected)
        {
            try
            {
                IsConnected = connected;
                if (!IsConnected)
                {
                    PollingTimer.Change(Timeout.Infinite, 10000);
                }
                SetFormState();
            }
            catch (Exception ex)
            {
                Common.ShowErrorMessageBox(ex);
            }
        }

        protected void ButtonSend_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                SendCommand();
            }
            catch (Exception ex)
            {
                Common.ShowErrorMessageBox(ex);
            }
        }

        private void StartPolling(int milliseconds)
        {
            try
            {
                PollingTimer.Change(milliseconds, milliseconds);
                AutoResetEventPolling.Set();
                SetFormState();
            }
            catch (Exception ex)
            {
                Common.ShowErrorMessageBox(ex);
            }
        }

        private void StopPolling()
        {
            try
            {
                PollingTimer.Change(Timeout.Infinite, Timeout.Infinite);
                SetFormState();
            }
            catch (Exception ex)
            {
                Common.ShowErrorMessageBox(ex);
            }
        }

        protected void PollingTimerCallback(object state)
        {
            try
            {
                AutoResetEventPolling.WaitOne();
                if (CanSend)
                {
                    Dispatcher?.BeginInvoke((Action)(SendCommand));
                }
            }
            catch (Exception ex)
            {
                ICEventHandler.SendErrorMessage( "Timer Polling Error", ex);
            }
        }
        
        protected void CheckBoxPolling_OnUnchecked(object sender, RoutedEventArgs e)
        {
            try
            {
                StopPolling();
                SetFormState();
            }
            catch (Exception ex)
            {
                Common.ShowErrorMessageBox(ex);
            }
        }

        protected void CheckBoxPolling_OnChecked(object sender, RoutedEventArgs e)
        {
            try
            {
                StartPolling(int.Parse(ComboBoxPollTimes.SelectedValue.ToString()));
                SetFormState();
            }
            catch (Exception ex)
            {
                Common.ShowErrorMessageBox(ex);
            }
        }

        protected void ComboBoxPollTimes_OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            try
            {
                SetFormState();
            }
            catch (Exception ex)
            {
                Common.ShowErrorMessageBox(ex);
            }
        }

        protected void TextBoxParameter_OnKeyDown_Number(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Key is not (>= Key.D0 and <= Key.D9 or >= Key.NumPad0 and <= Key.NumPad9 or Key.OemPeriod or Key.Tab) && e.Key != Key.OemMinus && e.Key != Key.OemPlus
                    && e.Key != Key.Add && e.Key != Key.Subtract)
                {
                    e.Handled = true;
                    return;
                }
                SetFormState();
            }
            catch (Exception ex)
            {
                Common.ShowErrorMessageBox(ex);
            }
        }

        protected void TextBoxParameter_OnKeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Key == Key.Enter && CanSend)
                {
                    SendCommand();
                }
                SetFormState();
            }
            catch (Exception ex)
            {
                Common.ShowErrorMessageBox(ex);
            }
        }

        protected void CheckBoxKeepResults_OnUnchecked(object sender, RoutedEventArgs e)
        {
            try
            {
                KeepResults = false;
                SetFormState();
            }
            catch (Exception ex)
            {
                Common.ShowErrorMessageBox(ex);
            }
        }

        protected void CheckBoxKeepResults_OnChecked(object sender, RoutedEventArgs e)
        {
            try
            {
                KeepResults = true;
                SetFormState();
            }
            catch (Exception ex)
            {
                Common.ShowErrorMessageBox(ex);
            }
        }

        protected void TextBoxSyntax_OnMouseEnter(object sender, MouseEventArgs e)
        {
            try
            {
                Mouse.OverrideCursor = Cursors.Hand;
            }
            catch (Exception ex)
            {
                Common.ShowErrorMessageBox(ex);
            }
        }

        protected void TextBoxSyntax_OnMouseLeave(object sender, MouseEventArgs e)
        {
            try
            {
                Mouse.OverrideCursor = Cursors.Arrow;
            }
            catch (Exception ex)
            {
                Common.ShowErrorMessageBox(ex);
            }
        }

        protected void TextBoxSyntax_OnPreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                var textBox = (TextBox)sender;
                Clipboard.SetText(textBox.Text);
                SystemSounds.Asterisk.Play();
            }
            catch (Exception ex)
            {
                Common.ShowErrorMessageBox(ex);
            }
        }
    }
}
