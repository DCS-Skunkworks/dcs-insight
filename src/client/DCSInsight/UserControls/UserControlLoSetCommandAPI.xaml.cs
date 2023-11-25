using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using DCSInsight.Events;
using DCSInsight.JSON;
using DCSInsight.Misc;

namespace DCSInsight.UserControls
{
    /// <summary>
    /// Interaction logic for UserControlLoSetCommandAPI.xaml
    /// </summary>
    public partial class UserControlLoSetCommandAPI : UserControlAPIBase
    {

        public UserControlLoSetCommandAPI(DCSAPI dcsAPI, bool isConnected) : base(dcsAPI, isConnected)
        {
            InitializeComponent();
        }

        public new void Dispose()
        {
            base.Dispose();
            GC.SuppressFinalize(this);
        }

        public new async ValueTask DisposeAsync()
        {
            await base.DisposeAsync();
            GC.SuppressFinalize(this);
        }

        private void UserControlLoSetCommandAPI_OnLoaded(object sender, RoutedEventArgs e)
        {
            try
            {
                if (IsControlLoaded) return;

                IsTabStop = true;

                BuildUI();
                IsControlLoaded = true;
            }
            catch (Exception ex)
            {
                Common.ShowErrorMessageBox(ex);
            }
        }

        protected override void SetFormState()
        {
            try
            {
                ButtonSend.IsEnabled = !TextBoxParameterList.Any(o => string.IsNullOrEmpty(o.Text)) && IsConnected;

                if (DCSAPI.ReturnsData)
                {
                    CheckBoxPolling.IsEnabled = ButtonSend.IsEnabled;
                    ComboBoxPollTimes.IsEnabled = CheckBoxPolling.IsChecked == false;
                }
                CanSend = ButtonSend.IsEnabled;
            }
            catch (Exception ex)
            {
                Common.ShowErrorMessageBox(ex);
            }
        }

        protected override void BuildUI()
        {
            try
            {
                Mouse.OverrideCursor = Cursors.Wait;

                try
                {
                    TextBoxSyntax.Text = DCSAPI.Syntax;
                    TextBoxSyntax.ToolTip = $"Click to copy syntax. (API Id = {DCSAPI.Id})";

                    var controlList = new List<Control>();

                    foreach (var dcsAPIParameterType in DCSAPI.Parameters)
                    {
                        var label = new Label
                        {
                            Content = dcsAPIParameterType.ParameterName.Replace("_", "__"),
                            VerticalAlignment = VerticalAlignment.Center
                        };
                        controlList.Add(label);

                        if (dcsAPIParameterType.ParameterName == "iCommand")
                        {
                            var commands = LoSetCommand.LoadCommands();
                            var comboBox = new ComboBox
                            {
                                Name = "ComboBox" + dcsAPIParameterType.Id,
                                Tag = dcsAPIParameterType.Id,
                                MinWidth = 50,
                                MaxWidth = 100,
                                Height = 20,
                                IsReadOnly = true,
                                DisplayMemberPath = "Description",
                                ItemsSource = commands
                            };
                            controlList.Add(comboBox);
                            ComboBoxParameterList.Add(comboBox);
                        }
                        else
                        {
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
                            TextBoxParameterList.Add(textBox);
                        }
                    }

                    ButtonSend = new Button
                    {
                        Content = "Send",
                        Height = 20,
                        Width = 50,
                        VerticalAlignment = VerticalAlignment.Center,
                        Margin = new Thickness(20, 0, 0, 0)
                    };
                    ButtonSend.Click += ButtonSend_OnClick;
                    controlList.Add(ButtonSend);

                    if (DCSAPI.ReturnsData)
                    {
                        LabelKeepResults = new Label
                        {
                            Content = "Keep results",
                            VerticalAlignment = VerticalAlignment.Center,
                            Margin = new Thickness(10, 0, 0, 0)
                        };
                        controlList.Add(LabelKeepResults);

                        CheckBoxKeepResults = new CheckBox
                        {
                            Margin = new Thickness(0, 0, 0, 0),
                            VerticalAlignment = VerticalAlignment.Center
                        };
                        CheckBoxKeepResults.Checked += CheckBoxKeepResults_OnChecked;
                        CheckBoxKeepResults.Unchecked += CheckBoxKeepResults_OnUnchecked;
                        controlList.Add(CheckBoxKeepResults);

                        LabelPolling = new Label
                        {
                            Content = "Polling",
                            VerticalAlignment = VerticalAlignment.Center,
                            Margin = new Thickness(10, 0, 0, 0)
                        };
                        controlList.Add(LabelPolling);

                        CheckBoxPolling = new CheckBox
                        {
                            Margin = new Thickness(0, 0, 0, 0),
                            VerticalAlignment = VerticalAlignment.Center

                        };
                        CheckBoxPolling.Checked += CheckBoxPolling_OnChecked;
                        CheckBoxPolling.Unchecked += CheckBoxPolling_OnUnchecked;
                        controlList.Add(CheckBoxPolling);

                        LabelPollingInterval = new Label
                        {
                            Content = "Interval (ms) :",
                            VerticalAlignment = VerticalAlignment.Center,
                            Margin = new Thickness(10, 0, 0, 0)
                        };
                        controlList.Add(LabelPollingInterval);

                        ComboBoxPollTimes = new ComboBox
                        {
                            Height = 20,
                            Margin = new Thickness(2, 0, 0, 0),
                            VerticalAlignment = VerticalAlignment.Center
                        };
                        ComboBoxPollTimes.DataContextChanged += ComboBoxPollTimes_OnDataContextChanged;
                        ComboBoxPollTimes.Items.Add(500);
                        ComboBoxPollTimes.Items.Add(1000);
                        ComboBoxPollTimes.Items.Add(2000);
                        ComboBoxPollTimes.SelectedIndex = 0;
                        controlList.Add(ComboBoxPollTimes);
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
        
        public override void SetResult(DCSAPI dcsApi)
        {
            try
            {
                Dispatcher?.BeginInvoke((Action)(() => LabelResult.Content = $"Result ({dcsApi.ResultType})"));

                var result = dcsApi.ErrorThrown ? dcsApi.ErrorMessage : (string.IsNullOrEmpty(dcsApi.Result) ? "nil" : dcsApi.Result);

                if (KeepResults)
                {
                    Dispatcher?.BeginInvoke((Action)(() => TextBoxResult.Text = TextBoxResult.Text.Insert(0, "\n---\n")));
                    Dispatcher?.BeginInvoke((Action)(() => TextBoxResult.Text = TextBoxResult.Text.Insert(0, result)));
                    return;
                }
                Dispatcher?.BeginInvoke((Action)(() => TextBoxResult.Text = result));
            }
            catch (Exception ex)
            {
                Common.ShowErrorMessageBox(ex);
            }
        }

        protected override void SendCommand()
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
    }
}
