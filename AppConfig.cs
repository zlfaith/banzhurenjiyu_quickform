using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace DesktopMessageApp
{
    public class AppConfig
    {
        [JsonProperty("countdownName")]
        public string CountdownName { get; set; } = "目标日期";

        [JsonProperty("fontSize")]
        public double FontSize { get; set; } = 18;

        [JsonProperty("countdownFontSize")]
        public double CountdownFontSize { get; set; } = 20;

        [JsonProperty("remarkFontSize")]
        public double RemarkFontSize { get; set; } = 16;

        [JsonProperty("textAlignment")]
        public string TextAlignment { get; set; } = "Left";

        [JsonProperty("remark")]
        public string Remark { get; set; } = "";

        [JsonProperty("useQuickForm")]
        public bool UseQuickForm { get; set; } = true;

        [JsonProperty("showMessage")]
        public bool ShowMessage { get; set; } = true;

        [JsonProperty("showCountdown")]
        public bool ShowCountdown { get; set; } = true;

        [JsonProperty("showRemark")]
        public bool ShowRemark { get; set; } = true;

        [JsonProperty("targetDate")]
        public DateTime TargetDate { get; set; } = DateTime.Now.AddDays(30);

        [JsonProperty("messages")]
        public List<string> Messages { get; set; } = new List<string>();
    }
}
