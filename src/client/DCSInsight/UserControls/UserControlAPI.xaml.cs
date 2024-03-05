using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using DCSInsight.JSON;
using DCSInsight.Misc;
using Application = System.Windows.Application;

namespace DCSInsight.UserControls
{
    /// <summary>
    /// Interaction logic for UserControlAPI.xaml
    /// </summary>
    public partial class UserControlAPI : UserControlAPIBase
    {

        public UserControlAPI(DCSAPI dcsAPI, bool isConnected) : base(dcsAPI, isConnected)
        {
            InitializeComponent();
            LabelResultBase = LabelResult;
            TextBoxResultBase = TextBoxResult;
            
        }

        private void UserControlAPI_OnLoaded(object sender, RoutedEventArgs e)
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
                if (ButtonSend == null) throw new Exception("ButtonSend is null.");

                ButtonSend.IsEnabled = !TextBoxParameterList.Any(o => string.IsNullOrEmpty(o.Text)) && IsConnected;

                if (DCSAPI.ReturnsData && !IsLuaConsole)
                {
                    if (ComboBoxPollTimes == null || CheckBoxPolling == null) throw new Exception("ComboBoxPollTimes or CheckBoxPolling is null.");

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
            if (IsLuaConsole)
            {
                BuildLuaConsoleUI();
                return;
            }
            BuildGenericUI();
        }

        private void BuildLuaConsoleUI()
        {
            try
            {
                Mouse.OverrideCursor = Cursors.Wait;

                try
                {
                    TextBoxSyntax.Text = DCSAPI.Syntax;
                    TextBoxSyntax.MouseEnter -= Common.MouseEnter;
                    TextBoxSyntax.MouseLeave -= Common.MouseLeave;

                    StackPanelBottom.Visibility = Visibility.Visible;
                    var dockPanelParameters = Application.Current.MainWindow.FindChild<DockPanel>("DockPanelParameters") ?? throw new Exception("Failed to find DockPanelParameters");
                    dockPanelParameters.LastChildFill = true;
                    var controlList = new List<Control>();

                    var textBoxLuaCode = new TextBox
                    {
                        Name = "TextBox0", //only one parameter for Lua Console
                        Tag = 0,
                        MinWidth = 550,
                        Height = 20,
                        IsTabStop = true,
                        FontFamily = new FontFamily("Consolas"),
                        TextWrapping = TextWrapping.Wrap,
                        AcceptsReturn = true,
                        AcceptsTab = true,
                        Width = double.NaN,
                        MinHeight = 150,
                        VerticalScrollBarVisibility = ScrollBarVisibility.Auto
                    };

                    textBoxLuaCode.PreviewKeyDown += TextBoxLuaCode_OnPreviewKeyDown;

                    var brushConverter = new BrushConverter().ConvertFromString("#0000FF");
                    var labelConsoleWarning = new Label
                    {
                        Content = "[warning]"
                    };
                    if (brushConverter != null)
                    {
                        labelConsoleWarning.Foreground = (SolidColorBrush)brushConverter;
                    }
                    labelConsoleWarning.MouseEnter += Common.UIElement_OnMouseEnterHandIcon;
                    labelConsoleWarning.MouseLeave += Common.UIElement_OnMouseLeaveNormalIcon;
                    labelConsoleWarning.MouseDown += LabelConsoleWarningOnMouseDown;

                    void LabelConsoleWarningOnMouseDown(object sender, MouseButtonEventArgs e)
                    {
                        MessageBox.Show("WARNING! This function enables arbitrary lua code to be\r\nexecuted on your computer, including calls to package os (Operating System).\r\nDo NOT enable unless your are firewalled.", "Warning", MessageBoxButton.OK);
                    }

                    labelConsoleWarning.Tag = textBoxLuaCode;
                    StackPanelLinks.Children.Add(labelConsoleWarning);

                    var labelDefaultLua = new Label
                    {
                        Content = "[list environment]"
                    };
                    if (brushConverter != null)
                    {
                        labelDefaultLua.Foreground = (SolidColorBrush)brushConverter;
                    }
                    labelDefaultLua.MouseEnter += Common.UIElement_OnMouseEnterHandIcon;
                    labelDefaultLua.MouseLeave += Common.UIElement_OnMouseLeaveNormalIcon;
                    labelDefaultLua.MouseDown += LabelDefaultLuaOnMouseDown;

                    void LabelDefaultLuaOnMouseDown(object sender, MouseButtonEventArgs e)
                    {
                        try
                        {
                            var callingTextBox = (TextBox)((Label)sender).Tag;
                            callingTextBox.Text = Constants.ListEnvironmentSnippet;
                            SetFormState();
                            if (ButtonSend is { IsEnabled: true })
                            {
                                SendCommand();
                            }
                        }
                        catch (Exception ex)
                        {
                            Common.ShowErrorMessageBox(ex);
                        }
                    }

                    labelDefaultLua.Tag = textBoxLuaCode;
                    StackPanelLinks.Children.Add(labelDefaultLua);
                    
                    controlList.Add(textBoxLuaCode);
                    TextBoxParameterList.Add(textBoxLuaCode);

                    StackPanelBottom.Children.Add(GetButtonSend());

                    ItemsControlParameters.ItemsSource = controlList;
                    Common.LuaConsoleIsLoaded = true;
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

        private void BuildGenericUI()
        {
            try
            {
                Mouse.OverrideCursor = Cursors.Wait;

                try
                {
                    TextBoxSyntax.Text = DCSAPI.Syntax;
                    TextBoxSyntax.ToolTip = $"Click to copy syntax. (API Id = {DCSAPI.Id})";
                    StackPanelBottom.Visibility = Visibility.Collapsed;

                    var controlList = new List<Control>();

                    foreach (var dcsAPIParameter in DCSAPI.Parameters)
                    {
                        controlList.Add(GetLabelParameterName(dcsAPIParameter.ParameterName));

                        var textBoxParameter = GetTextBoxParameter(dcsAPIParameter);
                        controlList.Add(textBoxParameter);
                        TextBoxParameterList.Add(textBoxParameter);
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
    }
}
