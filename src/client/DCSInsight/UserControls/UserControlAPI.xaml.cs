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
                ButtonSend.IsEnabled = !TextBoxParameterList.Any(o => string.IsNullOrEmpty(o.Text)) && IsConnected;

                if (DCSAPI.ReturnsData && !IsLuaConsole)
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
                    TextBoxSyntax.ToolTip = $"Click to copy syntax. (API Id = {DCSAPI.Id})";
                    StackPanelBottom.Visibility = Visibility.Visible;
                    Application.Current.MainWindow.FindChild<DockPanel>("DockPanelParameters").LastChildFill = true;
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

                    TextBoxSyntax.PreviewMouseDown -= TextBoxSyntax_OnPreviewMouseDown;
                    TextBoxSyntax.MouseEnter -= TextBoxSyntax_OnMouseEnter;
                    TextBoxSyntax.MouseLeave -= TextBoxSyntax_OnMouseLeave;
                    TextBoxSyntax.ToolTip = null;


                    var labelConsoleWarning = new Label
                    {
                        Content = "[warning]",
                        Foreground = (SolidColorBrush)new BrushConverter().ConvertFromString("#0000FF")
                    };
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
                        Content = "[list environment]",
                        Foreground = (SolidColorBrush)new BrushConverter().ConvertFromString("#0000FF"),
                    };
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
                            if (ButtonSend.IsEnabled)
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

                    textBoxLuaCode.KeyUp += TextBoxParameter_OnKeyUp;

                    controlList.Add(textBoxLuaCode);
                    TextBoxParameterList.Add(textBoxLuaCode);

                    ButtonSend = new Button
                    {
                        Content = "Send",
                        Height = 20,
                        Width = 50,
                        VerticalAlignment = VerticalAlignment.Center,
                        Margin = new Thickness(20, 0, 0, 0)
                    };
                    ButtonSend.Click += ButtonSend_OnClick;

                    StackPanelBottom.Children.Add(ButtonSend);

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

                    foreach (var dcsAPIParameterType in DCSAPI.Parameters)
                    {
                        var label = new Label
                        {
                            Content = dcsAPIParameterType.ParameterName.Replace("_", "__"),
                            VerticalAlignment = VerticalAlignment.Center
                        };
                        controlList.Add(label);


                        var textBoxParameter = new TextBox
                        {
                            Name = "TextBox" + dcsAPIParameterType.Id,
                            Tag = dcsAPIParameterType.Id,
                            MinWidth = 50,
                            Height = 20,
                            IsTabStop = true
                        };

                        if (dcsAPIParameterType.Type == ParameterTypeEnum.number)
                        {
                            textBoxParameter.KeyDown += TextBoxParameter_OnKeyDown_Number;
                        }
                        textBoxParameter.KeyUp += TextBoxParameter_OnKeyUp;

                        controlList.Add(textBoxParameter);
                        TextBoxParameterList.Add(textBoxParameter);
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
                            VerticalAlignment = VerticalAlignment.Center,
                        };
                        ComboBoxPollTimes.DataContextChanged += ComboBoxPollTimes_OnDataContextChanged;
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
    }
}
