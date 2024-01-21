using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Media;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Shapes;
using ControlReference.CustomControls;
using DCSInsight.Lua;
using DCSInsight.Misc;
using DCSInsight.Properties;
using Clipboard = System.Windows.Clipboard;
using Cursors = System.Windows.Input.Cursors;
using KeyEventArgs = System.Windows.Input.KeyEventArgs;
using MouseEventArgs = System.Windows.Input.MouseEventArgs;

namespace DCSInsight.Windows
{
    /// <summary>
    /// Interaction logic for LuaWindow.xaml
    /// </summary>
    public partial class LuaWindow
    {
        private bool _isLoaded;
        private List<string> _aircraftList = new();
        private List<KeyValuePair<string, string>> _luaControls = new();
        private TextBlockSelectable _textBlockSelectable;
        private Popup _popupSearch;
        private DataGrid _dataGridValues;

        public LuaWindow()
        {
            InitializeComponent();
        }

        private void SetFormState()
        {
        }

        private void LuaWindow_OnLoaded(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_isLoaded)
                {
                    return;
                }

                _textBlockSelectable = new TextBlockSelectable("");
                _textBlockSelectable.MouseEnter += TextBlock_OnMouseEnter;
                _textBlockSelectable.MouseLeave += TextBlock_OnMouseLeave;
                SetContextMenu(_textBlockSelectable);

                _popupSearch = (Popup)FindResource("PopUpSearchResults");
                _popupSearch.Height = 400;
                _dataGridValues = (DataGrid)LogicalTreeHelper.FindLogicalNode(_popupSearch, "DataGridValues");

                _textBlockSelectable.FontFamily = new System.Windows.Media.FontFamily("Consolas");
                _textBlockSelectable.Width = Double.NaN;
                
                var border = new Border
                {
                    Child = _textBlockSelectable
                };
                StackPanelLuaCommand.Children.Add(border);
                StackPanelLuaCommand.Children.Add(new Line());

                StackPanelLuaCommand.UpdateLayout();

                LoadAircraft();

                SetFormState();

                TextBoxSearch.Focus();

                Top = Settings.Default.LuaWindowTop.CompareTo(-1) == 0 ? Top : Settings.Default.LuaWindowTop;
                Left = Settings.Default.LuaWindowTop.CompareTo(-1) == 0 ? Left : Settings.Default.LuaWindowTop;

