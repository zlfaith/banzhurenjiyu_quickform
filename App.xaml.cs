using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Win32;

namespace DesktopMessageApp
{
    public partial class App : System.Windows.Application
    {
        public App()
        {
            InitializeComponent();
            SetStartup();
        }

        private void SetStartup()
        {
            try
            {
                string appPath = System.Reflection.Assembly.GetExecutingAssembly().Location;
                string appName = "班主任寄语与倒计时";

                using (RegistryKey key = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true))
                {
                    if (key != null)
                    {
                        object existingValue = key.GetValue(appName);
                        if (existingValue == null || existingValue.ToString() != appPath)
                        {
                            key.SetValue(appName, appPath);
                        }
                    }
                }
            }
            catch { }
        }
    }
}