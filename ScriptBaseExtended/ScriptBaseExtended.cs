
using LoginPI.Engine.ScriptBase;
using SyslogNet.Client.Serialization;
using SyslogNet.Client.Transport;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;

public partial class ScriptBaseExtended
{
    private readonly ScriptBase ScriptBaseInstance;
    private readonly string _victoriaMetricsImportUrl;
    private readonly string _victoriaMetricsLateImportUrl;
    private SyslogUdpSender? syslogSender;
    private ISyslogMessageSerializer? serializer;
    private readonly string LoggingServer = "DoNotSyslog";
    public Exception? lastException;
    public string traceId = string.Empty;
    public string AppName;
    public string VSIUser = Environment.UserName;
    public Guid scriptBaseExtendedGuid = Guid.NewGuid();
    public bool IsApplicationScriptInDebugMode;

    [NoTrace]
    public ScriptBaseExtended(ScriptBase scriptBase, string fullAppName, string loggingServer, string openTelemetryUrl, string victoriaMetricsImportUrl, string victoriaMetricsLateImportUrl)
    {
        ScriptBaseInstance = scriptBase;
        AppName = fullAppName;
        LoggingServer = loggingServer;
        IsApplicationScriptInDebugMode = WasApplicationScriptCompiledInDebugMode();
        InitLoggingSender();
        InitializeOpenTelemetry(openTelemetryUrl);
        _victoriaMetricsImportUrl = victoriaMetricsImportUrl;
        _victoriaMetricsLateImportUrl = victoriaMetricsLateImportUrl;
    }

    [NoTrace]
    public void CleanupTasks()
    {
        try
        {
            // Anything that should run after the script finishes or crashes.
            // For example, closing connections, releasing resources, stopping background tasks, etc.

            // Cleanup telemetry
            try
            {
                Telemetry.Shutdown();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to stop telemetry: {ex.Message}");
            }
        }
        catch
        {
            // Swallow everything here, we don't want to throw exceptions during cleanup.
            // This method may even run twice
        }
    }
}
