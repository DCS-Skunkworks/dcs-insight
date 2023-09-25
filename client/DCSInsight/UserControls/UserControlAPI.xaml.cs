using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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
    /// Interaction logic for UserControlAPI.xaml
    /// </summary>
    public partial class UserControlAPI : UserControl, IDisposable, IAsyncDisposable
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private readonly DCSAPI _dcsAPI;
        private bool _isLoaded;
        private readonly List<TextBox> _textBoxParameterList = new();
        private bool _isConnected;
        private Timer _pollingTimer;
        private bool _canSend;
        private bool _keepResults;

        public int Id { get; private set; }

        public UserControlAPI(DCSAPI dcsAPI, bool isConnected)
        {
            InitializeComponent();
            _dcsAPI = dcsAPI;
            Id = _dcsAPI.Id;
            _isConnected = isConnected;
            _pollingTimer = new Timer(PollingTimerCallback);
            _pollingTimer.Change(Timeout.Infinite, 10000);
        }

        public void Dispose()
        {
            _pollingTimer?.Dispose();
        }

        private void UserControlAPI_OnLoaded(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_isLoaded) return;

                ComboBoxPollTimes.Items.Clear();
                ComboBoxPollTimes.Items.Add(500);
                ComboBoxPollTimes.Items.Add(1000);
                ComboBoxPollTimes.Items.Add(2000);
                ComboBoxPollTimes.SelectedIndex = 0;
                BuildUI();
                _isLoaded = true;
            }
            catch (Exception ex)
            {
                Common.ShowErrorMessageBox(ex);
            }
        }

        private void SetFormState()
        {
            try
            {
                ButtonSend.IsEnabled = !_textBoxParameterList.Any(o => string.IsNullOrEmpty(o.Text)) && _isConnected;
                CheckBoxPolling.IsEnabled = ButtonSend.IsEnabled;
                ComboBoxPollTimes.IsEnabled = CheckBoxPolling.IsChecked == false;
                _canSend = ButtonSend.IsEnabled;
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
                _isConnected = connected;
                SetFormState();
            }
            catch (Exception ex)
            {
                Common.ShowErrorMessageBox(ex);
            }
        }

        private void BuildUI()
        {
            try
            {
                TextBoxSyntax.Text = _dcsAPI.Syntax;
                TextBoxSyntax.ToolTip = $"API Id = {_dcsAPI.Id}";
                StackPanelParametersDev.Children.Clear();
                StackPanelParametersDev.Visibility = Visibility.Collapsed;
                StackPanelParameters.Visibility = Visibility.Visible;

                foreach (var dcsAPIParameterType in _dcsAPI.Parameters)
                {
                    var label = new Label
                    {
                        Content = dcsAPIParameterType.ParameterName.Replace("_", "__")
                    };
                    label.VerticalAlignment = VerticalAlignment.Center;
                    StackPanelParameters.Children.Add(label);
                    StackPanelParameters.UpdateLayout();

                    var textBox = new TextBox
                    {
                        Name = "TextBox" + dcsAPIParameterType.Id,
                        Tag = dcsAPIParameterType.Id,
                        Width = 50,
                        Height = 20
                    };
                    if (dcsAPIParameterType.Type == ParameterTypeEnum.number)
                    {
                        textBox.KeyDown += TextBoxParameter_OnKeyDown_Number;
                    }
                    textBox.KeyUp += TextBoxParameter_OnKeyUp;
                    _textBoxParameterList.Add(textBox);
                    StackPanelParameters.Children.Add(textBox);
                    StackPanelParameters.UpdateLayout();
                }


                //This is ugly
                StackPanelParameters.Children.Remove(ButtonSend);
                StackPanelParameters.Children.Remove(LabelKeepResult);
                StackPanelParameters.Children.Remove(CheckBoxKeepResults);
                StackPanelParameters.Children.Remove(LabelPollingCheckBox);
                StackPanelParameters.Children.Remove(CheckBoxPolling);
                StackPanelParameters.Children.Remove(LabelInterval);
                StackPanelParameters.Children.Remove(ComboBoxPollTimes);

                StackPanelParameters.Children.Add(ButtonSend);
                StackPanelParameters.Children.Add(LabelKeepResult);
                StackPanelParameters.Children.Add(CheckBoxKeepResults);
                StackPanelParameters.Children.Add(LabelPollingCheckBox);
                StackPanelParameters.Children.Add(CheckBoxPolling);
                StackPanelParameters.Children.Add(LabelInterval);
                StackPanelParameters.Children.Add(ComboBoxPollTimes);

                //No polling for procedures
                LabelPollingCheckBox.Visibility = _dcsAPI.ReturnsData ? Visibility.Visible : Visibility.Collapsed;
                LabelKeepResult.Visibility = _dcsAPI.ReturnsData ? Visibility.Visible : Visibility.Collapsed;
                CheckBoxKeepResults.Visibility = _dcsAPI.ReturnsData ? Visibility.Visible : Visibility.Collapsed;
                LabelInterval.Visibility = _dcsAPI.ReturnsData ? Visibility.Visible : Visibility.Collapsed;
                CheckBoxPolling.Visibility = _dcsAPI.ReturnsData ? Visibility.Visible : Visibility.Collapsed;
                ComboBoxPollTimes.Visibility = _dcsAPI.ReturnsData ? Visibility.Visible : Visibility.Collapsed;
                StackPanelParameters.UpdateLayout();

                SetFormState();
            }
            catch (Exception ex)
            {
                Common.ShowErrorMessageBox(ex);
            }
        }

        public void SetResult(DCSAPI dcsApi)
        {
            try
            {
                if (_keepResults)
                {
                    TextBoxResult.Text = TextBoxResult.Text.Insert(0, "\n-----------\n");
                    TextBoxResult.Text = TextBoxResult.Text.Insert(0, dcsApi.Result);
                    return;
                }
                TextBoxResult.Text = dcsApi.Result;
            }
            catch (Exception ex)
            {
                Common.ShowErrorMessageBox(ex);
            }
        }

        private void SendCommand()
        {
            try
            {
                foreach (var textBox in _textBoxParameterList)
                {
                    var parameterId = (int)textBox.Tag;
                    foreach (var parameter in _dcsAPI.Parameters)
                    {
                        if (parameter.Id == parameterId)
                        {
                            parameter.Value = textBox.Text;
                        }
                    }
                }

                ICEventHandler.SendCommand(this, _dcsAPI);
                SetFormState();
            }
            catch (Exception ex)
            {
                Common.ShowErrorMessageBox(ex);
            }
        }

        private void ButtonSend_OnClick(object sender, RoutedEventArgs e)
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

        private void PollingTimerCallback(object state)
        {
            try
            {
                if (_canSend)
                {
                    Dispatcher?.BeginInvoke((Action)(SendCommand));
                }
            }
            catch (Exception ex)
            {
                ICEventHandler.SendErrorMessage(this, "Timer Polling Error", ex);
            }
        }

        private void StartPolling(int milliseconds)
        {
            try
            {
                _pollingTimer.Change(milliseconds, milliseconds);
                SetFormState();
            }
            catch (Exception ex)
            {
                Common.ShowErrorMessageBox(ex);
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
                StartPolling(int.Parse(ComboBoxPollTimes.SelectedValue.ToString()));
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
                if (e.Key is not (>= Key.D0 and <= Key.D9 or >= Key.NumPad0 and <= Key.NumPad9 or Key.OemPeriod) && e.Key != Key.OemMinus && e.Key != Key.OemPlus
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

        private void TextBoxParameter_OnKeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Key == Key.Enter && _canSend)
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

        public async ValueTask DisposeAsync()
        {
            if (_pollingTimer != null) await _pollingTimer.DisposeAsync();
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
    }
}
