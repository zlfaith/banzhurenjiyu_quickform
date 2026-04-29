using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;

namespace DesktopMessageApp
{
    public partial class SettingsWindow : Window
    {
        public List<string> Messages { get; set; }
        public DateTime TargetDate { get; set; }
        public string CountdownName { get; set; }
        public new double FontSize { get; set; }
        public double CountdownFontSize { get; set; }
        public string TextAlignment { get; set; }

        public SettingsWindow(List<string> currentMessages, DateTime currentTargetDate, string currentCountdownName, double currentFontSize, double currentCountdownFontSize, string currentTextAlignment)
        {
            InitializeComponent();
            Messages = currentMessages ?? new List<string>();
            TargetDate = currentTargetDate;
            CountdownName = currentCountdownName ?? "目标日期";
            FontSize = currentFontSize > 0 ? currentFontSize : 18;
            CountdownFontSize = currentCountdownFontSize > 0 ? currentCountdownFontSize : 20;
            TextAlignment = currentTextAlignment ?? "Left";
            MessageTextBox.Text = string.Join(Environment.NewLine, Messages);
            CountdownNameTextBox.Text = CountdownName;
            FontSizeSlider.Value = FontSize;
            FontSizeValueText.Text = FontSize.ToString();
            CountdownFontSizeSlider.Value = CountdownFontSize;
            CountdownFontSizeValueText.Text = CountdownFontSize.ToString();
            
            switch (TextAlignment)
            {
                case "Center":
                    CenterAlignRadio.IsChecked = true;
                    break;
                case "Right":
                    RightAlignRadio.IsChecked = true;
                    break;
                default:
                    LeftAlignRadio.IsChecked = true;
                    break;
            }
            
            if (TargetDate != default)
            {
                TargetDatePicker.SelectedDate = TargetDate;
            }
            
            FontSizeSlider.ValueChanged += (s, e) => FontSizeValueText.Text = FontSizeSlider.Value.ToString();
            CountdownFontSizeSlider.ValueChanged += (s, e) => CountdownFontSizeValueText.Text = CountdownFontSizeSlider.Value.ToString();
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            string[] lines = MessageTextBox.Text.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
            Messages = new List<string>(lines);
            CountdownName = CountdownNameTextBox.Text.Trim();
            if (string.IsNullOrEmpty(CountdownName))
            {
                CountdownName = "目标日期";
            }
            FontSize = FontSizeSlider.Value;
            CountdownFontSize = CountdownFontSizeSlider.Value;
            
            if (CenterAlignRadio.IsChecked == true)
            {
                TextAlignment = "Center";
            }
            else if (RightAlignRadio.IsChecked == true)
            {
                TextAlignment = "Right";
            }
            else
            {
                TextAlignment = "Left";
            }
            
            if (TargetDatePicker.SelectedDate.HasValue)
            {
                TargetDate = TargetDatePicker.SelectedDate.Value;
            }
            DialogResult = true;
            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void Border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }
    }
}