using System;
using System.Collections.Generic;
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
        private readonly Timer _pollingTimer;
        private bool _canSend;
        private bool _keepResults;
        private Button _buttonSend;
        private Label _labelKeepResults;
        private CheckBox _checkBoxKeepResults;
        private Label _labelPolling;
        private CheckBox _checkBoxPolling;
        private Label _labelPollingInterval;
        private ComboBox _comboBoxPollTimes;

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
            GC.SuppressFinalize(this);
        }

        public async ValueTask DisposeAsync()
        {
            if (_pollingTimer != null)
            {
                await _pollingTimer.DisposeAsync();
                GC.SuppressFinalize(this);
            }
        }

        private void UserControlAPI_OnLoaded(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_isLoaded) return;

                IsTabStop = true;

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
                _buttonSend.IsEnabled = !_textBoxParameterList.Any(o => string.IsNullOrEmpty(o.Text)) && _isConnected;

                if (_dcsAPI.ReturnsData)
                {
                    _checkBoxPolling.IsEnabled = _buttonSend.IsEnabled;
                    _comboBoxPollTimes.IsEnabled = _checkBoxPolling.IsChecked == false;
                }
                _canSend = _buttonSend.IsEnabled;
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
                Mouse.OverrideCursor = Cursors.Wait;

                try
                {
                    TextBoxSyntax.Text = _dcsAPI.Syntax;
                    TextBoxSyntax.ToolTip = $"API Id = {_dcsAPI.Id}";

                    var controlList = new List<Control>();

                    foreach (var dcsAPIParameterType in _dcsAPI.Parameters)
                    {
                        var label = new Label
                        {
                            Content = dcsAPIParameterType.ParameterName.Replace("_", "__"),
                            VerticalAlignment = VerticalAlignment.Center
                        };
                        controlList.Add(label);

                        var textBox = new TextBox
                        {
                            Name = "TextBox" + dcsAPIParameterType.Id,
                            Tag = dcsAPIParameterType.Id,
                            MinWidth = 50,
                            MaxWidth = 100,
                            Height = 20,
                            IsTabStop = true
                        };

                        if (dcsAPIParameterType.Type == ParameterTypeEnum.number)
                        {
                            textBox.KeyDown += TextBoxParameter_OnKeyDown_Number;
                        }
                        textBox.KeyUp += TextBoxParameter_OnKeyUp;

                        controlList.Add(textBox);
                        _textBoxParameterList.Add(textBox);
                    }

                    _buttonSend = new Button
                    {
                        Content = "Send",
                        Height = 20,
                        Width = 50,
                        VerticalAlignment = VerticalAlignment.Center,
                        Margin = new Thickness(20, 0, 0, 0)
                    };
                    _buttonSend.Click += ButtonSend_OnClick;
                    controlList.Add(_buttonSend);

                    if (_dcsAPI.ReturnsData)
                    {
                        _labelKeepResults = new Label
                        {
                            Content = "Keep results",
                            VerticalAlignment = VerticalAlignment.Center,
                            Margin = new Thickness(10, 0, 0, 0)
                        };
                        controlList.Add(_labelKeepResults);

                        _checkBoxKeepResults = new CheckBox
                        {
                            Margin = new Thickness(0, 0, 0, 0),
                            VerticalAlignment = VerticalAlignment.Center
                        };
                        _checkBoxKeepResults.Checked += CheckBoxKeepResults_OnChecked;
                        _checkBoxKeepResults.Unchecked += CheckBoxKeepResults_OnUnchecked;
                        controlList.Add(_checkBoxKeepResults);

                        _labelPolling = new Label
                        {
                            Content = "Polling",
                            VerticalAlignment = VerticalAlignment.Center,
                            Margin = new Thickness(10, 0, 0, 0)
                        };
                        controlList.Add(_labelPolling);

                        _checkBoxPolling = new CheckBox
                        {
                            Margin = new Thickness(0, 0, 0, 0),
                            VerticalAlignment = VerticalAlignment.Center

                        };
                        _checkBoxPolling.Checked += CheckBoxPolling_OnChecked;
                        _checkBoxPolling.Unchecked += CheckBoxPolling_OnUnchecked;
                        controlList.Add(_checkBoxPolling);

                        _labelPollingInterval = new Label
                        {
                            Content = "Interval (ms) :",
                            VerticalAlignment = VerticalAlignment.Center,
                            Margin = new Thickness(10, 0, 0, 0)
                        };
                        controlList.Add(_labelPollingInterval);

                        _comboBoxPollTimes = new ComboBox
                        {
                            Height = 20,
                            Margin = new Thickness(2, 0, 0, 0),
                            VerticalAlignment = VerticalAlignment.Center
                        };
                        _comboBoxPollTimes.DataContextChanged += ComboBoxPollTimes_OnDataContextChanged;
                        _comboBoxPollTimes.Items.Add(500);
                        _comboBoxPollTimes.Items.Add(1000);
                        _comboBoxPollTimes.Items.Add(2000);
                        _comboBoxPollTimes.SelectedIndex = 0;
                        controlList.Add(_comboBoxPollTimes);
                    }

                    ItemsControlParameters.ItemsSource = controlList;
                    SetFormState();
                }
                catch (Exception ex)
                {
                    Common.ShowErrorMessageBox(ex);
                }

            }
            finally
            {
                Mouse.OverrideCursor = Cursors.Arrow;
            }
        }

        public void SetResult(DCSAPI dcsApi)
        {
            try
            {
                if (_keepResults)
                {
                    Dispatcher?.BeginInvoke((Action)(() => TextBoxResult.Text = TextBoxResult.Text.Insert(0, "\n-----------\n")));
                    Dispatcher?.BeginInvoke((Action)(() => TextBoxResult.Text = TextBoxResult.Text.Insert(0, dcsApi.Result)));
                    return;
                }
                Dispatcher?.BeginInvoke((Action)(() => TextBoxResult.Text = dcsApi.Result));
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

                ICEventHandler.SendCommand(_dcsAPI);
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
                ICEventHandler.SendErrorMessage( "Timer Polling Error", ex);
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
                StartPolling(int.Parse(_comboBoxPollTimes.SelectedValue.ToString()));
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
