using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using System.Threading.Tasks;
using WinForms = System.Windows.Forms;
using System.Drawing;

namespace DesktopMessageApp
{
    public partial class MainWindow : Window
    {
        private DateTime targetDate;
        private List<string> messages;
        private string countdownName;
        private double fontSize;
        private double countdownFontSize;
        private string textAlignment;
        private DispatcherTimer timer;
        private DispatcherTimer updateCheckTimer;
        private string configPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "班主任寄语与倒计时", "config.txt");
        private QuickFormClient quickFormClient;
        private const string ApiUrl = "https://quickform.cn/api/qvh8vbxcnt";
        private WinForms.NotifyIcon notifyIcon;

        public MainWindow()
        {
            InitializeComponent();
            messages = new List<string>();
            countdownName = "目标日期";
            fontSize = 18;
            countdownFontSize = 20;
            textAlignment = "Left";
            quickFormClient = new QuickFormClient(ApiUrl, configPath);
            LoadConfig();
            StartTimer();
            StartUpdateCheckTimer();
            UpdateCountdown();
            UpdateMessage();
            CheckForUpdatesAsync();
            
            this.ShowInTaskbar = false;
            this.Topmost = false;
            CreateNotifyIcon();
            this.Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            SendWindowToDesktopBackground();
        }

        private void SendWindowToDesktopBackground()
        {
            try
            {
                IntPtr hwnd = new System.Windows.Interop.WindowInteropHelper(this).Handle;
                IntPtr hDesktop = FindWindow("Progman", "Program Manager");
                if (hDesktop != IntPtr.Zero)
                {
                    IntPtr hShellView = FindWindowEx(hDesktop, IntPtr.Zero, "SHELLDLL_DefView", null);
                    if (hShellView != IntPtr.Zero)
                    {
                        hDesktop = FindWindowEx(hShellView, IntPtr.Zero, "SysListView32", null);
                    }
                    SetParent(hwnd, hDesktop);
                }
            }
            catch { }
        }

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern IntPtr SetParent(IntPtr hWndChild, IntPtr hWndNewParent);

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern IntPtr FindWindowEx(IntPtr hWndParent, IntPtr hWndChildAfter, string lpClassName, string lpWindowName);

        private void CreateNotifyIcon()
        {
            try
            {
                notifyIcon = new WinForms.NotifyIcon();
                notifyIcon.Icon = SystemIcons.Information;
                notifyIcon.Visible = true;
                notifyIcon.Text = "班主任寄语与倒计时";
                
                WinForms.ContextMenuStrip menu = new WinForms.ContextMenuStrip();
                menu.Items.Add("关闭", null, (s, e) => this.Close());
                notifyIcon.ContextMenuStrip = menu;
            }
            catch { }
        }

        private void StartTimer()
        {
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += (sender, e) => 
            {
                UpdateCountdown();
                UpdateMessage();
            };
            timer.Start();
        }

        private void StartUpdateCheckTimer()
        {
            updateCheckTimer = new DispatcherTimer();
            updateCheckTimer.Interval = TimeSpan.FromMinutes(5);
            updateCheckTimer.Tick += async (sender, e) =>
            {
                await CheckForUpdatesAsync();
            };
            updateCheckTimer.Start();
        }

        private async Task CheckForUpdatesAsync()
        {
            try
            {
                bool updated = await quickFormClient.CheckForUpdatesAsync();
                if (updated)
                {
                    LoadConfig();
                    UpdateMessage();
                    UpdateCountdown();
                }
            }
            catch
            {
            }
        }

        private void UpdateCountdown()
        {
            if (targetDate > DateTime.Now)
            {
                TimeSpan timeLeft = targetDate - DateTime.Now;
                CountdownTextBlock.Text = $"距离{countdownName}还有 {timeLeft.Days} 天";
                TargetDateTextBlock.Text = $"{countdownName}：{targetDate.ToString("yyyy年MM月dd日")}";
            }
            else if (targetDate != default)
            {
                CountdownTextBlock.Text = $"{countdownName}已到达！";
                TargetDateTextBlock.Text = $"{countdownName}：{targetDate.ToString("yyyy年MM月dd日")}";
            }
            CountdownTextBlock.FontSize = countdownFontSize;
            TargetDateTextBlock.FontSize = countdownFontSize * 0.8;
        }

