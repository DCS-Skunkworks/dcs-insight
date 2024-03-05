using DCSInsight.Communication;
using DCSInsight.Misc;
using System;
using System.Windows;
using System.Windows.Forms;
using DCSInsight.Properties;

namespace DCSInsight.Windows
{
    /// <summary>
    /// Interaction logic for WindowAskReloadAPIDialog.xaml
    /// </summary>
    public partial class WindowAskReloadAPIDialog
    {
        public DialogResult DialogResult { get; set; }

        public WindowAskReloadAPIDialog()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
            }
            catch (Exception ex)
            {
                Common.ShowErrorMessageBox(ex);
            }
        }

        private void ButtonYes_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                Settings.Default.AskForReloadAPIList = !CheckBoxDoNotAskAgain.IsChecked == true;
                Settings.Default.ReloadAPIList = true;
                Settings.Default.Save();
                DialogResult = DialogResult.Yes;
                Close();
            }
            catch (Exception ex)
            {
                Common.ShowErrorMessageBox(ex);
            }
        }

        private void ButtonNo_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                Settings.Default.AskForReloadAPIList = !CheckBoxDoNotAskAgain.IsChecked == true;
                Settings.Default.ReloadAPIList = false;
                Settings.Default.Save();
                DialogResult = DialogResult.No;
                Close();
            }
            catch (Exception ex)
            {
                Common.ShowErrorMessageBox(ex);
            }
        }
    }
}
