using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using DCSInsight.Events;
using DCSInsight.Interfaces;
using DCSInsight.JSON;
using DCSInsight.Misc;
using NLog;

namespace DCSInsight.Windows
{
    /// <summary>
    /// Interaction logic for WindowRangeTest.xaml
    /// </summary>
    public partial class WindowRangeTest : Window, IErrorListener, IConnectionListener, IDataListener, IDisposable

    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private readonly List<DCSAPI> _dcsAPIList;
        private bool _formLoaded;
        private DCSAPI _dcsAPI;
        private readonly List<TextBoxParam> _textBoxParameterList = new();
        private bool _isConnected = true;
        private bool _isRunning;
        private bool _stopRunning;
        private bool _doLoop;
        private readonly int _longPulse = 10000000;
        private bool _showChangesOnly;
        private bool _showErrors;
        private bool _showNilResults;
        private static readonly AutoResetEvent AutoResetEvent1 = new(false);
        private Thread _thread;
        private readonly int _threadLoopSleep = 20;
        private readonly List<ResultComparator> _resultComparatorList = new();
        private readonly object _lockObject = new();

        public WindowRangeTest(List<DCSAPI> dcsAPIList)
        {
            InitializeComponent();
            _dcsAPIList = dcsAPIList;
            ICEventHandler.AttachErrorListener(this);
            ICEventHandler.AttachConnectionListener(this);
            ICEventHandler.AttachDataListener(this);
        }

        public void Dispose()
        {
            ICEventHandler.DetachErrorListener(this);
            ICEventHandler.DetachConnectionListener(this);
            ICEventHandler.DetachDataListener(this);
            _stopRunning = true;
            _doLoop = false;
            AutoResetEvent1.Set();
            AutoResetEvent1.Set();
            _thread?.Join(10000000);
            GC.SuppressFinalize(this);
        }

        private void WindowRangeTest_OnLoaded(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_formLoaded) return;

