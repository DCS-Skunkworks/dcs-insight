using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;
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
        private const string ArgumentIdConst = "argument_id";
        private bool _isConnected = true;
        private bool _isRunning;
        private static readonly AutoResetEvent AutoResetEvent1 = new AutoResetEvent(false);
        private Thread _thread;
        private readonly StringBuilder _currentTestString = new StringBuilder();

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
            AutoResetEvent1.Set();
            GC.SuppressFinalize(this);
        }

        private void WindowRangeTest_OnLoaded(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_formLoaded) return;

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
                ButtonStart.IsEnabled = !_textBoxParameterList.Any(o => string.IsNullOrEmpty(o.Text)) && _isConnected;
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
                if(!_isConnected || !_isRunning) return;

                Debug.WriteLine("Error Message Received");
                _currentTestString.Append(args.Message + "\n");
                Dispatcher?.BeginInvoke((Action)(() => TextBoxResults.Text += _currentTestString.ToString()));
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

        public void DataReceived(DataEventArgs args)
        {
            try
            {
                if (!_isConnected || !_isRunning) return;

                Debug.WriteLine("Data Received");
                _currentTestString.Append(args.DCSApi.Result + "\n");
                Dispatcher?.BeginInvoke((Action)(() => TextBoxResults.Text += _currentTestString.ToString())); ;
                AutoResetEvent1.Set();
            }
            catch (Exception ex)
            {
                Common.ShowErrorMessageBox(ex);
            }
        }

        private void StartTesting()
        {
            try
            {
                TextBoxResults.Text = "";
                var dynamicCount = _textBoxParameterList.Count(o => o.RangeLimit == RangeLimitsEnum.From);
                if (dynamicCount == 0) return;

                if (dynamicCount == 1)
                {
                    var i = Convert.ToInt32(_textBoxParameterList.First(o => o.RangeLimit == RangeLimitsEnum.From).Text);
                    var x = Convert.ToInt32(_textBoxParameterList.First(o => o.RangeLimit == RangeLimitsEnum.To).Text);
                    if (x < i) throw new Exception("End limit is less than start limit.");
                    var argumentExists = _textBoxParameterList.Any(o => o.RangeLimit == RangeLimitsEnum.None);
                    var argumentId = "";
                    if (argumentExists)
                    {
                        argumentId = _textBoxParameterList.First(o => o.RangeLimit == RangeLimitsEnum.None).Text;
                    }

                    _thread = new Thread(() => { Loop1(i, x, argumentId); });
                    _thread.Start();
                }
            }
            catch (Exception ex)
            {
                Common.ShowErrorMessageBox(ex);
            }
        }

        private void Loop1(int i, int x, string argumentId)
        {
            try
            {
                _isRunning = true;
                try
                {
                    for (var j = i; j <= x; j++)
                    {
                        _dcsAPI.Parameters[0].Value = j.ToString();
                        if (!string.IsNullOrEmpty(argumentId))
                        {
                            _dcsAPI.Parameters[1].Value = argumentId;
                        }

                        SetCurrentTestStringParameters(_dcsAPI);
                        ICEventHandler.SendCommand(_dcsAPI);
                        AutoResetEvent1.WaitOne();
                        Thread.Sleep(100);
                    }
                }
                finally
                {
                    Thread.Sleep(500);
                    _isRunning = false;
                }
            }
            catch (Exception ex)
            {
                Common.ShowErrorMessageBox(ex);
            }
        }

        private void SetCurrentTestStringParameters(DCSAPI dcsApi)
        {
            //device_id = 10, command = 2, argument_id = 433  result : 
            _currentTestString.Clear();
            foreach (var dcsApiParameter in dcsApi.Parameters)
            {
                _currentTestString.Append($"{dcsApiParameter.ParameterName} = {dcsApiParameter.Value},");
            }

            _currentTestString.Append(" result : ");
        }

        private void BuildUI()
        {
            try
            {
                Mouse.OverrideCursor = Cursors.Wait;

                try
                {
                    var controlList = new List<Control>();

                    foreach (var dcsAPIParameterType in _dcsAPI.Parameters)
                    {
                        var rangeLimit = dcsAPIParameterType.ParameterName == ArgumentIdConst ? RangeLimitsEnum.None : RangeLimitsEnum.From;

                        var textEnd = dcsAPIParameterType.ParameterName == ArgumentIdConst ? " : " : " from :";
                        var label = new Label
                        {

                            Content = dcsAPIParameterType.ParameterName.Replace("_", "__") + textEnd,
                            VerticalAlignment = VerticalAlignment.Center
                        };
                        controlList.Add(label);

                        var textBox = new TextBoxParam
                        {
                            Name = "TextBox" + dcsAPIParameterType.Id,
                            Tag = dcsAPIParameterType.Id,
                            MinWidth = 50,
                            MaxWidth = 100,
                            Height = 20,
                            IsTabStop = true,
                            RangeLimit = rangeLimit
                        };

                        if (dcsAPIParameterType.Type == ParameterTypeEnum.number)
                        {
                            textBox.KeyDown += TextBoxParameter_OnKeyDown_Number;
                        }
                        textBox.KeyUp += TextBoxParameter_OnKeyUp;

                        controlList.Add(textBox);
                        _textBoxParameterList.Add(textBox);

                        if (dcsAPIParameterType.ParameterName == ArgumentIdConst) continue;

                        label = new Label
                        {
                            Content = " to :",
                            VerticalAlignment = VerticalAlignment.Center
                        };
                        controlList.Add(label);

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

                        controlList.Add(textBox);
                        _textBoxParameterList.Add(textBox);
                    }

                    ItemsControlParameter.ItemsSource = null;
                    ItemsControlParameter.ItemsSource = controlList;
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
                AutoResetEvent1.Set();
            }
            catch (Exception ex)
            {
                Common.ShowErrorMessageBox(ex);
            }
        }
    }
}
