using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using DCSInsight.Events;
using DCSInsight.Interfaces;
using DCSInsight.JSON;
using DCSInsight.UserControls;
using Newtonsoft.Json;
using NLog;
using NLog.Targets.Wrappers;
using NLog.Targets;
using DCSInsight.Misc;
using ErrorEventArgs = DCSInsight.Events.ErrorEventArgs;

namespace DCSInsight
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, ICommandListener, IErrorListener, IDisposable
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private TcpClient _tcpClient;
        private Thread _clientThread;
        private bool _isRunning;
        private List<DCSAPI> _dcsAPIList = new();
        
        private readonly Channel<DCSAPI> _asyncCommandsChannel = Channel.CreateUnbounded<DCSAPI>();
        private int _metaDataPollCounter;
        private readonly List<UserControlAPI> _loadedAPIUserControls = new();
        private const int MAX_CONTROLS_ON_PAGE = 200;
        private bool _formLoaded;

        public MainWindow()
        {
            InitializeComponent();
            ICEventHandler.AttachCommandListener(this);
            ICEventHandler.AttachErrorListener(this);
        }

        public void Dispose()
        {
            ICEventHandler.DetachCommandListener(this);
            ICEventHandler.DetachErrorListener(this);
            _tcpClient?.Dispose();
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
                _loadedAPIUserControls.ForEach(o => o.SetConnectionStatus(_isRunning));
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
                    IPEndPoint serverEndPoint = new IPEndPoint(IPAddress.Loopback, Convert.ToInt32(TextBoxPort.Text));
                    _isRunning = false;
                    _tcpClient = new TcpClient();
                    _tcpClient.Connect(serverEndPoint);
                    _isRunning = true;
                    _clientThread = new Thread(ClientThread);
                    _clientThread.Start();
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
                    _isRunning = false;
                    _tcpClient.Close();
                    _dcsAPIList.Clear();
                    _metaDataPollCounter = 0;
                    _loadedAPIUserControls.Clear();
                    ItemsControlAPI.ItemsSource = null;
                    ItemsControlAPI.Items.Clear();
                    SetConnectionStatus(_isRunning);
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

        private async void ClientThread()
        {
            Dispatcher?.BeginInvoke((Action)(() => SetConnectionStatus(_isRunning)));
            while (_isRunning)
            {
                try
                {
                    /* pear to the documentation on Poll:
                     * When passing SelectMode.SelectRead as a parameter to the Poll method it will return
                     * -either- true if Socket.Listen(Int32) has been called and a connection is pending;
                     * -or- true if data is available for reading;
                     * -or- true if the connection has been closed, reset, or terminated;
                     * otherwise, returns false
                     */

                    // Detect if client disconnected
                    if (_tcpClient.Client.Poll(0, SelectMode.SelectRead))
                    {
                        var buffer = new byte[1];
                        if (_tcpClient.Client.Receive(buffer, SocketFlags.Peek) == 0)
                        {
                            // Client disconnected
                            break;
                        }
                    }
                    if (!_tcpClient.Connected) break;

                    if (_dcsAPIList.Count == 0 && _metaDataPollCounter < 1)
                    {
                        Thread.Sleep(300);
                        _metaDataPollCounter++;
                        _tcpClient.GetStream().Write(Encoding.ASCII.GetBytes("SENDAPI\n"));
                        Thread.Sleep(1000);
                    }

                    if (_asyncCommandsChannel.Reader.Count > 0)
                    {
                        var cts = new CancellationTokenSource(100);
                        var dcsApi = await _asyncCommandsChannel.Reader.ReadAsync(cts.Token);
                        //_tcpClient.GetStream().Write(Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(dcsApi, Formatting.Indented) + "\n"));
                        Debug.WriteLine(JsonConvert.SerializeObject(dcsApi, Formatting.Indented));
                        _tcpClient.GetStream().Write(Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(dcsApi) + "\n"));
                    }

                    if (_tcpClient.Available <= 0) continue;

                    var bytes = new byte[_tcpClient.Available];
                    var bytesRead = _tcpClient.GetStream().Read(bytes);
                    var msg = Encoding.ASCII.GetString(bytes);

                    Dispatcher?.BeginInvoke((Action)(() => HandleMessage(msg)));
                    Thread.Sleep(100);
                }
                catch (SocketException ex)
                {
                    Dispatcher?.BeginInvoke((Action)(() => ErrorMessage(this, ex.Message, ex)));
                }
            }

            _isRunning = false;
            _tcpClient = null;
            Dispatcher?.BeginInvoke((Action)(() => SetConnectionStatus(_isRunning)));
        }

        private void SetConnectionStatus(bool connected)
        {
            ButtonConnect.Content = _isRunning ? "Disconnect" : "Connect";
            Title = _isRunning ? "Connected" : "Disconnected";
            SetFormState();
        }

        private void HandleMessage(string str)
        {
            try
            {
                if (_dcsAPIList == null || _dcsAPIList.Count == 0)
                {
                    HandleAPIMessage(str);
                    return;
                }

                foreach (var userControlApi in _loadedAPIUserControls)
                {
                    var dcsApi = JsonConvert.DeserializeObject<DCSAPI>(str);
                    if (userControlApi.Id == dcsApi.Id)
                    {
                        userControlApi.SetResult(dcsApi);
                    }
                }
            }
            catch (Exception ex)
            {
                Common.ShowErrorMessageBox(ex);
            }
        }

        private void HandleAPIMessage(string str)
        {
            try
            {
                _dcsAPIList = JsonConvert.DeserializeObject<List<DCSAPI>>(str);
                Debug.WriteLine("Count is " + _dcsAPIList.Count);
                Dispatcher?.BeginInvoke((Action)(ShowAPIs));
            }
            catch (Exception ex)
            {
                Dispatcher?.BeginInvoke((Action)(() => Common.ShowErrorMessageBox(ex)));
            }
        }
        
        public async void SendCommand(object sender, SendCommandEventArgs args)
        {
            try
            {
                await AddAsyncCommand(args.APIObject);
            }
            catch (Exception ex)
            {
                Common.ShowErrorMessageBox(ex);
            }
        }

        private async Task AddAsyncCommand(DCSAPI dcsApi)
        {
            var cts = new CancellationTokenSource(100);
            await _asyncCommandsChannel.Writer.WriteAsync(dcsApi, cts.Token);
        }

        private void ButtonTest_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                _tcpClient.GetStream().Write(Encoding.ASCII.GetBytes("SENDAPI\n"));
            }
            catch (Exception ex)
            {
                Common.ShowErrorMessageBox(ex);
            }
        }

        private void ButtonConnect_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!_isRunning)
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
                _isRunning = false;
                _tcpClient?.Close();
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
                if (target == null)
                {
                    throw new Exception($"Could not find log with a target named: [{targetName}]. See NLog.config for configured targets");
                }

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
                throw new Exception("LogManager contains no configuration or there are no named targets. See NLog.config file to configure the logs.");
            }
            return fileName;
        }

        public void ErrorMessage(object sender, ErrorEventArgs args)
        {
            try
            {
                Logger.Error(args.Ex);
                Dispatcher?.BeginInvoke((Action)(() => TextBlockMessage.Text = $"{args.Message}. See log file."));
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        public void ErrorMessage(object sender, string message, Exception ex)
        {
            try
            {
                Logger.Error(ex);
                Dispatcher?.BeginInvoke((Action)(() => TextBlockMessage.Text = $"{message}. See log file."));
            }
            catch (Exception ex2)
            {
                Debug.WriteLine(ex2.Message);
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

        private void ButtonSearchControls_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                ShowAPIs();
            }
            catch (Exception ex)
            {
                Common.ShowErrorMessageBox(ex);
            }
        }


        private void ShowAPIs()
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

                    if (filteredAPIs.Count() > MAX_CONTROLS_ON_PAGE)
                    {
                        Common.ShowMessageBox($"Query returned {filteredAPIs.Count()} API. Max that can be displayed at any time is {MAX_CONTROLS_ON_PAGE}.");
                        return;
                    }

                    foreach (var dcsapi in filteredAPIs)
                    {
                        var userControl = new UserControlAPI(dcsapi, _isRunning);
                        _loadedAPIUserControls.Add(userControl);
                    }

                    ItemsControlAPI.ItemsSource = null;
                    ItemsControlAPI.Items.Clear();
                    ItemsControlAPI.ItemsSource = _loadedAPIUserControls;

                    TextBlockMessage.Text = $"{filteredAPIs.Count()} APIs loaded.";

                    if (filteredAPIs.Any())
                    {
                        ItemsControlAPI.Focus();
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
                    ShowAPIs();
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
    }
}
