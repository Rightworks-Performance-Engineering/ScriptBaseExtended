using FastColoredTextBoxNS;
using LoginPI.Engine.ScriptBase;
using Microsoft.Win32;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
public class Helpers
{
    private const string RegistryKeyPath = @"Software\ScriptBaseExtendedTools\Settings";


    public static string GenerateMethodWrappers()
    {
        List<string> addedMethods = [];

        Type type = typeof(ScriptBaseExtended);
        StringBuilder builder = new();

        builder.AppendLine(@"// TARGET:""C:\\Path\To\exe.exe""");
        builder.AppendLine(@"// START_IN:");
        builder.AppendLine(@"using LoginPI.Engine.ScriptBase;");
        builder.AppendLine(@"using LoginPI.Engine.ScriptBase.Components;");
        builder.AppendLine(@"using System;");
        builder.AppendLine(@"using System.Collections.Generic;");
        builder.AppendLine(@"using System.Diagnostics;");
        builder.AppendLine(@"using System.Diagnostics.Contracts;");
        builder.AppendLine(@"using System.Drawing;");
        builder.AppendLine(@"using System.IO;");
        builder.AppendLine(@"using System.Linq;");
        builder.AppendLine(@"using System.Reflection;");
        builder.AppendLine(@"using System.Runtime.InteropServices;");
        builder.AppendLine(@"using System.Runtime.InteropServices.ComTypes;");
        builder.AppendLine(@"using System.Runtime.CompilerServices;");
        builder.AppendLine(@"using System.Security.Cryptography;");
        builder.AppendLine(@"using System.Text;");
        builder.AppendLine(@"using System.Text.RegularExpressions;");
        builder.AppendLine(@"using System.Threading;");
        builder.AppendLine(@"using System.Threading.Tasks;");
        builder.AppendLine(@"public class ChangeThis : ScriptBase");
        builder.AppendLine(@"{");
        builder.AppendLine(@"#region ############### Vars ################");
        builder.AppendLine(@"    private const string _appName = ""Change This"";");
        builder.AppendLine(@"    private const string _syslogServer = ""YourSyslogServer.YourDomain.Com"";");
        builder.AppendLine(@"    private const string _openTelemetryUrl = ""http://YourTempoServer.YourDomain.Com:4318/v1/traces"";");
        builder.AppendLine(@"    private const string _victoriaMetricsImportUrl = ""https://YourVictoriaMetricsServer.YourDomain.Com/prometheus/api/v1/import/prometheus"";");
        builder.AppendLine(@"    private const string _victoriaMetricsLateImportUrl = """"; // Use this if you want to separate late imports");
        builder.AppendLine(@"    private const string _scriptBaseExtendedPath =@""\\Path\To\ScriptBaseExtended.dll"";");
        builder.AppendLine(@"    //public int Apple = 1;");
        builder.AppendLine(@"    //public int Potato = 10;");
        builder.AppendLine(@"#endregion ############### Vars For All Years ################");
        builder.AppendLine(@"    private void Execute()");
        builder.AppendLine(@"    {");
        builder.AppendLine(@"        try");
        builder.AppendLine(@"        {");
        builder.AppendLine(@"#region ################### Script Start ###################");
        builder.AppendLine(@"            Init(this,_appName,_syslogServer,_openTelemetryUrl,_victoriaMetricsImportUrl,_victoriaMetricsLateImportUrl);");
        builder.AppendLine(@"#endregion ################### Script Start ###################");
        builder.AppendLine(@"#region ################### Script Body (EDIT ME) ###################");
        builder.AppendLine();
        builder.AppendLine();
        builder.AppendLine();
        builder.AppendLine(@"            //Your Code Here");
        builder.AppendLine(@"            //Your Code Here");
        builder.AppendLine(@"            //Your Code Here");
        builder.AppendLine(@"            //Your Code Here");
        builder.AppendLine(@"            //Your Code Here");
        builder.AppendLine();
        builder.AppendLine();
        builder.AppendLine();
        builder.AppendLine(@"#endregion ################### Script Body (EDIT ME) ###################");
        builder.AppendLine(@"#region ######### Script Body Catch (DO NOT EDIT) #########");
        builder.AppendLine(@"        }");
        builder.AppendLine(@"        // Catch FatalErrorExceptions that generate from inside SBE");
        builder.AppendLine(@"        catch (Exception ex) when (WasHandledByTraceAspect(ex))");
        builder.AppendLine(@"        {");
        builder.AppendLine(@"            SendSyslog(""Error Executing Application"", ex);");
        builder.AppendLine(@"            CleanupTasks();");
        builder.AppendLine(@"            throw;");
        builder.AppendLine(@"        }");
        builder.AppendLine(@"        catch (Exception ex)");
        builder.AppendLine(@"        {");
        builder.AppendLine(@"            int callerLine = 0;");
        builder.AppendLine(@"            string callerMember = ""Unknown"";");
        builder.AppendLine(@"            try");
        builder.AppendLine(@"            {");
        builder.AppendLine(@"                StackTrace stackTrace = new StackTrace(ex, true);");
        builder.AppendLine(@"                StackFrame frame = stackTrace.GetFrames()?.FirstOrDefault();");
        builder.AppendLine(@"                if (frame != null)");
        builder.AppendLine(@"                {");
        builder.AppendLine(@"                    callerLine = frame.GetFileLineNumber();");
        builder.AppendLine(@"                    callerMember = frame.GetMethod()?.Name ?? ""Unknown"";");
        builder.AppendLine(@"                }");
        builder.AppendLine(@"            }");
        builder.AppendLine(@"            catch");
        builder.AppendLine(@"            {");
        builder.AppendLine(@"            }");
        builder.AppendLine(@"            SendExceptionToOpenTelemetry(");
        builder.AppendLine(@"                ex,");
        builder.AppendLine(@"                callerLine,");
        builder.AppendLine(@"                callerMember);");
        builder.AppendLine(@"            SendSyslog(");
        builder.AppendLine(@"                ""Error Executing Application"",");
        builder.AppendLine(@"                ex);");
        builder.AppendLine(@"                CleanupTasks();");
        builder.AppendLine(@"                throw;");
        builder.AppendLine(@"        }");
        builder.AppendLine(@"        finally");
        builder.AppendLine(@"        {");
        builder.AppendLine(@"            try");
        builder.AppendLine(@"            {");
        builder.AppendLine(@"                CleanupTasks();");
        builder.AppendLine(@"            }");
        builder.AppendLine(@"            catch");
        builder.AppendLine(@"            {");
        builder.AppendLine(@"             // discard   ");
        builder.AppendLine(@"            }");
        builder.AppendLine(@"        }");
        builder.AppendLine(@"    }");
        builder.AppendLine(@"#endregion ######### Script Body Catch (DO NOT EDIT) #########");
        builder.AppendLine(@"#region ##################### Helpers #####################");
        builder.AppendLine(@"    public void WaitForDebuggerAttach(int timeoutSeconds = 0)");
        builder.AppendLine(@"    {");
        builder.AppendLine(@"        Console.WriteLine(""Waiting for debugger to attach..."");");
        builder.AppendLine(@"        var sw = Stopwatch.StartNew();");
        builder.AppendLine(@"        while (!Debugger.IsAttached)");
        builder.AppendLine(@"        {");
        builder.AppendLine(@"            Wait(1,true, ""Waiting for debugger to attach..."");");
        builder.AppendLine(@"            if (timeoutSeconds > 0 && sw.Elapsed.TotalSeconds >= timeoutSeconds)");
        builder.AppendLine(@"            {");
        builder.AppendLine(@"                Console.WriteLine(""Debugger did not attach before timeout."");");
        builder.AppendLine(@"                return;");
        builder.AppendLine(@"            }");
        builder.AppendLine(@"        }");
        builder.AppendLine(@"        Console.WriteLine(""Debugger attached!"");");
        builder.AppendLine(@"        Debugger.Log(0, ""warmup"", ""Warmup\n"");  // Force VS to see managed IL before the real break");
        builder.AppendLine(@"        Thread.Sleep(10);");
        builder.AppendLine(@"        Debugger.Break(); ");
        builder.AppendLine(@"        Debugger.Break(); // Optional automatic break-in we call it twice because we had issues with remote debugging");
        builder.AppendLine(@"    }");
        builder.AppendLine(@"    private static readonly Assembly assembly =");
        builder.AppendLine(@"    Assembly.LoadFrom(_scriptBaseExtendedPath);");
        builder.AppendLine();
        builder.AppendLine(@"    private static readonly Type type =");
        builder.AppendLine(@"    assembly.GetType(""ScriptBaseExtended"");");
        builder.AppendLine();
        builder.AppendLine(@"    private object instance;");
        builder.AppendLine();
        builder.AppendLine(@"    public ScriptBase? sbi;");
        builder.AppendLine();
        builder.AppendLine(@"    public void Init(ScriptBase scriptBase, string appName, string loggingServer, string openTelemetryUrl, string victoriaMetricsImportUrl, string victoriaMetricsLateImportUrl)    {");
        builder.AppendLine(@"        sbi = scriptBase;");
        builder.AppendLine();
        builder.AppendLine(@"        try");
        builder.AppendLine(@"        {");
        builder.AppendLine(@"            instance = Activator.CreateInstance(");
        builder.AppendLine(@"                type,");
        builder.AppendLine(@"                new object[] { scriptBase, appName, loggingServer, openTelemetryUrl, victoriaMetricsImportUrl, victoriaMetricsLateImportUrl });");
        builder.AppendLine(@"        }");
        builder.AppendLine(@"        catch (Exception ex)");
        builder.AppendLine(@"        {");
        builder.AppendLine(@"            Console.WriteLine(""FAILED TO CREATE INSTANCE"");");
        builder.AppendLine(@"            Console.WriteLine(ex.ToString());");
        builder.AppendLine(@"            throw;");
        builder.AppendLine(@"        }");
        builder.AppendLine(@"    }");
        builder.AppendLine(@"    public void InitCheck(){");
        builder.AppendLine(@"        if(sbi is null) throw new Exception(""Please Init() ScriptBaseExtended First."");");
        builder.AppendLine(@"    }");
        builder.AppendLine(@"    private static bool WasHandledByTraceAspect(Exception ex)");
        builder.AppendLine(@"    {");
        builder.AppendLine(@"    return ex.Data.Contains(""TraceAspect.ExceptionHandled"")");
        builder.AppendLine(@"        || (ex.InnerException?.Data.Contains(""TraceAspect.ExceptionHandled"") ?? false);");
        builder.AppendLine(@"    }");
        builder.AppendLine();
        builder.AppendLine();
        builder.AppendLine(@"    private T InvokeReflectedMethod<T>(string methodName, Type[] parameterTypes, object[] parameters, int callerLine, string callerMember)");
        builder.AppendLine(@"    {");
        builder.AppendLine(@"        InitCheck();");
        builder.AppendLine();
        builder.AppendLine(@"        MethodInfo method = type.GetMethod(methodName, parameterTypes);");
        builder.AppendLine(@"        if (method == null) {");
        builder.AppendLine(@"            // Handle the case where the method is not found");
        builder.AppendLine(@"            ABORT($""Method '{methodName}' with specified parameters not found."");");
        builder.AppendLine(@"            throw new MissingMethodException($""Method '{methodName}' not found on type '{type.FullName}' with the specified parameter types.""); }");
        builder.AppendLine(@"        try {");
        builder.AppendLine(@"            MethodInfo SetCallerContextMethod = type.GetMethod(""SetCallerContext"", new Type[] { typeof(int), typeof(string) });");
        builder.AppendLine(@"            SetCallerContextMethod.Invoke(");
        builder.AppendLine(@"                instance,");
        builder.AppendLine(@"                new object[]");
        builder.AppendLine(@"                {");
        builder.AppendLine(@"                    callerLine,");
        builder.AppendLine(@"                    callerMember");
        builder.AppendLine(@"                });");
        builder.AppendLine(@"            if (typeof(T) == typeof(object) && method.ReturnType == typeof(void)) {");
        builder.AppendLine(@"                method.Invoke(instance, parameters);");
        builder.AppendLine(@"                return default(T); }// Return default for void ");
        builder.AppendLine(@"            else {");
        builder.AppendLine(@"                object result = method.Invoke(instance, parameters);");
        builder.AppendLine(@"                return (T)result; } }");
        builder.AppendLine(@"        catch (TargetInvocationException ex) { ");
        builder.AppendLine(@"            throw ex.InnerException ?? ex; }");
        builder.AppendLine(@"        catch (InvalidCastException ex) {");
        builder.AppendLine(@"            throw ex.InnerException ?? ex; }");
        builder.AppendLine(@"        catch (Exception ex) {");
        builder.AppendLine(@"            // Handle other potential exceptions during invocation");
        builder.AppendLine(@"            throw; }");
        builder.AppendLine(@"        finally");
        builder.AppendLine(@"        {");
        builder.AppendLine(@"            MethodInfo ClearCallerContextMethod = type.GetMethod(""ClearCallerContext"", new Type[] { });");
        builder.AppendLine(@"            ClearCallerContextMethod?.Invoke(");
        builder.AppendLine(@"                instance,");
        builder.AppendLine(@"                null);");
        builder.AppendLine(@"        }");
        builder.AppendLine(@"    }");
        builder.AppendLine(@"    private void InvokeReflectedVoidMethod(string methodName, Type[] parameterTypes, object[] parameters, int callerLine, string callerMember)");
        builder.AppendLine(@"    { InvokeReflectedMethod<object>(methodName, parameterTypes, parameters, callerLine, callerMember); }");

        builder.AppendLine();
        builder.AppendLine(@"#endregion ##################### Helpers #####################");
        builder.AppendLine();
        builder.AppendLine(@"#region ##################### Reflected Methods #####################");
        builder.AppendLine();


        // Generate parameter list string with default values where applicable
        // Fix uppercase types being returned
        string FormatType(Type type) =>
            type == typeof(void) ? "void" :
            type == typeof(string) ? "string" :
            type == typeof(int) ? "int" :
            type == typeof(bool) ? "bool" :
            type == typeof(double) ? "double" :
            type == typeof(float) ? "float" :
            type == typeof(long) ? "long" :
            type == typeof(short) ? "short" :
            type == typeof(byte) ? "byte" :
            type == typeof(char) ? "char" :
            type == typeof(DateTime) ? "DateTime" :
            type == typeof(DateTime?) ? "DateTime?" :
            type == typeof(List<string>) ? "List<string>" :
            type == typeof(List<int>) ? "List<int>" :
            type == typeof(List<bool>) ? "List<bool>" :
            type == typeof(List<double>) ? "List<double>" :
            type == typeof(List<float>) ? "List<float>" :
            type == typeof(List<long>) ? "List<long>" :
            type == typeof(List<short>) ? "List<short>" :
            type == typeof(List<byte>) ? "List<byte>" :
            type == typeof(List<char>) ? "List<char>" :
            type == typeof(string[]) ? "string[]" :
            type == typeof(int[]) ? "int[]" :
            type == typeof(bool[]) ? "bool[]" :
            type == typeof(double[]) ? "double[]" :
            type == typeof(float[]) ? "float[]" :
            type == typeof(long[]) ? "long[]" :
            type == typeof(short[]) ? "short[]" :
            type == typeof(byte[]) ? "byte[]" :
            type == typeof(char[]) ? "char[]" :
            type == typeof(ValueTuple<bool, bool>) ? "(bool, bool)" :
            type == typeof(ValueTuple<int, int>) ? "(int, int)" :
            type == typeof(ValueTuple<bool, bool, bool>) ? "(bool, bool, bool)" :
            type == typeof(ValueTuple<bool, bool, bool, bool>) ? "(bool, bool, bool, bool)" :
            type == typeof(ValueTuple<string, string>) ? "(string, string)" :
            type == typeof(ValueTuple<string, string, int>) ? "(string, string, int)" :
            type == typeof(ValueTuple<string, string, string>) ? "(string, string, string)" :
            type == typeof(ValueTuple<string, string, string, string>) ? "(string, string, string, string)" :
            type == typeof(ValueTuple<string, string, string, string, int>) ? "(string, string, string, string, int)" :
            type.IsGenericType ?
                $"{type.Name.Split('`')[0]}<{string.Join(", ", type.GetGenericArguments().Select(FormatType))}>" :
            type.Name; // Fallback for other types

        foreach (MethodInfo method in type.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly))
        {

            ParameterInfo[] parameters = method.GetParameters();



            string parameterList = string.Join(", ", parameters
                .Where(p => p.ParameterType != typeof(ScriptBase) && p.Name is not "callerLine" and not "callerMember")
                .Select(p => p.HasDefaultValue
                    ? $"{FormatType(p.ParameterType)} {p.Name} = {FormatDefaultValue(p)}"
                    : $"{FormatType(p.ParameterType)} {p.Name}"));

            parameterList += (parameterList.Length > 0 ? ", " : "") + "[CallerLineNumber] int callerLine = 0, [CallerMemberName] string callerMember = \"\"";

            string FormatDefaultValue(ParameterInfo param)
            {
                if (param.HasDefaultValue)
                {
                    object defaultValue = param.DefaultValue;
                    Type paramType = param.ParameterType;

                    if (defaultValue == null) return "null"; // Handle null values

                    if (paramType == typeof(string))
                        return $"@\"{defaultValue}\""; // Encase strings in quotes

                    if (paramType == typeof(bool))
                        return defaultValue.ToString().ToLowerInvariant(); // Preserve lowercase for booleans

                    if (paramType == typeof(string[]))
                        return $"{{ {string.Join(", ", ((string[])defaultValue).Select(s => $"@\"{s}\""))} }}"; // Format string arrays

                    if (paramType == typeof(int[]))
                        return $"{{ {string.Join(", ", (int[])defaultValue)} }}"; // Format integer arrays

                    if (paramType == typeof(KeyCode) && param.DefaultValue is KeyCode keyValue)
                        return $"KeyCode." + keyValue.ToString().Trim();

                    return defaultValue.ToString(); // Default formatting for other types
                }
                throw new Exception(@$"Param {param.Name} has no default value.");
            }

            string argumentList = string.Join(", ", parameters.Select(p => p.Name));

            MethodInfo[] overloads = type.GetMethods()
                .Where(m => m.Name == method.Name && m.GetParameters().Length == parameters.Length)
                .ToArray();

            for (int i = 0; i < overloads.Count(); i++)
            {
                if (i > 0)
                {
                    bool sameParams = false;
                    for (int p = 0; p < overloads[i].GetParameters().Count() && p < overloads[i - 1].GetParameters().Count(); p++)
                    {
                        if (overloads[i].GetParameters()[p].Name == overloads[i - 1].GetParameters()[p].Name)
                        {
                            sameParams = true;
                            break;
                        }
                    }
                    if (sameParams)
                    { continue; }
                }
                bool isAsync = typeof(Task).IsAssignableFrom(method.ReturnType);

                string returnType = FormatType(overloads[i].ReturnType);


                if (returnType.StartsWith("ValueTuple`") || returnType.StartsWith("Tuple`"))
                {
                    var genericArgs = overloads[i].ReturnType.GetGenericArguments();
                    StringBuilder sb = new();
                    sb.Append("(");
                    for (int t = 0; t < genericArgs.Length; t++)
                    {
                        sb.Append(FormatType(genericArgs[t]) + ", ");
                    }
                    returnType = sb.ToString().Trim().Trim(',') + ")";
                }

                if (isAsync)
                {
                    string declaration = $"public async {returnType} {method.Name}({parameterList})";
                    if (addedMethods.Any(dec => dec == declaration))
                    { continue; }
                    else
                    { addedMethods.Add(declaration); }

                    builder.Append($"    {declaration}");
                    if (returnType == "void")
                    {
                        builder.Append($"    {{ await InvokeReflectedVoidMethod(\"{method.Name}\", new Type[] {{ {string.Join(", ", parameters.Select(p => $"typeof({FormatType(p.ParameterType)})"))} }}, new Object[] {{ {argumentList} }},callerLine, callerMember); }}");
                    }
                    else
                    {
                        builder.Append($"    {{  return await InvokeReflectedMethod<{returnType}>(\"{method.Name}\", new Type[] {{ {string.Join(", ", parameters.Select(p => $"typeof({FormatType(p.ParameterType)})"))} }}, new Object[] {{ {argumentList} }}, callerLine, callerMember); }}");
                    }
                }
                else
                {
                    string declaration = $"public {returnType} {method.Name}({parameterList})";
                    if (addedMethods.Any(dec => dec == declaration))
                    { continue; }
                    else
                    { addedMethods.Add(declaration); }

                    builder.Append($"    {declaration}");
                    if (returnType == "void")
                    {
                        builder.Append($"    {{ InvokeReflectedVoidMethod(\"{method.Name}\", new Type[] {{ {string.Join(", ", parameters.Select(p => $"typeof({FormatType(p.ParameterType)})"))} }}, new Object[] {{ {argumentList} }}, callerLine, callerMember); }}");
                    }
                    else
                    {
                        builder.Append($"    {{ return InvokeReflectedMethod<{returnType}>(\"{method.Name}\", new Type[] {{ {string.Join(", ", parameters.Select(p => $"typeof({FormatType(p.ParameterType)})"))} }}, new Object[] {{ {argumentList} }}, callerLine, callerMember); }}");
                    }
                }
                builder.AppendLine();
            }
        }
        builder.AppendLine();
        foreach (FieldInfo field in type.GetFields(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly))
        {
            string fieldType = FormatType(field.FieldType);
            builder.AppendLine($"    public readonly FieldInfo {field.Name}Field = type.GetField(\"{field.Name}\"); public {fieldType} Get{field.Name}() {{ return ({fieldType}){field.Name}Field.GetValue(instance); }}");

        }
        builder.AppendLine();
        builder.AppendLine(@"#endregion ##################### Reflected Methods #####################");
        builder.AppendLine();
        builder.AppendLine(@"}");
        string finalString = builder.ToString().Trim();
        return finalString.Replace("    public bool EnumWindows(EnumWindowsProc lpEnumFunc, IntPtr lParam)\r\n    { return InvokeReflectedMethod<bool>(\"EnumWindows\", new Type[] { typeof(EnumWindowsProc), typeof(IntPtr) }, new Object[] { lpEnumFunc, lParam }); }", "");

    }

    public static string ConvertMethod(string code)
    {
        Type type = typeof(ScriptBase);
        HashSet<string> excludedMethods = ["GetType", "ToString", "Equals", "GetHashCode"];
        foreach (MethodInfo method in type.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly))
        {
            if (!excludedMethods.Contains(method.Name))
            {
                string pattern = $@"(?<!ScriptBaseInstance\.){method.Name}\s*\(";
                code = Regex.Replace(code, pattern, $"ScriptBaseInstance.{method.Name}(", RegexOptions.None);
            }
        }
        return code;
    }

    public static string ConvertExtendedMethod(string code)
    {
        Type type = typeof(ScriptBase);
        HashSet<string> excludedMethods = ["GetType", "ToString", "Equals", "GetHashCode"];
        foreach (MethodInfo method in type.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly))
        {
            if (!excludedMethods.Contains(method.Name))
            {
                string pattern = $@"\bScriptBaseInstance\.{method.Name}\s*\(";
                code = Regex.Replace(code, pattern, $"{method.Name}(", RegexOptions.None);
            }
        }
        return code;
    }

    public static string FormatTextForAppendLine(string inputText)
    {
        List<string> lines = inputText.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries).ToList();

        StringBuilder outputBuilder = new();

        foreach (string line in lines)
        {
            string escapedLine = line.Replace("\"", "\"\""); // For C# verbatim string literals
            if (string.IsNullOrWhiteSpace(escapedLine))
            {
                outputBuilder.Append("builder.AppendLine();\n");
            }
            else
            {
                outputBuilder.AppendFormat("builder.AppendLine(@\"{0}\");\n", escapedLine);
            }

        }

        return outputBuilder.ToString();
    }

    public static string UnFormatTextForAppendLine(string inputText)
    {
        List<string> lines = inputText.Split(new[] { "\n" }, StringSplitOptions.RemoveEmptyEntries).ToList();

        StringBuilder outputBuilder = new();

        foreach (string line in lines)
        {
            if (line.Trim() == "builder.AppendLine();")
            {
                outputBuilder.AppendLine();
            }
            else if (line.StartsWith("builder.AppendLine(@\"") && line.EndsWith("\");\r"))
            {
                string extractedText = line.Substring("builder.AppendLine(@\"".Length,
                    line.Length - "builder.AppendLine(@\"\");\r".Length);
                extractedText = extractedText.Replace("\"\"", "\"");
                outputBuilder.AppendLine(extractedText);
            }
        }

        return outputBuilder.ToString();
    }

    public static ContextMenuStrip CreateContextMenu(FastColoredTextBox textBox)
    {
        ContextMenuStrip contextMenu = new();

        // Copy option
        ToolStripMenuItem copyItem = new("Copy");
        copyItem.Click += (sender, e) =>
        {
            if (!string.IsNullOrEmpty(textBox.SelectedText))
            {
                Clipboard.SetText(textBox.SelectedText);
            }
        };

        // Paste option
        ToolStripMenuItem pasteItem = new("Paste");
        pasteItem.Click += (sender, e) =>
        {
            if (Clipboard.ContainsText())
            {
                textBox.InsertText(Clipboard.GetText());
            }
        };

        // Add items to the menu
        contextMenu.Items.Add(copyItem);
        contextMenu.Items.Add(pasteItem);

        return contextMenu;
    }

    public static void SaveDarkModePreference(bool isDarkMode)
    {
        using (RegistryKey key = Registry.CurrentUser.CreateSubKey(RegistryKeyPath))
        {
            key.SetValue("DarkMode", isDarkMode ? 1 : 0, RegistryValueKind.DWord);
        }
    }

    public static bool LoadDarkModePreference()
    {
        using (RegistryKey key = Registry.CurrentUser.OpenSubKey(RegistryKeyPath))
        {
            if (key == null)
            {
                SaveDarkModePreference(true);
                return true;
            }

            return key.GetValue("DarkMode", 1) as int? == 1;
        }
    }

    public static FastColoredTextBox CreateCodeEditor(bool isDarkMode)
    {
        return new FastColoredTextBox()
        {
            Dock = DockStyle.Fill,
            Language = Language.CSharp,
            AutoIndent = true,
            AutoScrollMinSize = new Size(0, 20),
            Font = new Font("Consolas", 10),
            ShowLineNumbers = true,
            BackColor = isDarkMode ? Color.FromArgb(30, 30, 30) : Color.LightGray,
            ForeColor = isDarkMode ? Color.White : Color.Black,
            SelectionStyle = new SelectionStyle(new SolidBrush(isDarkMode ? Color.FromArgb(100, 149, 158, 19) : Color.FromArgb(100, 206, 87, 222))),
            ShowScrollBars = true

        };
    }

    public static void ToggleDarkMode(Form form, bool isDarkMode)
    {
        Color backColor = isDarkMode ? Color.Black : Color.White;
        Color foreColor = isDarkMode ? Color.White : Color.Black;
        Color textBoxBackColor = isDarkMode ? Color.FromArgb(30, 30, 30) : Color.LightGray;
        Color selectionColor = isDarkMode ? Color.FromArgb(100, 149, 158, 19) : Color.FromArgb(100, 206, 87, 222);

        form.BackColor = backColor;

        UpdateControlColors(form.Controls, backColor, foreColor, textBoxBackColor, selectionColor);
        SaveDarkModePreference(isDarkMode);
    }

    private static void UpdateControlColors(Control.ControlCollection controls, Color backColor, Color foreColor, Color textBoxBackColor, Color selectionColor)
    {
        foreach (Control control in controls)
        {
            if (control is Label || control is Button || control is CheckBox || control is TrackBar)
            {
                control.ForeColor = foreColor;
            }
            else if (control is TextBox)
            {
                control.BackColor = textBoxBackColor;
                control.ForeColor = foreColor;

            }
            else if (control is FastColoredTextBox)
            {
                control.ForeColor = foreColor;
                control.BackColor = textBoxBackColor;
                (control as FastColoredTextBox)!.IndentBackColor = textBoxBackColor;
                (control as FastColoredTextBox)!.LineNumberColor = foreColor;
                (control as FastColoredTextBox)!.SelectionStyle = new SelectionStyle(new SolidBrush(selectionColor));
            }
            else if (control is MenuStrip menuStrip)
            {
                menuStrip.BackColor = textBoxBackColor;
                menuStrip.ForeColor = foreColor;

                foreach (ToolStripMenuItem item in menuStrip.Items)
                {
                    if (item is ToolStripMenuItem menuItem)
                    {
                        menuItem.ForeColor = foreColor;
                        menuItem.BackColor = textBoxBackColor;
                    }
                }
            }
            if (control.HasChildren)
            {
                UpdateControlColors(control.Controls, backColor, foreColor, textBoxBackColor, selectionColor);
            }
        }
    }
}