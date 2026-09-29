public partial class ScriptBaseExtended
{
    public void LogAndWait(double seconds, bool showOnScreen = false, string onScreenText = "Waiting...")
    {
        ScriptBaseInstance.Wait(seconds, showOnScreen, onScreenText);
    }
    public void LogAndWait(int seconds, bool showOnScreen = false, string onScreenText = "Waiting...")
    {
        ScriptBaseInstance.Wait(seconds, showOnScreen, onScreenText);
    }

}