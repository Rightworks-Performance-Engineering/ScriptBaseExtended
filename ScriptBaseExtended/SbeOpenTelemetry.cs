
using MethodBoundaryAspect.Fody.Attributes;
using OpenTelemetry;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Reflection;
using System.Threading;

public class TraceAttribute : OnMethodBoundaryAspect
{
    private static readonly ActivitySource Source = new("ScriptBaseExtended");
    private const string ProcessedKey = "TraceAspect.ExceptionHandled";
    private readonly List<string> hideParams = ["MethodThatTakesInSensitiveData", "AnotherMethodThatTakesInSensitiveData"];

    private sealed class TraceContext
    {
        public Stopwatch Stopwatch { get; set; }
        public Activity Activity { get; set; }
    }
    public override void OnEntry(MethodExecutionArgs args)
    {
        if (!Telemetry.TelemetryEnabled)
        {
            return;
        }
        if (args.Method.IsSpecialName && !Telemetry.IsDebugEnabled)
        {
            return;
        }
        if (args.Method.IsDefined(typeof(NoTraceAttribute)) && !Telemetry.IsDebugEnabled)
        {
            return;
        }


        Activity activity;

        if (Activity.Current == null &&
            Telemetry.RootActivity != null)
        {
            activity = Source.StartActivity(
                $"{args.Method.DeclaringType?.Name}.{args.Method.Name}",
                ActivityKind.Internal,
                Telemetry.RootActivity.Context);
        }
        else
        {
            activity = Source.StartActivity(
                $"{args.Method.DeclaringType?.Name}.{args.Method.Name}",
                ActivityKind.Internal);
        }

        if (activity != null) Telemetry.ActiveSpans.TryAdd(activity, 0);

        activity?.SetTag(
            "class",
            args.Method.DeclaringType?.FullName);

        activity?.SetTag(
            "method",
            args.Method.Name);



        if (args.Method.GetParameters().Length > 0 && !hideParams.Contains(args.Method.Name))
        {
            var parameters = args.Method.GetParameters();
            for (int i = 0; i < parameters.Length; i++)
            {
                var paramName = parameters[i].Name;
                var paramType = parameters[i].ParameterType;

                if (paramType != typeof(string) && typeof(System.Collections.IEnumerable).IsAssignableFrom(paramType))
                {
                    var enumerable = args.Arguments[i] as System.Collections.IEnumerable;
                    if (enumerable != null)
                    {
                        int index = 0;
                        foreach (var item in enumerable)
                        {
                            activity?.SetTag($"param.{paramName}[{index}]", item?.ToString() ?? "null");
                            index++;
                        }
                    }
                    else
                    {
                        activity?.SetTag($"param.{paramName}", "null");
                    }
                }
                else
                {
                    var paramValue = args.Arguments[i]?.ToString() ?? "null";
                    activity?.SetTag($"param.{paramName}", paramValue);
                }
            }
        }

        if (Telemetry.CallerLine.HasValue)
        {
            activity?.SetTag(
                "ApplicationScript.caller.line",
                Telemetry.CallerLine.Value);
        }

        if (!string.IsNullOrWhiteSpace(Telemetry.CallerMember))
        {
            activity?.SetTag(
                "ApplicationScript.caller.member",
                Telemetry.CallerMember);
        }

        args.MethodExecutionTag = new TraceContext
        {
            Stopwatch = Stopwatch.StartNew(),
            Activity = activity
        };
    }


    public override void OnExit(MethodExecutionArgs args)
    {
        if (!Telemetry.TelemetryEnabled)
        {
            return;
        }
        if (args.MethodExecutionTag is not TraceContext context)
            return;

        context.Stopwatch.Stop();

        context.Activity?.SetTag(
            "duration.ms",
            context.Stopwatch.Elapsed.TotalMilliseconds);

        context.Activity?.Dispose();

        if (context.Activity != null)
        {
            Telemetry.ActiveSpans.TryRemove(context.Activity, out _);
        }

    }

