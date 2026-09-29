
using SyslogNet.Client;
using SyslogNet.Client.Serialization;
using SyslogNet.Client.Transport;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.RegularExpressions;

[NoTrace]
public partial class ScriptBaseExtended
{

    [NoTrace]
    private Severity GetSeverityFromString(string severityName)
    {
        if (severityName == "warn")
        {
            severityName = "Warning";
        }
        else if (severityName == "info")
        {
            severityName = "Informational";
        }
        else if (severityName == "error")
        {
            severityName = "Error";
        }
        else if (severityName == "debug")
        {
            severityName = "Debug";
        }

        if (Enum.TryParse<Severity>(severityName, true, out var result))
            return result;

        return Severity.Informational;
    }

    private void InitLoggingSender()
    {
        syslogSender = new SyslogUdpSender(LoggingServer, 514);
        serializer = new SyslogRfc5424MessageSerializer();
    }

    [NoTrace]
    public void SendSyslog(string msg, Exception? ex = null, string? level = "Informational", Dictionary<string, string>? additionalStructuredData = null)
    {

        if (LoggingServer == "DoNotSyslog")
        {
            return;
        }

        Severity severity = GetSeverityFromString(level);

        if (severity == Severity.Debug && !IsDebugBuild())
        {
            return; // Don't send debug messages if debug mode is not enabled
        }

        StructuredDataElement structuredData = new("scriptBaseExtended", new Dictionary<string, string>
            {
                { "scriptBaseExtendedSessionGuid", scriptBaseExtendedGuid.ToString() },
                { "user", VSIUser }
            });

        if (traceId != "")
        {
            structuredData.Parameters["traceId"] = traceId;
        }

        if (!string.IsNullOrWhiteSpace(Telemetry.CallerMember))
        {
            structuredData.Parameters.Add("callerLine", Telemetry.CallerLine?.ToString() == "0" ? "Unknown" : Telemetry.CallerLine?.ToString() ?? "N/A");
            structuredData.Parameters.Add("callerMember", Telemetry.CallerMember);
        }

        if (additionalStructuredData != null)
        {
            foreach (var kvp in additionalStructuredData)
            {
                if (kvp.Value == null)
                {
                    structuredData.Parameters.Add(kvp.Key, "null");
                    continue;
                }
                structuredData.Parameters.Add(kvp.Key, kvp.Value.Replace("\n", "\r").Replace("\r\n", "\r"));
            }
        }

        if (ex != null)
        {
            structuredData.Parameters.Add("exceptionMessage", ex.Message.Replace("\n", "\r").Replace("\r\n", "\r"));
            structuredData.Parameters.Add("exceptionStackTrace", ex.StackTrace.Replace("\n", "\r").Replace("\r\n", "\r") ?? "No stack trace available"); 

            if (severity == Severity.Informational)
            {
                severity = Severity.Error;
            }

            string? csFileName = ex.Data["csFileName"] as string;
            string? csFileLine = ex.Data["csFileLine"] as string;


            if (!string.IsNullOrEmpty(csFileName))
            {
                structuredData.Parameters.Add("csFileName", csFileName);
            }
            if (!string.IsNullOrEmpty(csFileLine))
            {
                structuredData.Parameters.Add("csFileLine", csFileLine);
            }

            if (ex.InnerException != null)
            {
                string? innerCsFileName = ex.Data["innerCsFileName"] as string;
                string? innerCsFileLine = ex.Data["innerCsFileLine"] as string;
                if (!string.IsNullOrEmpty(innerCsFileName))
                {
                    structuredData.Parameters.Add("innerCsFileName", innerCsFileName);
                }
                if (!string.IsNullOrEmpty(innerCsFileLine))
                {
                    structuredData.Parameters.Add("innerCsFileLine", innerCsFileLine);
                }
            }

        }

        var message = new SyslogMessage(
            dateTimeOffset: DateTimeOffset.Now,
            facility: Facility.InternalMessages,
            severity: severity,
            hostName: Environment.MachineName,
            appName: "Application Script:" + Regex.Replace(AppName, @"[^0-9a-zA-Z ]", "_"),
            message: msg.Replace("\n", "\r").Replace("\r\n", "\r"),
            procId: Process.GetCurrentProcess().Id.ToString(),
            msgId: Guid.NewGuid().ToString("D"),
            structuredDataElements: structuredData
        );

        syslogSender!.Send(message, serializer);
        Console.WriteLine($"Sent syslog {Enum.GetName(typeof(Severity), severity)} message: " + msg);
    }
}