        private void UpdateMessage()
        {
            if (messages.Count > 0)
            {
                DayOfWeek dayOfWeek = DateTime.Now.DayOfWeek;
                int index;
                if (dayOfWeek == DayOfWeek.Sunday)
                {
                    index = 6;
                }
                else
                {
                    index = (int)dayOfWeek - 1;
                }
                index = index % messages.Count;
                MessageTextBlock.Text = messages[index];
            }
            else
            {
                MessageTextBlock.Text = "默认寄语：努力学习，天天向上！";
            }
            MessageTextBlock.FontSize = fontSize;
            switch (textAlignment)
            {
                case "Center":
                    MessageTextBlock.TextAlignment = TextAlignment.Center;
                    break;
                case "Right":
                    MessageTextBlock.TextAlignment = TextAlignment.Right;
                    break;
                default:
                    MessageTextBlock.TextAlignment = TextAlignment.Left;
                    break;
            }
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }

        private void SettingsButton_Click(object sender, RoutedEventArgs e)
        {
            SettingsWindow settingsWindow = new SettingsWindow(messages, targetDate, countdownName, fontSize, countdownFontSize, textAlignment);
            settingsWindow.Owner = this;
            if (settingsWindow.ShowDialog() == true)
            {
                messages = settingsWindow.Messages;
                targetDate = settingsWindow.TargetDate;
                countdownName = settingsWindow.CountdownName;
                fontSize = settingsWindow.FontSize;
                countdownFontSize = settingsWindow.CountdownFontSize;
                textAlignment = settingsWindow.TextAlignment;
                UpdateMessage();
                UpdateCountdown();
                SaveConfig();
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            notifyIcon?.Dispose();
        }

        private void LoadConfig()
        {
            try
            {
                if (File.Exists(configPath))
                {
                    messages.Clear();
                    string[] lines = File.ReadAllLines(configPath);
                    if (lines.Length > 0)
                    {
                        countdownName = lines[0];
                        
                        if (lines.Length > 1 && double.TryParse(lines[1], out double size))
                        {
                            fontSize = size;
                            
                            if (lines.Length > 2 && double.TryParse(lines[2], out double countdownSize))
                            {
                                countdownFontSize = countdownSize;
                                
                                if (lines.Length > 3)
                                {
                                    textAlignment = lines[3];
                                    for (int i = 4; i < lines.Length - 1; i++)
                                    {
                                        messages.Add(lines[i]);
                                    }
                                    if (lines.Length > 4 && DateTime.TryParse(lines[lines.Length - 1], out DateTime date))
                                    {
                                        targetDate = date;
                                    }
                                }
                                else
                                {
                                    if (lines.Length > 2)
                                    {
                                        textAlignment = lines[2];
                                        for (int i = 3; i < lines.Length - 1; i++)
                                        {
                                            messages.Add(lines[i]);
                                        }
                                        if (lines.Length > 3 && DateTime.TryParse(lines[lines.Length - 1], out DateTime date))
                                        {
                                            targetDate = date;
                                        }
                                    }
                                }
                            }
                            else
                            {
                                if (lines.Length > 2)
                                {
                                    textAlignment = lines[2];
                                    for (int i = 3; i < lines.Length - 1; i++)
                                    {
                                        messages.Add(lines[i]);
                                    }
                                    if (lines.Length > 3 && DateTime.TryParse(lines[lines.Length - 1], out DateTime date))
                                    {
                                        targetDate = date;
                                    }
                                }
                            }
                        }
                        else
                        {
                            for (int i = 1; i < lines.Length - 1; i++)
                            {
                                messages.Add(lines[i]);
                            }
                            if (lines.Length > 1 && DateTime.TryParse(lines[lines.Length - 1], out DateTime date))
                            {
                                targetDate = date;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"加载配置失败：{ex.Message}");
            }
        }

        private void SaveConfig()
        {
            try
            {
                string configDir = Path.GetDirectoryName(configPath);
                if (!Directory.Exists(configDir))
                {
                    Directory.CreateDirectory(configDir);
                }
                
                List<string> configLines = new List<string>();
                configLines.Add(countdownName);
                configLines.Add(fontSize.ToString());
                configLines.Add(countdownFontSize.ToString());
                configLines.Add(textAlignment);
                configLines.AddRange(messages);
                configLines.Add(targetDate.ToString());
                
                using (StreamWriter writer = new StreamWriter(configPath, false))
                {
                    foreach (string line in configLines)
                    {
                        writer.WriteLine(line);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"保存配置失败：{ex.Message}\n路径：{configPath}");
            }
        }
    }
}