    public override void OnException(MethodExecutionArgs args)
    {
        try
        {
            if (!Telemetry.TelemetryEnabled)
            {
                return;
            }
            if (args.MethodExecutionTag is not TraceContext context)
                return;

            if (args.Exception.Data.Contains(ProcessedKey))
                return;

            if (Telemetry.ScriptBaseExtendedInstance.lastException != null && Telemetry.ScriptBaseExtendedInstance.lastException.Message == args.Exception.Message)
            {
                return;
            }

            Telemetry.ScriptBaseExtendedInstance.lastException = args.Exception;

            string csFileName = "Unknown";
            string csFileLine = "Unknown";

            StackTrace trace = new(args.Exception, true);
            StackFrame? firstFrameWithFile = null;

            foreach (StackFrame frame in trace.GetFrames())
            {
                if (!string.IsNullOrEmpty(frame.GetFileName()))
                {
                    firstFrameWithFile = frame;
                    break;
                }
            }

            if (firstFrameWithFile != null)
            {
                csFileName = firstFrameWithFile.GetFileName();
                if (firstFrameWithFile.GetFileLineNumber() != 0)
                {
                    csFileLine = firstFrameWithFile.GetFileLineNumber().ToString();
                }
            }

            ActivityTagsCollection tags = new()
            {
                    { "exception.type", args.Exception.GetType().FullName },
                    { "exception.message", args.Exception.Message },
                    { "exception.stacktrace", args.Exception.ToString() },
                    { "exception.csFileName", csFileName },
                    { "exception.csFileLine", csFileLine },
                    { "ApplicationScript.caller.line", Telemetry.CallerLine?.ToString() == "0" ? "Unknown" : Telemetry.CallerLine?.ToString() ?? "N/A" },
                    { "ApplicationScript.caller.member", Telemetry.CallerMember ?? "N/A"   },
                };

            args.Exception.Data["csFileName"] = csFileName;
            args.Exception.Data["csFileLine"] = csFileLine;

            string methodName = "UnknownCaller";
            if (args.Exception.TargetSite != null)
            {
                methodName = args.Exception.TargetSite.Name;
            }

            string callerInfo = "Script Caller: N/A";
            if (Telemetry.CallerLine.HasValue || !string.IsNullOrWhiteSpace(Telemetry.CallerMember))
            {
                callerInfo =
                    $"[Script Caller: {Telemetry.CallerMember} Line {Telemetry.CallerLine}]";
            }

            Console.WriteLine("================== Exception Occurred =================");
            Console.WriteLine(callerInfo);
            Console.WriteLine($"[{methodName}] Exception Message: '{args.Exception.Message}'");
            Console.WriteLine($"[Exception Stack trace]\n{args.Exception.StackTrace}");
            Telemetry.ScriptBaseExtendedInstance.SendSyslog($"[{methodName}] Exception: '{args.Exception.Message}'", args.Exception);
            if (args.Exception.InnerException != null)
            {
                string innerMethodName = "UnknownCaller";
                if (args.Exception.InnerException.TargetSite != null)
                {
                    innerMethodName = args.Exception.InnerException.TargetSite.Name;
                }

                string innercsFileName = "Unknown";
                string innercsFileLine = "Unknown";

                StackTrace innertrace = new(args.Exception.InnerException, true);
                StackFrame? innerfirstFrameWithFile = null;

                foreach (StackFrame innerframe in innertrace.GetFrames())
                {
                    if (!string.IsNullOrEmpty(innerframe.GetFileName()))
                    {
                        innerfirstFrameWithFile = innerframe;
                        break;
                    }
                }

                if (innerfirstFrameWithFile != null)
                {
                    innercsFileName = innerfirstFrameWithFile.GetFileName();
                    if (innerfirstFrameWithFile.GetFileLineNumber() != 0)
                    {
                        innercsFileLine = innerfirstFrameWithFile.GetFileLineNumber().ToString();
                    }
                }

                args.Exception.InnerException.Data["innerCsFileName"] = csFileName;
                args.Exception.InnerException.Data["innerCsFileLine"] = csFileLine;

                Console.WriteLine($"[{innerMethodName}] Inner Exception Message: '{args.Exception.InnerException.Message}'");
                Telemetry.ScriptBaseExtendedInstance.SendSyslog($"[{innerMethodName}] Inner Exception: '{args.Exception.InnerException.Message}'", args.Exception.InnerException);
            }

            foreach (var spanTags in context.Activity.Tags)
            {
                tags[spanTags.Key] = spanTags.Value;
            }

            foreach (var rootTags in Telemetry.RootActivity.Tags)
            {
                tags[rootTags.Key] = rootTags.Value;
            }

            foreach (var providerTags in Telemetry._provider.GetResource().Attributes)
            {
                tags[providerTags.Key] = providerTags.Value;
            }

            context.Activity?.SetStatus(
            ActivityStatusCode.Error,
            args.Exception.Message);


            context.Activity?.AddEvent(
                new ActivityEvent(
                    "exception",
                    tags: tags));

            context.Activity?.Dispose();

            args.Exception.Data[ProcessedKey] = true;
        }
        catch (Exception ex) when (ex.TargetSite.Name == "SendSyslog")
        {
            string methodName = ex.TargetSite.Name;
            Console.WriteLine("Unknown error sending exception to telemetry");
            Console.WriteLine($"[{methodName}] Exception StackTrace: '{ex.StackTrace}'");
            Console.WriteLine($"[{methodName}] Exception StackTrace contains SendSyslog, not sending syslog message to avoid infinite loop.");
            return;

        }
        catch (Exception ex)
        {
            string methodName = ex.TargetSite.Name;
            Console.WriteLine("Unknown error sending exception to telemetry");
            Console.WriteLine($"[{methodName}] Exception StackTrace: '{ex.StackTrace}'");
            Telemetry.ScriptBaseExtendedInstance.SendSyslog($"[{methodName}] Exception: '{ex.Message}'", ex);
            Telemetry.ScriptBaseExtendedInstance.SendSyslog($"[{args.Exception.TargetSite.Name}] Exception: '{args.Exception.Message}'", args.Exception);
            throw;
        }
    }
}

