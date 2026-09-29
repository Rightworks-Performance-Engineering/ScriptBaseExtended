using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;


public partial class ScriptBaseExtended
{
    private Dictionary<string, string> GetLabelsForCurrentTest()
    {
        return new Dictionary<string, string> { { "job", "ScriptBaseExtended" }, { "instance", Environment.MachineName }, { "ApplicationName", AppName }, { "WindowsVersion", GetWindowsFriendlyName() } };
    }

    public void SendTimer(string TimerName, int value, string metricName = "timer_duration_seconds")
    {
        SendPrometheusMetric(metricName, value / 1000.0, "", new Dictionary<string, string> { { "MeasurementId", TimerName } });
    }

    public void SendMetric(string TimerName, int value, string metricName = "session_metrics")
    {
        SendPrometheusMetric(metricName, value / 1000.0, "", new Dictionary<string, string> { { "MeasurementId", TimerName } });
    }

    public void SendMetricToVictoriaMetrics(string MeasurementId, int value, DateTime timeStamp, string metricName = "SessionMetrics")
    {
        SendPrometheusMetric(metricName, value / 1000.0, "", new Dictionary<string, string> { { "MeasurementId", MeasurementId } }, timeStamp);
    }

    private void SendPrometheusMetric(string metricName, double value, string helpText = "", Dictionary<string, string> labels = null, DateTime? timestamp = null)
    {
        labels ??= [];

        foreach (var kvp in GetLabelsForCurrentTest())
        {
            if (!labels.ContainsKey(kvp.Key))
            {
                labels.Add(kvp.Key, kvp.Value);
            }
        }

        // SessionMetrics do not need ApplicationName
        if (metricName == "SessionMetrics")
        {
            labels.Remove("ApplicationName");
        }

        string Endpoint = _victoriaMetricsImportUrl;

        var httpClient = new HttpClient();
        //string username = "UserName";
        //string password = "P@ssw0rd";
        //string credentials = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{username}:{password}"));
        //string authHeader = $"Basic {credentials}";
        //httpClient.DefaultRequestHeaders.Add("Authorization", authHeader);

        StringBuilder sb = new();
        if (!string.IsNullOrEmpty(helpText))
        {
            sb.AppendLine($"# HELP {metricName} {helpText}");
        }
        sb.Append($"# TYPE {metricName} gauge\n{metricName}");
        sb.Append("{");
        sb.Append(string.Join(",", labels.Select(kv => $"{kv.Key}=\"{kv.Value}\"")));
        sb.Append("}");
        sb.Append($" {value}");

        if (timestamp.HasValue && _victoriaMetricsLateImportUrl != "")
        {
            long unixTimeMilliseconds = new DateTimeOffset(timestamp.Value).ToUnixTimeMilliseconds();
            sb.Append($" {unixTimeMilliseconds}");

            Endpoint = _victoriaMetricsLateImportUrl;
        }

        sb.Append("\n");

        HttpResponseMessage response = httpClient.PostAsync(Endpoint, new StringContent(sb.ToString(), Encoding.UTF8, "text/plain")).Result;
        response.EnsureSuccessStatusCode();

    }
}