using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace DCSInsight.Misc
{
    internal static class TextBoxSearchLuaControls
    {
        internal static void HandleTyping(TextBox textBoxSearch)
        {
            try
            {
                if (string.IsNullOrEmpty(textBoxSearch.Text))
                {
                    textBoxSearch.Background = null; 
                    return;
                }

                SetBackgroundSearchBanner(textBoxSearch);
            }
            catch (Exception ex)
            {
                Common.ShowErrorMessageBox(ex);
            }
        }

        internal static void SetBackgroundSearchBanner(TextBox textBoxSearch)
        {
            try
            {
                //new Uri(@"/dcs-insight;component/Images/cue_banner_search.png", UriKind.Relative)),
                if (string.IsNullOrEmpty(textBoxSearch.Text))
                {
                    // Create an ImageBrush.
                    var textImageBrush = new ImageBrush
                    {
                        ImageSource = new BitmapImage(
                            new Uri("pack://application:,,,/dcs-insight;component/Images/cue_banner_search.png", UriKind.RelativeOrAbsolute)),
                        AlignmentX = AlignmentX.Left,
                        Stretch = Stretch.Uniform
                    };

                    // Use the brush to paint the button's background.
                    textBoxSearch.Background = textImageBrush;
                }
                else
                {
                    textBoxSearch.Background = null;
                }
            }
            catch (Exception ex)
            {
                Common.ShowErrorMessageBox(ex);
            }
        }

        internal static void AdjustShownPopupData(TextBox textBoxSearch, Popup popupSearch, DataGrid dataGridValues, IEnumerable<KeyValuePair<string,string>> luaControls)
        {
            try
            {
                popupSearch.PlacementTarget = textBoxSearch;
                popupSearch.Placement = PlacementMode.Bottom;
                dataGridValues.Tag = textBoxSearch;
                if (!popupSearch.IsOpen)
                {
                    popupSearch.IsOpen = true;
                }

                if (string.IsNullOrEmpty(textBoxSearch.Text))
                {
                    dataGridValues.DataContext = luaControls;
                    dataGridValues.ItemsSource = luaControls;
                    dataGridValues.Items.Refresh();
                    return;
                }
                var subList = luaControls.Where(luaControl => !string.IsNullOrWhiteSpace(luaControl.Value) && 
                                                                   luaControl.Value.ToUpper().Contains(textBoxSearch.Text.ToUpper()));
                dataGridValues.DataContext = subList;
                dataGridValues.ItemsSource = subList;
                dataGridValues.Items.Refresh();
            }
            catch (Exception ex)
            {
                Common.ShowErrorMessageBox(ex, "AdjustShownPopupData()");
            }
        }

        internal static void HandleFirstSpace(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space && ((TextBox)sender).Text == "")
            {
                e.Handled = true;
            }
        }
    }
}