public class NoTraceAttribute : Attribute
{
}

public static class Telemetry
{
    public static bool TelemetryEnabled { get; set; } = true;
    public static TracerProvider _provider;
    private static readonly AsyncLocal<int?> _callerLine = new();
    private static readonly AsyncLocal<string> _callerMember = new();
    public static readonly ConcurrentDictionary<Activity, byte> ActiveSpans = new();
    public static bool IsDebugEnabled { get; set; }
    public static ScriptBaseExtended ScriptBaseExtendedInstance { get; set; }

    public static int? CallerLine
    {
        get => _callerLine.Value;
        set => _callerLine.Value = value;
    }

    public static string CallerMember
    {
        get => _callerMember.Value;
        set => _callerMember.Value = value;
    }

    public static void SetCallerContext(
        int callerLine,
        string callerMember)
    {
        CallerLine = callerLine;
        CallerMember = callerMember;
    }

    public static void ClearCallerContext()
    {
        CallerLine = null;
        CallerMember = null;
    }



    public static Activity RootActivity { get; set; }

    private static readonly ActivitySource Source =
        new("ScriptBaseExtended");



    private static readonly Dictionary<string, Activity> TimerActivities =
            new();



    public static void StartTimerLifecycleSpan(
        string timerName)
    {
        if (!Telemetry.TelemetryEnabled)
        {
            return;
        }
        if (RootActivity == null)
            return;

        if (TimerActivities.ContainsKey(timerName))
            return;

        var activity = Source.StartActivity(
            $"TimerLifecycle:{timerName}",
            ActivityKind.Internal,
            RootActivity.Context);

        activity?.SetTag("timer.name", timerName);
        activity?.AddEvent(
            new ActivityEvent("Timer Lifecycle Started"));

        TimerActivities[timerName] = activity;
    }

    public static void EndTimerLifecycleSpan(string timerName)
    {
        if (!Telemetry.TelemetryEnabled)
        {
            return;
        }
        if (!TimerActivities.TryGetValue(
            timerName,
            out var activity))
        {
            return;
        }

        activity.AddEvent(
            new ActivityEvent("Timer Lifecycle Ended"));

        activity.SetStatus(
            ActivityStatusCode.Ok);

        activity.Dispose();

        TimerActivities.Remove(timerName);
    }

