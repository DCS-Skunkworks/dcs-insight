using System;
using System.Collections.Generic;
using System.Media;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
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
        private readonly Timer _pollingTimer;
        protected bool CanSend;
        private bool _keepResults;
        protected Button? ButtonSend;
        protected CheckBox? CheckBoxPolling;
        protected ComboBox? ComboBoxPollTimes;
        protected Label? LabelResultBase;
        protected TextBox? TextBoxResultBase;
        private static readonly AutoResetEvent AutoResetEventPolling = new(false);
        protected readonly bool IsLuaConsole;

        public int Id { get; protected set; }
        protected abstract void BuildUI();
        protected abstract void SetFormState();

        protected UserControlAPIBase(DCSAPI dcsAPI, bool isConnected)
        {
            DCSAPI = dcsAPI;
            IsLuaConsole = DCSAPI.Id == Constants.LuaConsole;
            Id = DCSAPI.Id;
            IsConnected = isConnected;
            _pollingTimer = new Timer(PollingTimerCallback);
            _pollingTimer.Change(Timeout.Infinite, 10000);
        }

        public void Dispose()
        {
            AutoResetEventPolling.Set();
            AutoResetEventPolling.Set();
            _pollingTimer?.Dispose();
            AutoResetEventPolling.Dispose();
            GC.SuppressFinalize(this);
        }

        public async ValueTask DisposeAsync()
        {
            AutoResetEventPolling.Set();
            AutoResetEventPolling.Set();
            await _pollingTimer.DisposeAsync();
            AutoResetEventPolling.Dispose();
            GC.SuppressFinalize(this);
        }

        protected void SendCommand()
        {
            try
            {
                foreach (var textBox in TextBoxParameterList)
                {
                    var parameterId = (int)textBox.Tag;
                    foreach (var parameter in DCSAPI.Parameters)
                    {
                        if (parameter.Id == parameterId)
                        {
                            parameter.Value = textBox.Text;
                        }
                    }
                }

                ICEventHandler.SendCommand(DCSAPI);
                SetFormState();
            }
            catch (Exception ex)
            {
                Common.ShowErrorMessageBox(ex);
            }
        }

        private string ResultTextBoxFirstLine()
        {
            var textBoxResultText = "";
            Application.Current.Dispatcher.Invoke(
                DispatcherPriority.Normal,
                (ThreadStart)delegate { textBoxResultText = TextBoxResultBase?.Text ?? ""; });

            if (string.IsNullOrEmpty(textBoxResultText)) return "";

            return textBoxResultText.Contains('\n', StringComparison.Ordinal) ? textBoxResultText : textBoxResultText[..textBoxResultText.IndexOf("\n", StringComparison.Ordinal)];
        }

        internal void SetResult(DCSAPI dcsApi)
        {
            try
            {
                if (LabelResultBase == null || TextBoxResultBase == null) return;

                Dispatcher?.BeginInvoke((Action)(() => LabelResultBase.Content = $"Result ({dcsApi.ResultType})"));

                var result = dcsApi.ErrorThrown ? dcsApi.ErrorMessage ?? "nil" : string.IsNullOrEmpty(dcsApi.Result) ? "nil" : dcsApi.Result;

                AutoResetEventPolling.Set();


                if (result == ResultTextBoxFirstLine() && result == DCSAPI.Result && !IsLuaConsole)
                {
                    return;
                }

                DCSAPI.Result = result;

                if (_keepResults)
                {
                    Dispatcher?.BeginInvoke((Action)(() => TextBoxResultBase.Text = TextBoxResultBase.Text.Insert(0, result + "\n")));
                    return;
                }
                Dispatcher?.BeginInvoke((Action)(() => TextBoxResultBase.Text = result));
            }
            catch (Exception ex)
            {
                Common.ShowErrorMessageBox(ex);
            }
        }


        public void SetConnectionStatus(bool connected)
        {
            try
            {
                IsConnected = connected;
                if (!IsConnected)
                {
                    _pollingTimer.Change(Timeout.Infinite, 10000);
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
                _pollingTimer.Change(milliseconds, milliseconds);
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
                _pollingTimer.Change(Timeout.Infinite, Timeout.Infinite);
                SetFormState();
            }
            catch (Exception ex)
            {
                Common.ShowErrorMessageBox(ex);
            }
        }

        private void PollingTimerCallback(object? state)
        {
            try
            {
                AutoResetEventPolling.WaitOne();
                if (CanSend)
                {
                    Dispatcher?.BeginInvoke((Action)SendCommand);
                }
            }
            catch (Exception ex)
            {
                ICEventHandler.SendErrorMessage("Timer Polling Error", ex);
            }
        }

        private void CheckBoxPolling_OnUnchecked(object sender, RoutedEventArgs e)
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

        private void CheckBoxPolling_OnChecked(object sender, RoutedEventArgs e)
        {
            try
            {
                var comboBoxPollTimes = (ComboBox)sender;
                StartPolling(int.Parse(comboBoxPollTimes.SelectedValue.ToString() ?? "500"));
                SetFormState();
            }
            catch (Exception ex)
            {
                Common.ShowErrorMessageBox(ex);
            }
        }

        private void ComboBoxPollTimes_OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
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

        private void TextBoxParameter_OnKeyDown_Number(object sender, KeyEventArgs e)
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

        private void CheckBoxKeepResults_OnUnchecked(object sender, RoutedEventArgs e)
        {
            try
            {
                _keepResults = false;
                SetFormState();
            }
            catch (Exception ex)
            {
                Common.ShowErrorMessageBox(ex);
            }
        }

        private void CheckBoxKeepResults_OnChecked(object sender, RoutedEventArgs e)
        {
            try
            {
                _keepResults = true;
                SetFormState();
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

        protected static Label GetLabelParameterName(string parameterName)
        {
            return new Label
            {
                Content = parameterName.Replace("_", "__"),
                VerticalAlignment = VerticalAlignment.Center
            };
        }
        protected TextBox GetTextBoxParameter(ParameterInfo parameterInfo)
        {
            var textBox = new TextBox
            {
                Name = "TextBox" + parameterInfo.Id,
                Tag = parameterInfo.Id,
                MinWidth = 50,
                Height = 20,
                IsTabStop = true
            };


            if (parameterInfo.Type == ParameterTypeEnum.number)
            {
                textBox.KeyDown += TextBoxParameter_OnKeyDown_Number;
            }
            textBox.KeyUp += TextBoxParameter_OnKeyUp;

            return textBox;
        }

        protected Button GetButtonSend()
        {
            ButtonSend = new Button
            {
                Content = "Send",
                Height = 20,
                Width = 50,
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Thickness(20, 0, 0, 0)
            };
            ButtonSend.Click += ButtonSend_OnClick;

            return ButtonSend;
        }

        protected static Label GetLabelKeepResults()
        {
            return new Label
            {
                Content = "Keep results",
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Thickness(10, 0, 0, 0)
            };
        }

        protected CheckBox GetCheckBoxKeepResults()
        {
            var checkBoxKeepResults = new CheckBox
            {
                Margin = new Thickness(0, 0, 0, 0),
                VerticalAlignment = VerticalAlignment.Center
            };
            checkBoxKeepResults.Checked += CheckBoxKeepResults_OnChecked;
            checkBoxKeepResults.Unchecked += CheckBoxKeepResults_OnUnchecked;
            return checkBoxKeepResults;
        }

        protected static Label GetLabelPolling()
        {
            return new Label
            {
                Content = "Poll",
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Thickness(10, 0, 0, 0)
            };
        }

        protected CheckBox GetCheckBoxPolling()
        {
            CheckBoxPolling = new CheckBox
            {
                Margin = new Thickness(0, 0, 0, 0),
                VerticalAlignment = VerticalAlignment.Center

            };
            CheckBoxPolling.Checked += CheckBoxPolling_OnChecked;
            CheckBoxPolling.Unchecked += CheckBoxPolling_OnUnchecked;
            return CheckBoxPolling;
        }

        protected static Label GetLabelPollingInterval()
        {
            return new Label
            {
                Content = "Interval (ms) :",
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Thickness(10, 0, 0, 0)
            };
        }

        protected ComboBox GetComboBoxPollTimes()
        {
            ComboBoxPollTimes = new ComboBox
            {
                Height = 20,
                Margin = new Thickness(2, 0, 0, 0),
                VerticalAlignment = VerticalAlignment.Center
            };
            ComboBoxPollTimes.DataContextChanged += ComboBoxPollTimes_OnDataContextChanged;
            ComboBoxPollTimes.Items.Add(100);
            ComboBoxPollTimes.Items.Add(500);
            ComboBoxPollTimes.Items.Add(1000);
            ComboBoxPollTimes.Items.Add(2000);
            ComboBoxPollTimes.SelectedIndex = 0;
            return ComboBoxPollTimes;
        }
    }
}
