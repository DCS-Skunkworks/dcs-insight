using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using DCSInsight.JSON;
using DCSInsight.Misc;

namespace DCSInsight.UserControls
{
    /// <summary>
    /// Interaction logic for UserControlLoSetCommandAPI.xaml
    /// </summary>
    public partial class UserControlLoSetCommandAPI : UserControlAPIBase
    {

        private Popup _popupSearchICommand;
        private DataGrid _dataGridValues;
        private List<LoSetCommand> _loSetICommands;
        private LoSetCommand _loSetICommand;
        private TextBox _textBoxSearchICommand;

        public UserControlLoSetCommandAPI(DCSAPI dcsAPI, bool isConnected) : base(dcsAPI, isConnected)
        {
            InitializeComponent();
            LabelResultBase = LabelResult;
            TextBoxResultBase = TextBoxResult;
        }

        private void UserControlLoSetCommandAPI_OnLoaded(object sender, RoutedEventArgs e)
        {
            try
            {
                if (IsControlLoaded) return;

                _loSetICommands = LoSetCommand.LoadCommands();
                _popupSearchICommand = (Popup)FindResource("PopUpSearchResults");
                _popupSearchICommand.Height = 400;
                _dataGridValues = (DataGrid)LogicalTreeHelper.FindLogicalNode(_popupSearchICommand, "DataGridValues");
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

                        if (dcsAPIParameterType.ParameterName.StartsWith('i'))
                        {
                            var commands = LoSetCommand.LoadCommands();
                            _textBoxSearchICommand = new TextBox
                            {
                                Name = "TextBox" + dcsAPIParameterType.Id,
                                Tag = dcsAPIParameterType.Id,
                                MinWidth = 150,
                                MaxWidth = 350,
                                Height = 20,
                            };

                            _textBoxSearchICommand.TextChanged += TextBoxSearchICommand_OnTextChanged;
                            _textBoxSearchICommand.KeyUp += TextBoxSearchICommand_OnKeyUp;
                            _textBoxSearchICommand.PreviewKeyDown += TextBoxSearchICommand_PreviewKeyDown;
                            TextBoxSearchLoSetCommands.SetBackgroundSearchBanner(_textBoxSearchICommand);
                            controlList.Add(_textBoxSearchICommand);
                            TextBoxParameterList.Add(_textBoxSearchICommand);
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
                            Content = "Poll",
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
                        ComboBoxPollTimes.Items.Add(50);
                        ComboBoxPollTimes.Items.Add(100);
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
        
        private void Selector_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (_dataGridValues.SelectedItems.Count == 1)
                {
                    _loSetICommand = (LoSetCommand)_dataGridValues.SelectedItem;
                    _textBoxSearchICommand.Text = _loSetICommand.Code;
                }
                SetFormState();
            }
            catch (Exception ex)
            {
                Common.ShowErrorMessageBox(ex);
            }
        }
        
        private void Control_OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (_dataGridValues.SelectedItems.Count == 1)
                {
                    _loSetICommand = (LoSetCommand)_dataGridValues.SelectedItem;
                    _textBoxSearchICommand.Text = _loSetICommand.Code;
                    SetFormState();
                }
                _popupSearchICommand.IsOpen = false;
                SetFormState();
            }
            catch (Exception ex)
            {
                Common.ShowErrorMessageBox(ex);
            }
        }

        private void UIElement_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (_dataGridValues.SelectedItems.Count == 1)
                {
                    _loSetICommand = (LoSetCommand)_dataGridValues.SelectedItem;
                    _textBoxSearchICommand.Text = _loSetICommand.Code;
                }
                _popupSearchICommand.IsOpen = false;
                SetFormState();
            }
            catch (Exception ex)
            {
                Common.ShowErrorMessageBox(ex);
            }
        }

        private void TextBoxSearchICommand_OnKeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                TextBoxSearchLoSetCommands.AdjustShownPopupData(((TextBox)sender), _popupSearchICommand, _dataGridValues, _loSetICommands);
                SetFormState();
            }
            catch (Exception ex)
            {
                Common.ShowErrorMessageBox(ex);
            }
        }

        private void TextBoxSearchICommand_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            TextBoxSearchLoSetCommands.SetBackgroundSearchBanner(((TextBox)sender));
        }
        
        private void TextBoxSearchICommand_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            TextBoxSearchLoSetCommands.HandleFirstSpace(sender, e);
        }
    }
}