    public static void AddTimerLifecycleEvent(string timerName, string eventName)
    {
        if (!Telemetry.TelemetryEnabled)
        {
            return;
        }
        if (TimerActivities.TryGetValue(
                timerName,
                out var activity))
        {
            activity.AddEvent(
                new ActivityEvent(eventName));
        }
    }
    public static void AddTimerLifecycleEvent(string timerName, string eventName, int newTimeSpanValue, string additionalInfo = "")
    {
        if (!Telemetry.TelemetryEnabled)
        {
            return;
        }
        if (TimerActivities.TryGetValue(
                timerName,
                out var activity))
        {
            ActivityTagsCollection tags = new()
            {
                    { "new.timespan.value", newTimeSpanValue.ToString() }
                };

            if (!string.IsNullOrEmpty(additionalInfo))
            {
                tags.Add("additional.info", additionalInfo);
            }

            activity.AddEvent(
            new ActivityEvent(eventName, tags: tags));
        }
    }

    public static void AddReportedTimer(string timerName, int reportedTimeSpanValue)
    {
        if (!Telemetry.TelemetryEnabled)
        {
            return;
        }

        var stop = DateTime.UtcNow;

        var start = stop - TimeSpan.FromMilliseconds(reportedTimeSpanValue);


        using var activity = Source.StartActivity(
            $"TimerDuration:{timerName}",
            ActivityKind.Internal,
            Telemetry.RootActivity.Context, startTime: start);

        activity?.SetTag("timer.name", timerName);

        activity?.SetStatus(
    ActivityStatusCode.Ok);

        activity?.Dispose();

    }

