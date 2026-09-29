using LoginPI.Engine.ScriptBase;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;

public partial class ScriptBaseExtended
{
    private readonly Dictionary<string, Stopwatch> Timers = [];
    public void LogAndSetTimer(string timerName, int value, bool reportToScriptBase = true)
    {
        Telemetry.StartTimerLifecycleSpan(timerName);
        Telemetry.AddTimerLifecycleEvent(timerName, "LogAndSetTimer", value);
        Telemetry.EndTimerLifecycleSpan(timerName);
        ReportTimer(timerName, value, reportToScriptBase);
    }

    public void LogAndStartTimer(string timerName)
    {
        Timers[timerName] = new Stopwatch();
        Timers[timerName].Start();
        Telemetry.StartTimerLifecycleSpan(timerName);
        Telemetry.AddTimerLifecycleEvent(timerName, "LogAndStartTimer", 0);
    }

    public void LogAndStopTimer(string timerName, bool reportToScriptBase = true)
    {
        if (!Timers.TryGetValue(timerName, out var timer))
        {
            throw new Exception($"Timer with the name '{timerName}' does not exist");
        }
        timer.Stop();
        int value = (int)timer.ElapsedMilliseconds;
        ReportTimer(timerName, value, reportToScriptBase);
        Telemetry.AddTimerLifecycleEvent(timerName, "LogAndStopTimer", value);
        Telemetry.EndTimerLifecycleSpan(timerName);
        Timers.Remove(timerName);
    }

    public void LogAndCancelTimer(string timerName)
    {
        if (!Timers.TryGetValue(timerName, out _))
        {
            throw new Exception($"Timer with the name '{timerName}' does not exist");
        }
        Timers.Remove(timerName);
        Telemetry.AddTimerLifecycleEvent(timerName, "LogAndCancelTimer");
        Telemetry.EndTimerLifecycleSpan(timerName);
    }

    private void ReportTimer(string timerName, int value, bool reportToScriptBase)
    {
        ScriptBaseInstance.SetTimer(timerName, value);
        SendTimer(timerName, value);
        Telemetry.AddReportedTimer(timerName, value);
    }
}