using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using NLog;

namespace DCSInsight.Misc
{
    internal static class Common
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        internal static bool LuaConsoleIsLoaded;
        internal static bool LuaConsoleSearchWarningGiven;
        public static MouseEventHandler MouseEnter => UIElement_OnMouseEnterHandIcon;
        public static MouseEventHandler MouseLeave => UIElement_OnMouseLeaveNormalIcon;

        internal static void ShowMessageBox(string message)
        {
            MessageBox.Show(message, "", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        internal static string GetApplicationPath()
        {
            return AppDomain.CurrentDomain.BaseDirectory;
        }

        internal static void ShowErrorMessageBox(Exception ex, string? message = null)
        {
            if(message != null) Logger.Error(ex, message);

            MessageBox.Show(ex.Message, $"Details logged to error log.{Environment.NewLine}{ex.Source}", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        /// <summary>
        /// Checks the setting "JSON Directory". This folder must contain all the JSON files
        /// </summary>
        /// <param name="jsonDirectory"></param>
        /// <returns>
        /// 0, 0 : folder not found, json not found
        /// 1, 0 : folder found, json not found
        /// 1, 1 : folder found, json found
        /// </returns>
        internal static Tuple<bool, bool> CheckJSONDirectory(string jsonDirectory)
        {
            if (string.IsNullOrEmpty(jsonDirectory) || !Directory.Exists(jsonDirectory))
            {
                /*
                 * Folder not found
                 */
                return new Tuple<bool, bool>(false, false);
            }

            var files = Directory.EnumerateFiles(jsonDirectory);

            /*
             * This is not optimal, the thing is that there is no single file to rely
             * on in order to determine that this folder is the DCS-BIOS JSON directory.
             * Files can be changed (although rare) but it cannot be taken for certain
             * that this doesn't happen.
             *
             * The solution is to count the number of json files in the folder.
             * This gives a fairly certain indication that the folder is in fact
             * the JSON folder. There are JSON files in other folders but not many.
             */
            var jsonFound = files.Count(filename => filename.ToLower().EndsWith(".json")) >= 10;

            return new Tuple<bool, bool>(true, jsonFound);
        }
        
        internal static void UIElement_OnMouseEnterHandIcon(object sender, MouseEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Hand;
        }

        internal static void UIElement_OnMouseLeaveNormalIcon(object sender, MouseEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Arrow;
        }

        internal static T? FindVisualChild<T>(DependencyObject? dependencyObject) where T : DependencyObject
        {
            if (dependencyObject == null) return null;

            for (var i = 0; i < VisualTreeHelper.GetChildrenCount(dependencyObject); i++)
            {
                var child = VisualTreeHelper.GetChild(dependencyObject, i);
                
                if (child is T o)
                {
                    return o;
                }

                var childItem = FindVisualChild<T>(child);
                if (childItem != null) return childItem;
            }
            return null;
        }

        /// <summary>
        /// Finds a Child of a given item in the visual tree. 
        /// </summary>
        /// <param name="parent">A direct parent of the queried item.</param>
        /// <typeparam name="T">The type of the queried item.</typeparam>
        /// <param name="childName">x:Name or Name of child. </param>
        /// <returns>The first parent item that matches the submitted type parameter. 
        /// If not matching item can be found, 
        /// a null parent is being returned.</returns>
        internal static T? FindChild<T>(this DependencyObject? parent, string childName) where T : DependencyObject
        {
            // Confirm parent and childName are valid. 
            if (parent == null) return null;

            T? foundChild = null;

            var childrenCount = VisualTreeHelper.GetChildrenCount(parent);
            for (var i = 0; i < childrenCount; i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                // If the child is not of the request child type child
                if (child is not T childType)
                {
                    // recursively drill down the tree
                    foundChild = FindChild<T>(child, childName);

                    // If the child is found, break, so we do not overwrite the found child. 
                    if (foundChild != null) break;
                }
                else if (!string.IsNullOrEmpty(childName))
                {
                    // If the child's name is set for search
                    if (childType is not FrameworkElement frameworkElement || frameworkElement.Name != childName) continue;
                    // if the child's name is of the request name
                    foundChild = childType;
                    break;
                }
                else
                {
                    // child element found.
                    foundChild = childType;
                    break;
                }
            }

            return foundChild;
        }

        internal static bool TryFindVisualChildByName<TChild>(this DependencyObject parent, string childElementName, out TChild? childElement, bool isCaseSensitive = false)
            where TChild : FrameworkElement
        {
            childElement = null;

            // Popup.Child content is not part of the visual tree.
            // To prevent traversal from breaking when parent is a Popup,
            // we need to explicitly extract the content.
            if (parent is Popup popup)
            {
                parent = popup.Child;
            }

            if (parent == null)
            {
                return false;
            }

            var stringComparison = isCaseSensitive
                ? StringComparison.Ordinal
                : StringComparison.OrdinalIgnoreCase;

            for (var i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                if (child is TChild resultElement
                    && resultElement.Name.Equals(childElementName, stringComparison))
                {
                    childElement = resultElement;
                    return true;
                }

                if (child.TryFindVisualChildByName(childElementName, out childElement))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
