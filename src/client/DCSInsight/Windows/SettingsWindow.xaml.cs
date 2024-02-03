using System;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using DCSInsight.Misc;
using DCSInsight.Properties;
using System.Windows.Forms;

namespace DCSInsight.Windows
{
    /// <summary>
    /// Interaction logic for SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow
    {
        public string DcsBiosJSONLocation { get; private set; }
        private bool _isLoaded;
        public bool DCSBIOSChanged { get; private set; } = false;

        public SettingsWindow()
        {
            InitializeComponent();
        }

        private void SetFormState()
        {
            CheckDCSBIOSStatus();
        }

        private void SettingsWindow_OnLoaded(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_isLoaded)
                {
                    return;
                }

                Mouse.OverrideCursor = System.Windows.Input.Cursors.Arrow;
                ButtonOk.IsEnabled = false;
                LoadSettings();
                SetFormState();
                _isLoaded = true;
            }
            catch (Exception exception)
            {
                System.Windows.MessageBox.Show(exception.Message + Environment.NewLine + exception.StackTrace);
            }
        }

        private void CheckDCSBIOSStatus()
        {
            var result = Common.CheckJSONDirectory(Environment.ExpandEnvironmentVariables(TextBoxDcsBiosJSONLocation.Text));
            ButtonOk.IsEnabled = false;

            if (result.Item1 == false && result.Item2 == false)
            {
                LabelDCSBIOSNotFound.Foreground = Brushes.Red;
                LabelDCSBIOSNotFound.Content = "<-- Warning, folder does not exist.";
                return;
            }

            if (result.Item1 && result.Item2 == false)
            {
                LabelDCSBIOSNotFound.Foreground = Brushes.Red;
                LabelDCSBIOSNotFound.Content = "<-- Warning, folder does not contain JSON files.";
                return;
            }
            

            LabelDCSBIOSNotFound.Foreground = Brushes.LimeGreen;
            LabelDCSBIOSNotFound.Content = " JSON files found.";
            ButtonOk.IsEnabled = true;
        }

        private void LoadSettings()
        {
            TextBoxDcsBiosJSONLocation.Text = Settings.Default.DCSBiosJSONLocation;
        }

        private void ButtonCancel_OnClick(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void ButtonOk_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                CheckValuesDCSBIOS();
                
                if (DCSBIOSChanged)
                {
                    Settings.Default.DCSBiosJSONLocation = Environment.ExpandEnvironmentVariables(TextBoxDcsBiosJSONLocation.Text);
                    Settings.Default.Save();
                }
                DialogResult = true;
                Close();
            }
            catch (Exception exception)
            {
                System.Windows.MessageBox.Show($"{exception.Message}{Environment.NewLine}{exception.StackTrace}");
            }
        }

        private void ButtonBrowse_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                var folderBrowserDialog = new FolderBrowserDialog()
                {
                    ShowNewFolderButton = false,
                };
                
                var folderLocation = Settings.Default.DCSBiosJSONLocation;
                if (!string.IsNullOrEmpty(folderLocation))
                {
                    folderBrowserDialog.SelectedPath = folderLocation;
                    folderBrowserDialog.InitialDirectory = folderLocation;
                }

                if (folderBrowserDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    Debug.WriteLine("Selected Path : " + folderBrowserDialog.SelectedPath);
                    var result = Common.CheckJSONDirectory(folderBrowserDialog.SelectedPath);
                    if (result.Item1 && result.Item2)
                    {
                        TextBoxDcsBiosJSONLocation.Text = folderBrowserDialog.SelectedPath;
                    }
                    else if (result.Item1 && result.Item2 == false)
                    {
                        System.Windows.MessageBox.Show("Cannot use selected directory as it did not contain JSON files.", "Invalid directory", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }

                SetFormState();
            }
            catch (Exception ex)
            {
                Common.ShowErrorMessageBox(ex);
            }
        }

        private void CheckValuesDCSBIOS()
        {
            try
            {
                if (string.IsNullOrEmpty(TextBoxDcsBiosJSONLocation.Text))
                {
                    throw new Exception("DCS-BIOS JSON directory cannot be empty");
                }
                try
                {
                    var directoryInfo = new DirectoryInfo(TextBoxDcsBiosJSONLocation.Text);
                    DcsBiosJSONLocation = TextBoxDcsBiosJSONLocation.Text;
                }
                catch (Exception ex)
                {
                    throw new Exception($"DCS-BIOS Error while checking DCS-BIOS location : {ex.Message}");
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"DCS-BIOS Error checking values : {Environment.NewLine}{ex.Message}");
            }
        }

        private void DcsBiosDirty(object sender, TextChangedEventArgs e)
        {
            DCSBIOSChanged = true;
            ButtonOk.IsEnabled = true;
        }

        private void SettingsWindow_OnKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (!ButtonOk.IsEnabled && e.Key == Key.Escape)
            {
                DialogResult = false;
                e.Handled = true;
                Close();
            }
        }

    }
}
