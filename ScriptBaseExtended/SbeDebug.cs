using System;
using System.Diagnostics;
using System.Reflection;
using System.Threading;


public partial class ScriptBaseExtended
{

    public void WaitForDebuggerAttachInSbe(int timeoutSeconds = 0)
    {
        Console.WriteLine("Waiting for debugger to attach...");
        var sw = Stopwatch.StartNew();
        while (!Debugger.IsAttached)
        {
            ScriptBaseInstance.Wait(1, true, "Waiting for debugger to attach...");
            if (timeoutSeconds > 0 && sw.Elapsed.TotalSeconds >= timeoutSeconds)
            {
                Console.WriteLine("Debugger did not attach before timeout.");
                return;
            }
        }
        Console.WriteLine("Debugger attached!");
        Debugger.Log(0, "warmup", "Warmup\n");
        Thread.Sleep(10);
        Debugger.Break();
    }

    private bool WasApplicationScriptCompiledInDebugMode()
    {
        Assembly applicationScriptAssembly = ScriptBaseInstance.GetType().Assembly;
        DebuggableAttribute dbg = applicationScriptAssembly.GetCustomAttribute<DebuggableAttribute>();

        if (dbg == null)
            return false;
        return dbg.IsJITOptimizerDisabled;
    }
    public bool IsDebugBuild()
    {
        if (IsApplicationScriptInDebugMode)
        {
            return true;
        }
#if DEBUG
        return true;
#else
        return false;
#endif
    }
}