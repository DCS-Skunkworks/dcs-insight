using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Input;
using DCSInsight.Events;
using DCSInsight.Interfaces;
using DCSInsight.JSON;
using DCSInsight.UserControls;
using NLog;
using NLog.Targets.Wrappers;
using NLog.Targets;
using DCSInsight.Misc;
using ErrorEventArgs = DCSInsight.Events.ErrorEventArgs;
using System.Windows.Media.Imaging;
using DCSInsight.Communication;
using DCSInsight.Windows;

namespace DCSInsight
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, IErrorListener, IConnectionListener, IDataListener, IDisposable
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private List<DCSAPI> _dcsAPIList = new();
        private readonly List<UserControlAPI> _loadedAPIUserControls = new();
        private bool _formLoaded;
        private TCPClientHandler _tcpClientHandler;
        private bool _isConnected;
        private bool _rangeTesting;


        public MainWindow()
        {
            InitializeComponent();
            ICEventHandler.AttachErrorListener(this);
            ICEventHandler.AttachConnectionListener(this);
            ICEventHandler.AttachDataListener(this);
        }

        public void Dispose()
        {
            ICEventHandler.DetachErrorListener(this);
            ICEventHandler.DetachConnectionListener(this);
            ICEventHandler.DetachDataListener(this);
            _tcpClientHandler?.Dispose();
            GC.SuppressFinalize(this);
        }

        private void MainWindow_OnLoaded(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_formLoaded) return;

                ShowVersionInfo();
                SetFormState();
                CheckBoxTop.IsChecked = true;
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
                ButtonConnect.IsEnabled = !string.IsNullOrEmpty(TextBoxServer.Text) && !string.IsNullOrEmpty(TextBoxPort.Text);
                ButtonRangeTest.IsEnabled = _isConnected && _dcsAPIList.Count > 0;
            }
            catch (Exception ex)
            {
                Common.ShowErrorMessageBox(ex);
            }
        }

        private void Connect()
        {
            try
            {
                Mouse.OverrideCursor = Cursors.Wait;
                try
                {
                    _tcpClientHandler?.Disconnect();
                    _isConnected = false;
                    _tcpClientHandler = new TCPClientHandler(TextBoxServer.Text, TextBoxPort.Text);
                    _tcpClientHandler.Connect();
                    _tcpClientHandler.LogJSON = true;//TODO
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

        private void Disconnect()
        {
            try
            {
                Mouse.OverrideCursor = Cursors.Wait;
                try
                {
                    _isConnected = false;
                    _tcpClientHandler?.Disconnect();
                    _dcsAPIList.Clear();
                    _loadedAPIUserControls.Clear();
                    ItemsControlAPI.ItemsSource = null;
                    ItemsControlAPI.Items.Clear();
                    SetConnectionStatus(_isConnected);
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

        public void ConnectionStatus(ConnectionEventArgs args)
        {
            try
            {
                _isConnected = args.IsConnected;
                Dispatcher?.BeginInvoke((Action)(() => SetConnectionStatus(args.IsConnected)));
                Dispatcher?.BeginInvoke((Action)(SetFormState)); ;
            }
            catch (Exception ex)
            {
                Dispatcher?.BeginInvoke((Action)(() => Common.ShowErrorMessageBox(ex)));
            }
        }
        
        public void ErrorMessage(ErrorEventArgs args)
        {
            try
            {
                if (_rangeTesting) return;

                Logger.Error(args.Ex);
                Dispatcher?.BeginInvoke((Action)(() => TextBlockMessage.Text = $"{args.Message}. See log file."));
            }
            catch (Exception ex)
            {
                Debug.WriteLine("ErrorMessage() : " + ex.Message);
            }
        }

        public void DataReceived(DataEventArgs args)
        {
            try
            {
                if (_rangeTesting) return;

                if (args.DCSAPIS != null)
                {
                    HandleAPIMessage(args.DCSAPIS);
                }
                if (args.DCSApi != null)
                {
                    HandleMessage(args.DCSApi);
                }
            }
            catch (Exception ex)
            {
                Dispatcher?.BeginInvoke((Action)(() => Common.ShowErrorMessageBox(ex)));
            }
        }

        private void SetConnectionStatus(bool connected)
        {
            ButtonConnect.Content = connected ? "Disconnect" : "Connect";
            Title = connected ? "Connected" : "Disconnected";
            SetFormState();
            _loadedAPIUserControls.ForEach(o => o.SetConnectionStatus(_isConnected));
        }

        private void HandleMessage(DCSAPI dcsApi)
        {
            try
            {
                foreach (var userControlApi in _loadedAPIUserControls)
                {
                    if (userControlApi.Id == dcsApi.Id)
                    {
                        userControlApi.SetResult(dcsApi);
                    }
                }

                Dispatcher?.BeginInvoke((Action)(SetFormState));
            }
            catch (Exception ex)
            {
                Common.ShowErrorMessageBox(ex, "HandleMessage()");
            }
        }

        private void HandleAPIMessage(List<DCSAPI> dcsApis)
        {
            try
            {
                _dcsAPIList = dcsApis;
                //Debug.WriteLine("Count is " + _dcsAPIList.Count);
                Dispatcher?.BeginInvoke((Action)(() => ShowAPIs()));
                Dispatcher?.BeginInvoke((Action)(SetFormState));
            }
            catch (Exception ex)
            {
                Dispatcher?.BeginInvoke((Action)(() => Common.ShowErrorMessageBox(ex)));
            }
        }
        
        private void ButtonConnect_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!_isConnected)
                {
                    Connect();
                    return;
                }
                Disconnect();
            }
            catch (Exception ex)
            {
                Common.ShowErrorMessageBox(ex);
            }
        }

        /*
        private static List<DCSBIOSControl> ReadControlsFromDocJson(string inputPath)
        {
            // input is a map from category string to a map from key string to control definition
            // we read it all then flatten the grand children (the control definitions)
            var input = File.ReadAllText(inputPath);
            try
            {
                return JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, DCSBIOSControl>>>(input)!
                    .Values
                    .SelectMany(category => category.Values)
                    .ToList();
            }
            catch (Exception e)
            {
                Logger.Error(e, "ReadControlsFromDocJson : Failed to read DCS-BIOS JSON.");
            }

            return null;
        }*/

        private void MainWindow_OnClosing(object sender, CancelEventArgs e)
        {
            try
            {
                _isConnected = false;
                _tcpClientHandler?.Disconnect();
            }
            catch (Exception ex)
            {
                Common.ShowErrorMessageBox(ex);
            }
        }


        private static void TryOpenLogFileWithTarget(string targetName)
        {
            try
            {
                var logFilePath = GetLogFilePathByTarget(targetName);
                if (logFilePath == null || !File.Exists(logFilePath))
                {
                    MessageBox.Show($"No log file found {logFilePath}", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }
                Process.Start(new ProcessStartInfo
                {
                    FileName = logFilePath,
                    UseShellExecute = true
                });
            }
            catch (Exception ex)
            {
                Common.ShowErrorMessageBox(ex);
            }
        }

        /// <summary>
        /// Try to find the path of the log with a file target given as parameter
        /// See NLog.config in the main folder of the application for configured log targets
        /// </summary>
        private static string GetLogFilePathByTarget(string targetName)
        {
            string fileName;
            if (LogManager.Configuration != null && LogManager.Configuration.ConfiguredNamedTargets.Count != 0)
            {
                Target target = LogManager.Configuration.FindTargetByName(targetName);
                if (target != null)
                {
                    FileTarget fileTarget;

                    // Unwrap the target if necessary.
                    if (target is not WrapperTargetBase wrapperTarget)
                    {
                        fileTarget = target as FileTarget;
                    }
                    else
                    {
                        fileTarget = wrapperTarget.WrappedTarget as FileTarget;
                    }

                    if (fileTarget == null)
                    {
                        throw new Exception($"Could not get a FileTarget type log from {target.GetType()}");
                    }

                    var logEventInfo = new LogEventInfo { TimeStamp = DateTime.Now };
                    fileName = fileTarget.FileName.Render(logEventInfo);
                }
                else
                {
                    throw new Exception($"Could not find log with a target named: [{targetName}]. See NLog.config for configured targets");
                }
            }
            else
            {
                throw new Exception("LogManager contains no configuration or there are no named targets. See NLog.config file to configure the logs.");
            }
            return fileName;
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

        private void UpdateSearchButton()
        {

            if (!string.IsNullOrEmpty(TextBoxSearchAPI.Text))
            {
                ButtonSearchAPI.Source = new BitmapImage(new Uri(@"/dcs-insight;component/Images/clear_search_result.png", UriKind.Relative));
                ButtonSearchAPI.Tag = "Clear";
            }
            else
            {
                ButtonSearchAPI.Source = new BitmapImage(new Uri(@"/dcs-insight;component/Images/search_api.png", UriKind.Relative));
                ButtonSearchAPI.Tag = "Search";
            }
        }

        private void ButtonSearchAPI_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                var mode = (string)ButtonSearchAPI.Tag;
                if (mode == "Search")
                {
                    ButtonSearchAPI.Source = new BitmapImage(new Uri(@"/dcs-insight;component/Images/clear_search_result.png", UriKind.Relative));
                    ButtonSearchAPI.Tag = "Clear";
                }
                else
                {
                    ButtonSearchAPI.Source = new BitmapImage(new Uri(@"/dcs-insight;component/Images/search_api.png", UriKind.Relative));
                    ButtonSearchAPI.Tag = "Search";
                    TextBoxSearchAPI.Text = "";
                }

                ShowAPIs(true);
            }
            catch (Exception ex)
            {
                Common.ShowErrorMessageBox(ex);
            }
        }


        private void ShowAPIs(bool searching = false)
        {
            try
            {
                try
                {
                    Mouse.OverrideCursor = Cursors.Wait;
                    _loadedAPIUserControls.Clear();

                    var searchText = string.IsNullOrEmpty(TextBoxSearchAPI.Text) ? "" : TextBoxSearchAPI.Text.Trim();
                    var filteredAPIs = _dcsAPIList;

                    if (!string.IsNullOrEmpty(searchText))
                    {
                        var searchWord = searchText.ToLower();
                        filteredAPIs = _dcsAPIList.Where(o => o.Syntax.ToLower().Contains(searchWord)).ToList();
                    }

                    foreach (var dcsapi in filteredAPIs)
                    {
                        var userControl = new UserControlAPI(dcsapi, _isConnected);
                        _loadedAPIUserControls.Add(userControl);
                    }

                    ItemsControlAPI.ItemsSource = null;
                    ItemsControlAPI.Items.Clear();
                    ItemsControlAPI.ItemsSource = _loadedAPIUserControls;

                    TextBlockMessage.Text = $"{filteredAPIs.Count} APIs loaded.";

                    if (filteredAPIs.Any())
                    {
                        ItemsControlAPI.Focus();
                    }

                    UpdateSearchButton();

                    if (searching)
                    {
                        TextBoxSearchAPI.Focus();
                    }
                }
                finally
                {
                    Mouse.OverrideCursor = Cursors.Arrow;
                }
            }
            catch (Exception ex)
            {
                Common.ShowErrorMessageBox(ex);
            }
        }

        private void TextBoxSearchAPI_OnKeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Key == Key.Enter)
                {
                    ShowAPIs(true);
                }
            }
            catch (Exception ex)
            {
                Common.ShowErrorMessageBox(ex);
            }
        }

        private void ShowVersionInfo()
        {
            try
            {
                var fileVersionInfo = FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location);
                TextBlockAppInfo.Text = $"dcs-insight v.{fileVersionInfo.FileVersion}";
            }
            catch (Exception ex)
            {
                Common.ShowErrorMessageBox(ex);
            }
        }

        private void TextBlockAppInfo_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                TryOpenLogFileWithTarget("logfile");
                if (_tcpClientHandler == null) return;

                _tcpClientHandler.LogJSON = true;
            }
            catch (Exception ex)
            {
                Common.ShowErrorMessageBox(ex);
            }
        }

        private void ButtonRangeTest_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                try
                {
                    _rangeTesting = true;
                    var windowRangeTest = new WindowRangeTest(_dcsAPIList);
                    windowRangeTest.ShowDialog();
                }
                finally
                {
                    _rangeTesting = false;
                }
            }
            catch (Exception ex)
            {
                Common.ShowErrorMessageBox(ex);
            }
        }
    }
}
