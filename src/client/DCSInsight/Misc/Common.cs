using System;
using System.Windows;
using NLog;

namespace DCSInsight.Misc
{
    public static class Common
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public static void ShowMessageBox(string message)
        {
            MessageBox.Show(message, "", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        public static string GetApplicationPath()
        {
            return AppDomain.CurrentDomain.BaseDirectory;
        }

        public static void ShowErrorMessageBox(Exception ex, string message = null)
        {
            Logger.Error(ex, message);
            MessageBox.Show(ex.Message, $"Details logged to error log.{Environment.NewLine}{ex.Source}", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
