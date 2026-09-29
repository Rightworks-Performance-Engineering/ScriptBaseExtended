using System;
using System.Linq;
using System.Management;


public partial class ScriptBaseExtended
{
    public string GetWindowsFriendlyName()
    {
        using var searcher = new ManagementObjectSearcher("SELECT Caption FROM Win32_OperatingSystem");
        using var collection = searcher.Get();
        var os = collection.Cast<ManagementObject>().FirstOrDefault();
        if (os != null)
        {
            // Returns strings like "Microsoft Windows 11 Pro" 
            // or "Microsoft Windows Server 2025 Standard"
            return os["Caption"]?.ToString() ?? "Unknown";
        }
        throw new Exception("Unknown Windows Version");
    }
}