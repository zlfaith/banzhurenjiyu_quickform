using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using System.Threading.Tasks;
using WinForms = System.Windows.Forms;
using System.Drawing;
using Newtonsoft.Json;

namespace DesktopMessageApp
{
    public partial class MainWindow : Window
    {
        private AppConfig config;
        private DispatcherTimer timer;
        private DispatcherTimer updateCheckTimer;
        private string configPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "班主任寄语与倒计时", "config.json");
        private QuickFormClient quickFormClient;
        private const string ApiUrl = "https://quickform.cn/api/qvh8vbxcnt";
        private WinForms.NotifyIcon notifyIcon;

        public MainWindow()
        {
            InitializeComponent();
            config = new AppConfig();
            quickFormClient = new QuickFormClient(ApiUrl, configPath);
            LoadConfig();
            StartTimer();
            StartUpdateCheckTimer();
            UpdateCountdown();
            UpdateMessage();
            UpdateRemark();
            UpdatePanelVisibility();
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
                UpdateRemark();
            };
            timer.Start();
        }

        private void StartUpdateCheckTimer()
        {
            updateCheckTimer = new DispatcherTimer();
            updateCheckTimer.Interval = TimeSpan.FromMinutes(5);
            updateCheckTimer.Tick += async (sender, e) =>
            {
                if (config.UseQuickForm)
                {
                    await CheckForUpdatesAsync();
                }
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
                    UpdateRemark();
                    UpdatePanelVisibility();
                }
            }
            catch
            {
            }
        }

        private void UpdateCountdown()
        {
            if (config.TargetDate > DateTime.Now)
            {
                TimeSpan timeLeft = config.TargetDate - DateTime.Now;
                CountdownTextBlock.Text = $"距离{config.CountdownName}还有 {timeLeft.Days} 天";
                TargetDateTextBlock.Text = $"{config.CountdownName}：{config.TargetDate.ToString("yyyy年MM月dd日")}";
            }
            else if (config.TargetDate != default)
            {
                CountdownTextBlock.Text = $"{config.CountdownName}已到达！";
                TargetDateTextBlock.Text = $"{config.CountdownName}：{config.TargetDate.ToString("yyyy年MM月dd日")}";
            }
            CountdownTextBlock.FontSize = config.CountdownFontSize;
            TargetDateTextBlock.FontSize = config.CountdownFontSize * 0.8;
        }

        private void UpdateMessage()
        {
            if (config.Messages.Count > 0)
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
                index = index % config.Messages.Count;
                MessageTextBlock.Text = config.Messages[index];
            }
            else
            {
                MessageTextBlock.Text = "默认寄语：努力学习，天天向上！";
            }
            MessageTextBlock.FontSize = config.FontSize;
            switch (config.TextAlignment)
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

        private void UpdateRemark()
        {
            if (!string.IsNullOrWhiteSpace(config.Remark))
            {
                RemarkTextBlock.Text = config.Remark;
            }
            else
            {
                RemarkTextBlock.Text = "暂无备注";
            }
            RemarkTextBlock.FontSize = config.RemarkFontSize;
        }

        private void UpdatePanelVisibility()
        {
            MessagePanel.Visibility = config.ShowMessage ? Visibility.Visible : Visibility.Collapsed;
            CountdownPanel.Visibility = config.ShowCountdown ? Visibility.Visible : Visibility.Collapsed;
            RemarkPanel.Visibility = config.ShowRemark ? Visibility.Visible : Visibility.Collapsed;
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
            SettingsWindow settingsWindow = new SettingsWindow(
                config.Messages, 
                config.TargetDate, 
                config.CountdownName, 
                config.FontSize, 
                config.CountdownFontSize, 
                config.RemarkFontSize, 
                config.TextAlignment, 
                config.Remark, 
                config.UseQuickForm,
                config.ShowMessage,
                config.ShowCountdown,
                config.ShowRemark
            );
            settingsWindow.Owner = this;
            if (settingsWindow.ShowDialog() == true)
            {
                config.Messages = settingsWindow.Messages;
                config.TargetDate = settingsWindow.TargetDate;
                config.CountdownName = settingsWindow.CountdownName;
                config.FontSize = settingsWindow.FontSize;
                config.CountdownFontSize = settingsWindow.CountdownFontSize;
                config.RemarkFontSize = settingsWindow.RemarkFontSize;
                config.TextAlignment = settingsWindow.TextAlignment;
                config.Remark = settingsWindow.Remark;
                config.UseQuickForm = settingsWindow.UseQuickForm;
                config.ShowMessage = settingsWindow.ShowMessage;
                config.ShowCountdown = settingsWindow.ShowCountdown;
                config.ShowRemark = settingsWindow.ShowRemark;
                
                UpdateMessage();
                UpdateCountdown();
                UpdateRemark();
                UpdatePanelVisibility();
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
                    string json = File.ReadAllText(configPath);
                    AppConfig? loadedConfig = JsonConvert.DeserializeObject<AppConfig>(json);
                    if (loadedConfig != null)
                    {
                        config = loadedConfig;
                        if (config.Messages == null)
                            config.Messages = new List<string>();
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
                
                string json = JsonConvert.SerializeObject(config, Formatting.Indented);
                File.WriteAllText(configPath, json);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"保存配置失败：{ex.Message}\n路径：{configPath}");
            }
        }
    }
}