                _isLoaded = true;
            }
            catch (Exception exception)
            {
                Common.ShowErrorMessageBox(exception);
            }
        }

        private void LuaWindow_OnKeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Key == Key.Escape)
                {
                    Close();
                }
            }
            catch (Exception exception)
            {
                Common.ShowMessageBox(exception.Message + Environment.NewLine + exception.StackTrace);
            }
        }
        
        private void TextBlock_OnMouseEnter(object sender, MouseEventArgs e)
        {
            try
            {
                Mouse.OverrideCursor = Cursors.IBeam;
            }
            catch (Exception exception)
            {
                Common.ShowMessageBox(exception.Message + Environment.NewLine + exception.StackTrace);
            }
        }

        private void TextBlock_OnMouseLeave(object sender, MouseEventArgs e)
        {
            try
            {
                Mouse.OverrideCursor = Cursors.Arrow;
            }
            catch (Exception exception)
            {
                Common.ShowMessageBox(exception.Message + Environment.NewLine + exception.StackTrace);
            }
        }
        
        private void SetContextMenu(TextBlockSelectable textBlock)
        {
            try
            {
                ContextMenu contextMenu = new();
                contextMenu.Opened += TextBlock_ContextMenuOpened;
                contextMenu.Tag = textBlock;
                var menuItemCopy = new MenuItem
                {
                    Tag = textBlock,
                    Header = "Copy"
                };
                menuItemCopy.Click += MenuItemCopy_OnClick;
                contextMenu.Items.Add(menuItemCopy);
                textBlock.ContextMenu = contextMenu;
            }
            catch (Exception exception)
            {
                Common.ShowMessageBox(exception.Message + Environment.NewLine + exception.StackTrace);
            }
        }

        private void TextBlock_ContextMenuOpened(object sender, RoutedEventArgs e)
        {
            try
            {
                var contextMenu = (ContextMenu)sender;
                var textBlock = (TextBlockSelectable)contextMenu.Tag;
                ((MenuItem)contextMenu.Items[0]).IsEnabled = !string.IsNullOrEmpty(textBlock.SelectedText);
            }
            catch (Exception ex)
            {
                Common.ShowErrorMessageBox(ex);
            }
        }

        private void MenuItemCopy_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                var textBlock = ((MenuItem)sender).Tag;
                CopyToClipboard((TextBlockSelectable)textBlock);
            }
            catch (Exception exception)
            {
                Common.ShowMessageBox(exception.Message + Environment.NewLine + exception.StackTrace);
            }
        }

        private void CopyToClipboard(TextBlockSelectable textBlock)
        {
            if (string.IsNullOrEmpty(textBlock.SelectedText)) textBlock.SelectAll();

            Clipboard.SetText(textBlock.SelectedText ?? "");
            SystemSounds.Asterisk.Play();
        }

        private void LoadAircraft()
        {
            _aircraftList = LuaAssistant.GetAircraftList(Settings.Default.DCSBiosJSONLocation);
            ComboBoxAircraft.DataContext = _aircraftList;
            ComboBoxAircraft.ItemsSource = _aircraftList;
            ComboBoxAircraft.Items.Refresh();

            if (_aircraftList.Count <= 0) return;
            ComboBoxAircraft.SelectionChanged += ComboBoxAircraft_OnSelectionChanged;
            ComboBoxAircraft.SelectedIndex = 0;
        }

        private void LoadLuaControls(string aircraftId)
        {
            ComboBoxLuaControls.SelectionChanged -= ComboBoxLuaControls_OnSelectionChanged;
            _luaControls = LuaAssistant.GetLuaControls(aircraftId);
            ComboBoxLuaControls.DataContext = _luaControls;
            ComboBoxLuaControls.ItemsSource = _luaControls;
            ComboBoxLuaControls.DisplayMemberPath = "Key";
            ComboBoxLuaControls.Items.Refresh();

            if (_luaControls.Count > 0)
            {
                ComboBoxLuaControls.SelectionChanged += ComboBoxLuaControls_OnSelectionChanged;
                ComboBoxLuaControls.SelectedIndex = 0;
            }
        }

        private void ComboBoxAircraft_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                LoadLuaControls((string)ComboBoxAircraft.SelectedItem);
            }
            catch (Exception exception)
            {
                Common.ShowErrorMessageBox(exception);
            }
        }

        private void ComboBoxLuaControls_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                var luaControl = (KeyValuePair<string,string>)ComboBoxLuaControls.SelectedItem;

                var luaSignatures = LuaAssistant.GetModuleFunctionSignatures();

                //A_10C:definePotentiometer("HARS_LATITUDE", 44, 3005, 271, { 0, 1 }, "HARS", "HARS Latitude Dial")
                var startIndex = luaControl.Value.IndexOf(":", StringComparison.Ordinal);
                var endIndex = luaControl.Value.IndexOf("(", StringComparison.Ordinal) - startIndex;
                var functionName = "function Module" + luaControl.Value.Substring(startIndex, endIndex);

                var luaSignature = luaSignatures.Find(o => o.StartsWith(functionName + "("));
                _textBlockSelectable.Text = string.IsNullOrEmpty(luaSignature) ? luaControl.Value : $"{luaSignature.Replace("function ","")}\n{luaControl.Value}";
            }
            catch (Exception exception)
            {
                Common.ShowErrorMessageBox(exception);
            }
        }

        private void TextBoxSearch_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            TextBoxSearchLoSetCommands.SetBackgroundSearchBanner(TextBoxSearch);
        }

        private void TextBoxSearch_OnKeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                TextBoxSearchLuaControls.AdjustShownPopupData(TextBoxSearch, _popupSearch, _dataGridValues, _luaControls);
                SetFormState();
            }
            catch (Exception ex)
            {
                Common.ShowErrorMessageBox(ex);
            }
        }

        private void TextBoxSearch_OnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            TextBoxSearchLuaControls.HandleFirstSpace(sender, e);
        }

        private void LuaWindow_OnClosing(object sender, CancelEventArgs e)
        {
            try
            {
                Settings.Default.LuaWindowTop = Top;
                Settings.Default.LuaWindowLeft = Left;
                Settings.Default.Save();  
            }
            catch (Exception ex)
            {
                Common.ShowErrorMessageBox(ex);
            }
        }
        
        private void DataGridValues_OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            LuaControlSelected(true);
        }

        private void DataGridValues_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            LuaControlSelected(true);
        }
        
        private void DataGridValues_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            LuaControlSelected(false);
        }

        private void LuaControlSelected(bool keepSearchOpen)
        {
            try
            {
                if (_dataGridValues.SelectedItems.Count == 1)
                {
                    var keyValuePair = (KeyValuePair<string,string>)_dataGridValues.SelectedItem;
                    ComboBoxLuaControls.SelectedItem = keyValuePair;
                    SetFormState();
                }
                _popupSearch.IsOpen = keepSearchOpen;
                SetFormState();
            }
            catch (Exception ex)
            {
                Common.ShowErrorMessageBox(ex);
            }
        }
    }
}
