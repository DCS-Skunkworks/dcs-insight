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

        private readonly Popup _popupSearchICommand;
        private readonly DataGrid _dataGridValues;
        private readonly List<LoSetCommand> _loSetICommands;
        private LoSetCommand? _loSetICommand;
        private TextBox? _textBoxSearchICommand;

        public UserControlLoSetCommandAPI(DCSAPI dcsAPI, bool isConnected) : base(dcsAPI, isConnected)
        {
            InitializeComponent();
            LabelResultBase = LabelResult;
            TextBoxResultBase = TextBoxResult;

            _loSetICommands = LoSetCommand.LoadCommands();
            if (_loSetICommands == null) throw new ArgumentException("Failed load ICommands.");

            _popupSearchICommand = (Popup)FindResource("PopUpSearchResults");

            if (_popupSearchICommand == null) throw new ArgumentException("Failed to find PopUpSearchResults.");

            _popupSearchICommand.Height = 400;
            var node = LogicalTreeHelper.FindLogicalNode(_popupSearchICommand, "DataGridValues");
            if (node == null) throw new ArgumentException("Failed to find DataGridValues.");

            _dataGridValues = (DataGrid)node;
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
                if (ButtonSend == null || CheckBoxPolling == null || ComboBoxPollTimes == null) return;

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

                    foreach (var dcsAPIParameter in DCSAPI.Parameters)
                    {
                        var label = new Label
                        {
                            Content = dcsAPIParameter.ParameterName.Replace("_", "__"),
                            VerticalAlignment = VerticalAlignment.Center
                        };
                        controlList.Add(label);

                        if (dcsAPIParameter.ParameterName.StartsWith('i'))
                        {
                            var commands = LoSetCommand.LoadCommands();
                            _textBoxSearchICommand = new TextBox
                            {
                                Name = "TextBox" + dcsAPIParameter.Id,
                                Tag = dcsAPIParameter.Id,
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
                            var textBoxParameter = GetTextBoxParameter(dcsAPIParameter);
                            controlList.Add(textBoxParameter);
                            TextBoxParameterList.Add(textBoxParameter);

                            controlList.Add(textBoxParameter);
                            TextBoxParameterList.Add(textBoxParameter);
                        }
                    }

                    controlList.Add(GetButtonSend());

                    if (DCSAPI.ReturnsData)
                    {
                        controlList.Add(GetLabelKeepResults());
                        controlList.Add(GetCheckBoxKeepResults());
                        controlList.Add(GetLabelPolling());
                        controlList.Add(GetCheckBoxPolling());
                        controlList.Add(GetLabelPollingInterval());
                        controlList.Add(GetComboBoxPollTimes());
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
                if (_textBoxSearchICommand == null) return;

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
                if (_textBoxSearchICommand == null) return;

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
                if (_textBoxSearchICommand == null) return;

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
                
                TextBoxSearchLoSetCommands.AdjustShownPopupData((TextBox)sender, _popupSearchICommand, _dataGridValues, _loSetICommands);
                SetFormState();
            }
            catch (Exception ex)
            {
                Common.ShowErrorMessageBox(ex);
            }
        }

        private void TextBoxSearchICommand_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            TextBoxSearchLoSetCommands.SetBackgroundSearchBanner((TextBox)sender);
        }
        
        private void TextBoxSearchICommand_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            TextBoxSearchLoSetCommands.HandleFirstSpace(sender, e);
        }
    }
}