    public static void RecordException(Exception ex)
    {
        try
        {
            if (!Telemetry.TelemetryEnabled)
            {
                return;
            }
            // prevent duplicate logs of the same Exception
            if (ex.Data.Contains("TraceAspect.ExceptionHandled"))
                return;

            if (Telemetry.ScriptBaseExtendedInstance.lastException != null && Telemetry.ScriptBaseExtendedInstance.lastException.Message == ex.Message)
            {
                return;
            }

            Telemetry.ScriptBaseExtendedInstance.lastException = ex;
            var activity = Activity.Current;

            if (activity == null)
                return;




            string csFileName = "Unknown";
            string csFileLine = "Unknown";

            StackTrace trace = new(ex, true);
            StackFrame? firstFrameWithFile = null;

            foreach (StackFrame frame in trace.GetFrames())
            {
                if (!string.IsNullOrEmpty(frame.GetFileName()))
                {
                    firstFrameWithFile = frame;
                    break;
                }
            }

            if (firstFrameWithFile != null)
            {
                csFileName = firstFrameWithFile.GetFileName();
                if (firstFrameWithFile.GetFileLineNumber() != 0)
                {
                    csFileLine = firstFrameWithFile.GetFileLineNumber().ToString();
                }
            }

            ActivityTagsCollection tags = new()
        {
            { "exception.type", ex.GetType().FullName },
            { "exception.message", ex.Message },
            { "exception.stacktrace", ex.ToString() },
            { "exception.csFileName", csFileName },
            { "exception.csFileLine", csFileLine },
            { "ApplicationScript.caller.line", Telemetry.CallerLine?.ToString() == "0" ? "Unknown" : Telemetry.CallerLine?.ToString() ?? "N/A" },
            { "ApplicationScript.caller.member", Telemetry.CallerMember ?? "N/A" }
        };

            ex.Data["csFileName"] = csFileName;
            ex.Data["csFileLine"] = csFileLine;


            string methodName = "UnknownCaller";
            if (ex.TargetSite != null)
            {
                methodName = ex.TargetSite.Name;
            }

            string callerInfo = "Script Caller: N/A";
            if (Telemetry.CallerLine.HasValue || !string.IsNullOrWhiteSpace(Telemetry.CallerMember))
            {
                callerInfo =
                    $"[Script Caller: {Telemetry.CallerMember} Line {Telemetry.CallerLine}]";
            }
            Console.WriteLine("================== Exception Occurred =================");
            Console.WriteLine(callerInfo);
            Console.WriteLine($"[{methodName}] Exception Message: '{ex.Message}'");
            Console.WriteLine($"[Exception Stack trace]\n{ex.StackTrace}");
            Telemetry.ScriptBaseExtendedInstance.SendSyslog($"[{methodName}] Exception: '{ex.Message}'", ex);
            if (ex.InnerException != null)
            {
                string innerMethodName = "UnknownCaller";
                if (ex.InnerException.TargetSite != null)
                {
                    innerMethodName = ex.InnerException.TargetSite.Name;
                }

                string innercsFileName = "Unknown";
                string innercsFileLine = "Unknown";

                StackTrace innertrace = new(ex, true);
                StackFrame? innerfirstFrameWithFile = null;

                foreach (StackFrame innerframe in innertrace.GetFrames())
                {
                    if (!string.IsNullOrEmpty(innerframe.GetFileName()))
                    {
                        innerfirstFrameWithFile = innerframe;
                        break;
                    }
                }

                if (innerfirstFrameWithFile != null)
                {
                    innercsFileName = innerfirstFrameWithFile.GetFileName();
                    if (innerfirstFrameWithFile.GetFileLineNumber() != 0)
                    {
                        innercsFileLine = innerfirstFrameWithFile.GetFileLineNumber().ToString();
                    }
                }

                ex.InnerException.Data["innerCsFileName"] = csFileName;
                ex.InnerException.Data["innerCsFileLine"] = csFileLine;

                Console.WriteLine($"[{innerMethodName}] Inner Exception Message: '{ex.InnerException.Message}'");
                Telemetry.ScriptBaseExtendedInstance.SendSyslog($"[{innerMethodName}] Inner Exception: '{ex.InnerException.Message}'", ex.InnerException);
            }

            activity.SetStatus(
                ActivityStatusCode.Error,
                ex.Message);


            foreach (var spanTags in activity.Tags)
            {
                tags[spanTags.Key] = spanTags.Value;
            }

            foreach (var rootTags in RootActivity.Tags)
            {
                tags[rootTags.Key] = rootTags.Value;
            }

            foreach (var providerTags in _provider.GetResource().Attributes)
            {
                tags[providerTags.Key] = providerTags.Value;
            }

            activity.AddEvent(new ActivityEvent("exception", tags: tags));

            ex.Data["TraceAspect.ExceptionHandled"] = true;
        }
        catch (Exception iex) when (ex.TargetSite.Name == "SendSyslog")
        {
            string methodName = ex.TargetSite.Name;
            Console.WriteLine("Unknown error sending exception to telemetry");
            Console.WriteLine($"[{methodName}] Exception StackTrace: '{iex.StackTrace}'");
            Console.WriteLine($"[{methodName}] Exception StackTrace contains SendSyslog, not sending syslog message to avoid infinite loop.");
            return;

        }
        catch (Exception iex)
        {
            string methodName = ex.TargetSite.Name;
            Console.WriteLine("Unknown error sending exception to telemetry");
            Console.WriteLine($"[{methodName}] Exception StackTrace: '{iex.StackTrace}'");
            Telemetry.ScriptBaseExtendedInstance.SendSyslog($"[{methodName}] Exception: '{iex.Message}'", iex);
            throw;
        }
    }

    private static bool ValidateCollector(
    string uri,
    int timeoutMs = 2000)
    {
        try
        {
            using var client = new HttpClient
            {
                Timeout = TimeSpan.FromMilliseconds(timeoutMs)
            };

            using var request =
                new HttpRequestMessage(
                    HttpMethod.Options,
                    uri);

            var response =
                client.SendAsync(request).GetAwaiter().GetResult();

            return true;
        }
        catch (Exception ex)
        {
            ScriptBaseExtendedInstance.SendSyslog($"OpenTelemetry initialized Failed! Could not connect to {uri}", ex);
            return false;
        }
    }