                CheckBoxShowErrors.IsChecked = true;
                CheckBoxShowNilResults.IsChecked = true;
                CheckBoxTop.IsChecked = true;
                PopulateAPIComboBox();
                SetFormState();
                _formLoaded = true;
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
                ButtonStart.IsEnabled = !_textBoxParameterList.Any(o => string.IsNullOrEmpty(o.Text)) && _isConnected && !_isRunning;
                TextBoxResults.Visibility = !string.IsNullOrEmpty(TextBoxResults.Text) ? Visibility.Visible : Visibility.Collapsed;
                ButtonStop.IsEnabled = _isRunning && !_stopRunning;
                CheckBoxLoop.IsEnabled = _isConnected;
                CheckBoxShowChangesOnly.IsEnabled = CheckBoxLoop.IsChecked == true;
            }
            catch (Exception ex)
            {
                Common.ShowErrorMessageBox(ex);
            }
        }

        public void ErrorMessage(ErrorEventArgs args)
        {
            try
            {
                if (!_isConnected || !_isRunning) return;

                Dispatcher?.BeginInvoke((Action)(() => AddResultText(args.Message)));
                AutoResetEvent1.Set();
            }
            catch (Exception ex)
            {
                Common.ShowErrorMessageBox(ex);
            }
        }

        public void ConnectionStatus(ConnectionEventArgs args)
        {
            try
            {
                _isConnected = args.IsConnected;
            }
            catch (Exception ex)
            {
                Common.ShowErrorMessageBox(ex);
            }
        }

        private void SetCurrentTestStructure(DCSAPI dcsApi)
        {
            lock (_lockObject)
            {
                if (_resultComparatorList.All(o => o.IsMatch(dcsApi) == false))
                {
                    _resultComparatorList.Add(new ResultComparator(dcsApi.CloneJson()));
                }
            }
        }

        public void DataReceived(DataEventArgs args)
        {
            try
            {
                lock (_lockObject)
                {
                    if (!_isConnected || !_isRunning) return;

                    if (args.DCSApi.ErrorThrown)
                    {
                        if(_showErrors) Dispatcher?.BeginInvoke((Action)(() => AddResultText(GetResultString(args.DCSApi))));
                        AutoResetEvent1.Set();
                        return;
                    }

                    if (string.IsNullOrEmpty(args.DCSApi.Result) && !_showNilResults)
                    {
                        AutoResetEvent1.Set();
                        return;
                    }

                    var hasChanged = _resultComparatorList.First(o => o.IsMatch(args.DCSApi)).SetResult(args.DCSApi);

                    if (_showChangesOnly && !hasChanged)
                    {
                        AutoResetEvent1.Set();
                    }
                    else
                    {
                        Dispatcher?.BeginInvoke((Action)(() => AddResultText(_resultComparatorList.First(o => o.IsMatch(args.DCSApi)).GetResultString())));
                        AutoResetEvent1.Set();
                    }
                }
            }
            catch (Exception ex)
            {
                Common.ShowErrorMessageBox(ex);
            }
        }

        private string GetResultString(DCSAPI dcsApi)
        {
            var currentTestString = new StringBuilder();

            foreach (var dcsApiParameter in dcsApi.Parameters)
            {
                currentTestString.Append($"{dcsApiParameter.ParameterName} [{dcsApiParameter.Value}], ");
            }

            currentTestString.Append($" result : {(dcsApi.ErrorThrown ? dcsApi.ErrorMessage : (string.IsNullOrEmpty(dcsApi.Result) ? "nil" : dcsApi.Result))}");

            return currentTestString.ToString();
        }

        private void StartTesting()
        {
            _stopRunning = false;

            try
            {
                lock (_lockObject)
                {
                    _resultComparatorList.Clear();
                }
                TextBoxResults.Text = "";
                var dynamicCount = _textBoxParameterList.Count(o => o.RangeLimit == RangeLimitsEnum.From);
                if (dynamicCount == 0) return;

                if (dynamicCount == 1)
                {
                    var i = Convert.ToInt32(_textBoxParameterList.First(o => o.RangeLimit == RangeLimitsEnum.From).Text);
                    var x = Convert.ToInt32(_textBoxParameterList.First(o => o.RangeLimit == RangeLimitsEnum.To).Text);
                    if (x < i) throw new Exception("End limit is less than start limit.");

                    _thread = new Thread(() => { Loop1(i, x); });
                    _thread.Start();
                }
                else if (dynamicCount == 2)
                {
                    var i = Convert.ToInt32(_textBoxParameterList.First(o => o.RangeLimit == RangeLimitsEnum.From).Text);
                    var x = Convert.ToInt32(_textBoxParameterList.First(o => o.RangeLimit == RangeLimitsEnum.To).Text);
                    if (x < i) throw new Exception($"End limit is less than start limit. {i}-{x}");
                    var j = Convert.ToInt32(_textBoxParameterList.Where(o => o.RangeLimit == RangeLimitsEnum.From).Skip(1).First().Text);
                    var y = Convert.ToInt32(_textBoxParameterList.Where(o => o.RangeLimit == RangeLimitsEnum.To).Skip(1).First().Text);
                    if (y < j) throw new Exception($"End limit is less than start limit. {j}-{y}");

                    _thread = new Thread(() => { Loop2(i, x, j, y); });
                    _thread.Start();
                }
                else if (dynamicCount == 3)
                {
                    var i = Convert.ToInt32(_textBoxParameterList.First(o => o.RangeLimit == RangeLimitsEnum.From).Text);
                    var x = Convert.ToInt32(_textBoxParameterList.First(o => o.RangeLimit == RangeLimitsEnum.To).Text);
                    if (x < i) throw new Exception($"End limit is less than start limit. {i}-{x}");
                    var j = Convert.ToInt32(_textBoxParameterList.Where(o => o.RangeLimit == RangeLimitsEnum.From).Skip(1).First().Text);
                    var y = Convert.ToInt32(_textBoxParameterList.Where(o => o.RangeLimit == RangeLimitsEnum.To).Skip(1).First().Text);
                    if (y < j) throw new Exception($"End limit is less than start limit. {j}-{y}");
                    var k = Convert.ToInt32(_textBoxParameterList.Where(o => o.RangeLimit == RangeLimitsEnum.From).Skip(2).First().Text);
                    var z = Convert.ToInt32(_textBoxParameterList.Where(o => o.RangeLimit == RangeLimitsEnum.To).Skip(2).First().Text);
                    if (z < k) throw new Exception($"End limit is less than start limit. {k}-{z}");

                    _thread = new Thread(() => { Loop3(i, x, j, y, k, z); });
                    _thread.Start();
                }
            }
            catch (Exception ex)
            {
                Common.ShowErrorMessageBox(ex);
            }
        }

        private void Loop1(int i, int x)
        {
            try
            {
                _isRunning = true;
                try
                {
                    do
                    {
                        PulseLed.Pulse(_doLoop ? 300 : _longPulse);

                        for (var b = i; b <= x; b++)
                        {
                            if (_stopRunning) return;

                            _dcsAPI.Parameters[0].Value = b.ToString();

                            SetCurrentTestStructure(_dcsAPI);
                            ICEventHandler.SendCommand(_dcsAPI);
                            AutoResetEvent1.WaitOne();
                            Thread.Sleep(_threadLoopSleep);
                            Dispatcher?.BeginInvoke((Action)(SetFormState));
                        }
                    } while (_doLoop);
                }
                finally
                {
                    Thread.Sleep(500);
                    _isRunning = false;
                    Dispatcher?.BeginInvoke((Action)(SetFormState));
                    PulseLed.Pulse();
                }
            }
            catch (Exception ex)
            {
                Common.ShowErrorMessageBox(ex);
            }
        }

        private void Loop2(int i, int x, int j, int y)
        {
            try
            {
                _isRunning = true;
                try
                {
                    do
                    {
                        PulseLed.Pulse(_doLoop ? 300 : _longPulse);

                        for (var b = i; b <= x; b++)
                        {
                            for (var c = j; c <= y; c++)
                            {
                                if (_stopRunning) return;

                                _dcsAPI.Parameters[0].Value = b.ToString();
                                _dcsAPI.Parameters[1].Value = c.ToString();
                                lock (_lockObject)
                                {
                                    if (_resultComparatorList.Count > 0) _resultComparatorList[0].GetResultString();
                                    SetCurrentTestStructure(_dcsAPI);
                                    _resultComparatorList[0].GetResultString();
                                }
                                ICEventHandler.SendCommand(_dcsAPI);
                                AutoResetEvent1.WaitOne();
                                Thread.Sleep(_threadLoopSleep);
                                Dispatcher?.BeginInvoke((Action)(SetFormState));
                            }
                        }
                    } while (_doLoop);
                }
                finally
                {
                    Thread.Sleep(500);
                    _isRunning = false;
                    Dispatcher?.BeginInvoke((Action)(SetFormState));
                    PulseLed.Pulse();
                }
            }
            catch (Exception ex)
            {
                Common.ShowErrorMessageBox(ex);
            }
        }

        private void Loop3(int i, int x, int j, int y, int k, int z)
        {
            try
            {
                _isRunning = true;
                try
                {
                    do
                    {
                        PulseLed.Pulse(_doLoop ? 300 : _longPulse);

                        for (var b = i; b <= x; b++)
                        {
                            for (var c = j; c <= y; c++)
                            {
                                for (var d = k; d <= z; d++)
                                {
                                    if (_stopRunning) return;

                                    _dcsAPI.Parameters[0].Value = b.ToString();
                                    _dcsAPI.Parameters[1].Value = c.ToString();
                                    _dcsAPI.Parameters[2].Value = d.ToString();

                                    SetCurrentTestStructure(_dcsAPI);
                                    ICEventHandler.SendCommand(_dcsAPI);
                                    AutoResetEvent1.WaitOne();
                                    Thread.Sleep(_threadLoopSleep);
                                    Dispatcher?.BeginInvoke((Action)(SetFormState));
                                }
                            }
                        }
                    } while (_doLoop);
                }
                finally
                {
                    Thread.Sleep(500);
                    _isRunning = false;
                    Dispatcher?.BeginInvoke((Action)(SetFormState));
                    PulseLed.Pulse();
                }
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
                    //var controlList = new List<Control>();
                    StackPanelParameters.Children.Clear();

                    foreach (var dcsAPIParameterType in _dcsAPI.Parameters)
                    {
                        var stackPanel = new StackPanel
                        {
                            Orientation = Orientation.Horizontal
                        };

                        var label = new Label
                        {

                            Content = dcsAPIParameterType.ParameterName.Replace("_", "__") + " from :",
                            VerticalAlignment = VerticalAlignment.Center
                        };
                        stackPanel.Children.Add(label);
                        //controlList.Add(label);

                        var textBox = new TextBoxParam
                        {
                            Name = "TextBox" + dcsAPIParameterType.Id,
                            Tag = dcsAPIParameterType.Id,
                            MinWidth = 50,
                            MaxWidth = 100,
                            Height = 20,
                            IsTabStop = true,
                            RangeLimit = RangeLimitsEnum.From
                        };

                        if (dcsAPIParameterType.Type == ParameterTypeEnum.number)
                        {
                            textBox.KeyDown += TextBoxParameter_OnKeyDown_Number;
                        }
                        textBox.KeyUp += TextBoxParameter_OnKeyUp;
                        stackPanel.Children.Add(textBox);
                        //controlList.Add(textBox);
                        _textBoxParameterList.Add(textBox);

                        label = new Label
                        {
                            Content = " to :",
                            VerticalAlignment = VerticalAlignment.Center
                        };
                        stackPanel.Children.Add(label);
                        //controlList.Add(label);

                        textBox = new TextBoxParam
                        {
                            Name = "TextBox" + dcsAPIParameterType.Id,
                            Tag = dcsAPIParameterType.Id,
                            MinWidth = 50,
                            MaxWidth = 100,
                            Height = 20,
                            IsTabStop = true,
                            RangeLimit = RangeLimitsEnum.To
                        };

                        if (dcsAPIParameterType.Type == ParameterTypeEnum.number)
                        {
                            textBox.KeyDown += TextBoxParameter_OnKeyDown_Number;
                        }
                        textBox.KeyUp += TextBoxParameter_OnKeyUp;

                        stackPanel.Children.Add(textBox);
                        //controlList.Add(textBox);
                        _textBoxParameterList.Add(textBox);
                        StackPanelParameters.Children.Add(stackPanel);
                    }

                    StackPanelParameters.UpdateLayout();
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

        private void PopulateAPIComboBox()
        {
            try
            {
                ComboBoxAPI.ItemsSource = null;
                ComboBoxAPI.ItemsSource = _dcsAPIList.Where(o => o.ParamCount > 0);
                ComboBoxAPI.DisplayMemberPath = "Syntax";
                ComboBoxAPI.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                Common.ShowErrorMessageBox(ex);
            }
        }

        private void ComboBoxAPI_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                _dcsAPI = (DCSAPI)ComboBoxAPI.SelectedItem;
                _textBoxParameterList.Clear();
                BuildUI();
            }
            catch (Exception ex)
            {
                Common.ShowErrorMessageBox(ex);
            }
        }

        private void ButtonInformation_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                MessageBox.Show(this, "Hard to pinpoint a certain behaviour or value in DCS?\nHere you can range test APIs and see what happens.\n\n" +
                                      "For example test get_frequency() over devices 0 - 50 to see which ones supports it.\n\n" +
                                      "DCS will throw errors when e.g. a device doesn't support a certain API (function).", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                Common.ShowErrorMessageBox(ex);
            }
        }

        private void ButtonStart_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                StartTesting();
            }
            catch (Exception ex)
            {
                Common.ShowErrorMessageBox(ex);
            }
        }

        private void ButtonStop_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                _stopRunning = true;
                AutoResetEvent1.Set();
                SetFormState();
            }
            catch (Exception ex)
            {
                Common.ShowErrorMessageBox(ex);
            }
        }

        private void CheckBoxTop_OnChecked(object sender, RoutedEventArgs e)
        {
            try
            {
                Topmost = true;
            }
            catch (Exception ex)
            {
                Common.ShowErrorMessageBox(ex);
            }
        }

        private void CheckBoxTop_OnUnchecked(object sender, RoutedEventArgs e)
        {
            try
            {
                Topmost = false;
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
                if (e.Key is not (>= Key.D0 and <= Key.D9 or >= Key.NumPad0 and <= Key.NumPad9 or Key.OemPeriod or Key.Tab or Key.Enter) && e.Key != Key.OemMinus && e.Key != Key.OemPlus
                    && e.Key != Key.Add && e.Key != Key.Subtract)
                {
                    e.Handled = true;
                    return;
                }

                if (e.Key == Key.Enter && ButtonStart.IsEnabled)
                {
                    StartTesting();
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
                SetFormState();
            }
            catch (Exception ex)
            {
                Common.ShowErrorMessageBox(ex);
            }
        }

        private void WindowRangeTest_OnClosing(object sender, CancelEventArgs e)
        {
            try
            {
                Dispose();
            }
            catch (Exception ex)
            {
                Common.ShowErrorMessageBox(ex);
            }
        }

        private void CheckBoxLoop_OnChecked(object sender, RoutedEventArgs e)
        {
            try
            {
                _doLoop = true;
                CheckBoxShowChangesOnly.IsChecked = true;
                SetFormState();
            }
            catch (Exception ex)
            {
                Common.ShowErrorMessageBox(ex);
            }
        }

        private void CheckBoxLoop_OnUnchecked(object sender, RoutedEventArgs e)
        {
            try
            {
                _doLoop = false;
                SetFormState();
            }
            catch (Exception ex)
            {
                Common.ShowErrorMessageBox(ex);
            }
        }

        private void CheckBoxShowChangesOnly_OnChecked(object sender, RoutedEventArgs e)
        {
            try
            {
                _showChangesOnly = true;
                SetFormState();
            }
            catch (Exception ex)
            {
                Common.ShowErrorMessageBox(ex);
            }
        }

        private void CheckBoxShowChangesOnly_OnUnchecked(object sender, RoutedEventArgs e)
        {
            try
            {
                _showChangesOnly = false;
                SetFormState();
            }
            catch (Exception ex)
            {
                Common.ShowErrorMessageBox(ex);
            }
        }

        private void AddResultText(string text)
        {
            TextBoxResults.Text += text + "\n";
            TextBoxResults.ScrollToEnd();
        }

        private void ComboBoxDecimals_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                var item = ComboBoxDecimals.SelectedValue.ToString();
                if (item != null && item != "*")
                {
                    ResultComparator.SetDecimals(true, Convert.ToInt32(item));
                    return;
                }

                ResultComparator.SetDecimals(false, 0);
            }
            catch (Exception ex)
            {
                Common.ShowErrorMessageBox(ex);
            }
        }

        private void CheckBoxShowErrors_OnChecked(object sender, RoutedEventArgs e)
        {
            try
            {
                _showErrors = true;
            }
            catch (Exception ex)
            {
                Common.ShowErrorMessageBox(ex);
            }
        }

        private void CheckBoxShowErrors_OnUnchecked(object sender, RoutedEventArgs e)
        {
            try
            {
                _showErrors = false;
            }
            catch (Exception ex)
            {
                Common.ShowErrorMessageBox(ex);
            }
        }

        private void CheckBoxShowNilResults_OnChecked(object sender, RoutedEventArgs e)
        {
            try
            {
                _showNilResults = true;
            }
            catch (Exception ex)
            {
                Common.ShowErrorMessageBox(ex);
            }
        }

        private void CheckBoxShowNilResults_OnUnchecked(object sender, RoutedEventArgs e)
        {
            try
            {
                _showNilResults = false;
            }
            catch (Exception ex)
            {
                Common.ShowErrorMessageBox(ex);
            }
        }
    }
}
