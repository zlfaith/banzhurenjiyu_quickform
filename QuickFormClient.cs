using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace DesktopMessageApp
{
    public class QuickFormClient
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiUrl;
        private readonly string _configPath;

        public QuickFormClient(string apiUrl, string configPath)
        {
            _httpClient = new HttpClient();
            _apiUrl = apiUrl;
            _configPath = configPath;
        }

        public async Task<bool> CheckForUpdatesAsync()
        {
            try
            {
                var latestSubmission = await GetLatestSubmissionAsync();
                if (latestSubmission == null)
                    return false;

                string localLastUpdate = GetLocalLastUpdateTime();
                
                if (DateTime.TryParse(localLastUpdate, out DateTime localTime))
                {
                    if (latestSubmission.SubmitTime > localTime)
                    {
                        await UpdateLocalConfig(latestSubmission);
                        SaveLastUpdateTime(latestSubmission.SubmitTime.ToString());
                        return true;
                    }
                }
                else
                {
                    await UpdateLocalConfig(latestSubmission);
                    SaveLastUpdateTime(latestSubmission.SubmitTime.ToString());
                    return true;
                }

                return false;
            }
            catch
            {
                return false;
            }
        }

        private async Task<QuickFormSubmission> GetLatestSubmissionAsync()
        {
            try
            {
                string allDataUrl = _apiUrl + "/all";
                HttpResponseMessage response = await _httpClient.GetAsync(allDataUrl);
                
                if (response.IsSuccessStatusCode)
                {
                    string json = await response.Content.ReadAsStringAsync();
                    var apiResponse = JsonConvert.DeserializeObject<QuickFormApiResponse>(json);
                    
                    if (apiResponse != null && apiResponse.Submissions != null && apiResponse.Submissions.Count > 0)
                    {
                        apiResponse.Submissions.Sort((a, b) => b.SubmitTime.CompareTo(a.SubmitTime));
                        return apiResponse.Submissions[0];
                    }
                }
                else
                {
                    string errorContent = await response.Content.ReadAsStringAsync();
                    throw new Exception($"API请求失败: {response.StatusCode}, 内容: {errorContent}");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"GetLatestSubmissionAsync 错误: {ex.Message}");
            }
            
            return null;
        }

        private async Task UpdateLocalConfig(QuickFormSubmission submission)
        {
            try
            {
                string configDir = Path.GetDirectoryName(_configPath);
                if (!Directory.Exists(configDir))
                {
                    Directory.CreateDirectory(configDir);
                }

                List<string> configLines = new List<string>();
                configLines.Add(submission.CountdownName ?? "目标日期");
                configLines.Add(submission.FontSize ?? "30");
                configLines.Add(submission.CountdownFontSize ?? "30");
                configLines.Add(submission.TextAlignment ?? "Center");

                if (!string.IsNullOrEmpty(submission.Messages))
                {
                    string[] messages = submission.Messages.Split(new[] { "\n", "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
                    configLines.AddRange(messages);
                }

                if (DateTime.TryParse(submission.TargetDate, out DateTime targetDate))
                {
                    configLines.Add(targetDate.ToString());
                }
                else
                {
                    configLines.Add(DateTime.Now.AddDays(30).ToString());
                }

                using (StreamWriter writer = new StreamWriter(_configPath, false))
                {
                    foreach (string line in configLines)
                    {
                        writer.WriteLine(line);
                    }
                }

                await Task.CompletedTask;
            }
            catch
            {
            }
        }

        private string GetLocalLastUpdateTime()
        {
            string lastUpdatePath = _configPath + ".lastupdate";
            if (File.Exists(lastUpdatePath))
            {
                try
                {
                    return File.ReadAllText(lastUpdatePath);
                }
                catch
                {
                    return string.Empty;
                }
            }
            return string.Empty;
        }

        private void SaveLastUpdateTime(string time)
        {
            try
            {
                string lastUpdatePath = _configPath + ".lastupdate";
                File.WriteAllText(lastUpdatePath, time);
            }
            catch
            {
            }
        }

        public async Task<bool> SubmitDataAsync(string messages, string countdownName, DateTime targetDate)
        {
            try
            {
                var data = new
                {
                    messages = messages,
                    countdownName = countdownName,
                    targetDate = targetDate.ToString("yyyy-MM-dd"),
                    submitTime = DateTime.Now.ToString("o")
                };

                string json = JsonConvert.SerializeObject(data);
                StringContent content = new StringContent(json, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await _httpClient.PostAsync(_apiUrl, content);
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }
    }

    public class QuickFormSubmission
    {
        [JsonProperty("messages")]
        public string Messages { get; set; }

        [JsonProperty("countdownName")]
        public string CountdownName { get; set; }

        [JsonProperty("targetDate")]
        public string TargetDate { get; set; }

        [JsonProperty("submitTime")]
        public DateTime SubmitTime { get; set; }

        [JsonProperty("fontSize")]
        public string FontSize { get; set; }

        [JsonProperty("countdownFontSize")]
        public string CountdownFontSize { get; set; }

        [JsonProperty("textAlignment")]
        public string TextAlignment { get; set; }
    }

    public class QuickFormApiResponse
    {
        [JsonProperty("submissions")]
        public List<QuickFormSubmission> Submissions { get; set; }
    }
}