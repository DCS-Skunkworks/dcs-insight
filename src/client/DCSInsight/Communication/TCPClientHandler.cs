using DCSInsight.Events;
using DCSInsight.JSON;
using DCSInsight.Misc;
using Newtonsoft.Json;
using NLog;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using DCSInsight.Interfaces;

namespace DCSInsight.Communication
{
    internal class TCPClientHandler : IDisposable, ICommandListener

    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private readonly Channel<DCSAPI> _asyncCommandsChannel = Channel.CreateUnbounded<DCSAPI>();
        private TcpClient _tcpClient;
        private Thread _clientThread;
        private bool _isRunning;
        private readonly string _host;
        private readonly string _port;
        private bool _apiListReceived = false;
        private int _metaDataPollCounter;
        public bool LogJSON { get; set; }
        private string _currentMessage = "";
        private volatile bool _responseReceived;
        private bool _requestAPIList;

        public TCPClientHandler(string host, string port, bool requestAPIList)
        {
            _host = host;
            _port = port;
            _requestAPIList = requestAPIList;
            ICEventHandler.AttachCommandListener(this);
        }


        public void Dispose()
        {
            ICEventHandler.DetachCommandListener(this);
            _tcpClient?.Dispose();
            GC.SuppressFinalize(this);
        }

        private async void ClientThread()
        {
            ICEventHandler.SendConnectionStatus(_isRunning);
            _responseReceived = true;
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

                    if (_requestAPIList && !_apiListReceived && _metaDataPollCounter < 1)
                    {
                        Thread.Sleep(300);
                        _metaDataPollCounter++;
                        _tcpClient.GetStream().Write(Encoding.ASCII.GetBytes("SENDAPI\n"));
                        Thread.Sleep(1000);
                    }

                    if (_asyncCommandsChannel.Reader.Count > 0 && _responseReceived)
                    {
                        var cts = new CancellationTokenSource(100);
                        var dcsApi = await _asyncCommandsChannel.Reader.ReadAsync(cts.Token);
                        if (LogJSON) Logger.Info(JsonConvert.SerializeObject(dcsApi, Formatting.Indented));
                        _tcpClient.GetStream().Write(Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(dcsApi) + "\n"));
                        _responseReceived = false;
                    }

                    if (_tcpClient.Available <= 0) continue;

                    var bytes = new byte[_tcpClient.Available];
                    var bytesRead = _tcpClient.GetStream().Read(bytes);
                    var msg = Encoding.ASCII.GetString(bytes);
                    if (LogJSON) Logger.Info(msg);
                    HandleMessage(msg);
                    Thread.Sleep(100);
                }
                catch (SocketException ex)
                {
                    Logger.Error(ex);
                    break;
                }
            }

            _isRunning = false;
            _tcpClient = null;
            ICEventHandler.SendConnectionStatus(_isRunning);
        }
        
        private void HandleMessage(string str)
        {
            try
            {
                if (!_apiListReceived)
                {
                    HandleAPIMessage(str);
                    return;
                }

                if (str.Contains("\"returns_data\":") && str.EndsWith("}")) // regex?
                {
                    var dcsApi = JsonConvert.DeserializeObject<DCSAPI>(_currentMessage + str);
                    _currentMessage = "";
                    ICEventHandler.SendData(dcsApi);
                    _responseReceived = true;
                }
                else
                {
                    _currentMessage += str;
                }
            }
            catch (Exception ex)
            {
                Common.ShowErrorMessageBox(ex, "HandleMessage()");
            }
        }

        private void HandleAPIMessage(string str)
        {
            try
            {
                var dcsAPIList = JsonConvert.DeserializeObject<List<DCSAPI>>(str);
                ICEventHandler.SendData(dcsAPIList);
                _responseReceived = true;
                _apiListReceived = true;
            }
            catch (Exception ex)
            {
                Common.ShowErrorMessageBox(ex, "HandleAPIMessage()");
            }
        }


        public void Connect()
        {
            if (_isRunning) return;

            try
            {
                IPEndPoint serverEndPoint;

                if (_host != "127.0.0.1")
                {
                    serverEndPoint = new(IPAddress.Parse(_host), Convert.ToInt32(_port));
                }
                else
                {
                    serverEndPoint = new(IPAddress.Loopback, Convert.ToInt32(_port));
                }
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

        public void Disconnect()
        {
            try
            {
                _isRunning = false;
                _tcpClient?.Close();
                _tcpClient = null;
            }
            catch (Exception ex)
            {
                Common.ShowErrorMessageBox(ex);
            }
        }

        public async Task AddAsyncCommand(DCSAPI dcsApi)
        {
            var cts = new CancellationTokenSource(100);
            await _asyncCommandsChannel.Writer.WriteAsync(dcsApi, cts.Token);
        }
        
        public async void SendCommand(SendCommandEventArgs args)
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
    }
}