    public static void Initialize(string uri, string scriptBaseExtendedGuid, string appName, string vsiUser, string server, bool isDebug, ScriptBaseExtended sbe)
    {
        if (_provider != null)
            return;
        ScriptBaseExtendedInstance = sbe;
        TelemetryEnabled = ValidateCollector(uri);
        if (!TelemetryEnabled)
        {
            return;
        }
        IsDebugEnabled = isDebug;

        Dictionary<string, object> attributes = new()
        {
            ["scriptbaseextended.guid"] = scriptBaseExtendedGuid,
            ["ApplicationScript.Name"] = appName,
            ["ApplicationScript.User"] = vsiUser,
            ["ApplicationScript.Server"] = server
        };
        Activity.Current = null;
        string version = Assembly.GetCallingAssembly()?.GetName().Version?.ToString() ?? "Unknown";
        _provider = Sdk.CreateTracerProviderBuilder()
            .SetResourceBuilder(
                ResourceBuilder
                    .CreateDefault()
                    .AddService("ScriptBaseExtended", serviceVersion: version)
                    .AddAttributes(attributes)
            )
            .AddSource("ScriptBaseExtended")
            .AddOtlpExporter(options =>
            {
                options.Endpoint = new Uri(uri);
                options.Protocol =
                    OpenTelemetry.Exporter.OtlpExportProtocol.HttpProtobuf;

                options.ExportProcessorType =
                    ExportProcessorType.Simple;
            })
            .Build();
        RootActivity = Source.StartActivity(
            $"{appName} on {server} as {vsiUser}",
            ActivityKind.Internal, default(ActivityContext));
        Console.WriteLine("Root Activity initialized.");

        ScriptBaseExtendedInstance.traceId = RootActivity!.TraceId.ToString();

        ScriptBaseExtendedInstance.SendSyslog("OpenTelemetry initialized successfully.");

    }

    public static void Shutdown()
    {

        if (!Telemetry.TelemetryEnabled)
        {
            RootActivity = null;
            _provider = null;
            return;
        }

        foreach (var timerActivity in TimerActivities.Values)
        {
            timerActivity.Dispose();
        }


        foreach (var activity in ActiveSpans.Keys)
        {
            if (activity is { IsStopped: false })
            {
                activity.SetStatus(ActivityStatusCode.Error, "Thread Aborted Abruptly");
                activity.Stop();
            }
        }
        ActiveSpans.Clear();
        TimerActivities.Clear();
        RootActivity?.Dispose();
        _provider?.ForceFlush();
        _provider?.Dispose();
        RootActivity = null;
        CallerLine = null;
        CallerMember = null;
        _provider = null;
        Console.WriteLine("OpenTelemetry shutdown completed.");
    }
}


[Trace]
public partial class ScriptBaseExtended
{
    private void InitializeOpenTelemetry(string uri)
    {
        ConcurrentDictionary<Activity, byte> ActiveSpans = new(); // For statics issue with reflection and mulitple inits.
        SendSyslog($"Initializing OpenTelemetry with URI: {uri}");
        Telemetry.Initialize(uri, scriptBaseExtendedGuid.ToString(), AppName, VSIUser, Environment.MachineName, IsApplicationScriptInDebugMode, this);
    }

    // Use this to add tags to the root activity that we don't have values for at the time of initialization, but we do later in the script.
    [NoTrace]
    public void AddTestInformationTagsToRootActivity()
    {
        if (Telemetry.RootActivity == null)
            return;

        Telemetry.RootActivity.SetTag("Tags.You.Want.On.Root.Activity", "SomeVar or Value Here" ?? "Unknown");
    }

    [NoTrace]
    public void SetCallerContext(
    int scriptCallerLine,
    string scriptCallerMember)
    {
        Telemetry.SetCallerContext(
            scriptCallerLine,
            scriptCallerMember);
    }
    [NoTrace]
    public void ClearCallerContext()
    {
        Telemetry.ClearCallerContext();
    }
    public void SendExceptionToOpenTelemetry(Exception ex)
    {
        Telemetry.RecordException(ex);
    }
}