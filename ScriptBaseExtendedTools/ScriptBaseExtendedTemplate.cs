using FastColoredTextBoxNS;
using static Helpers;

public class ScriptBaseExtendedTemplate : Form
{
    bool isDarkMode = LoadDarkModePreference();
    public ScriptBaseExtendedTemplate()
    {
        string templateText = "";
        string reflectedMethodsText = "";

        this.Text = "Application Script Template";
        Size = new Size(1920, 960);//TODO set to working area.
        StartPosition = FormStartPosition.CenterScreen;
        BackColor = isDarkMode ? Color.Black : Color.White;
        templateText = GenerateMethodWrappers();
        string reflectedMethodsStart = @"#region ##################### Reflected Methods #####################";
        reflectedMethodsText = templateText.Substring(templateText.IndexOf(reflectedMethodsStart));
        string[] lines = reflectedMethodsText.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
        reflectedMethodsText = string.Join(Environment.NewLine, lines.Skip(2).SkipLast(4));


        FastColoredTextBox codeEditor = new()
        {
            Dock = DockStyle.Fill,
            Language = Language.CSharp,
            AutoIndent = true,
            AutoScrollMinSize = new Size(0, 20),
            Font = new Font("Consolas", 10),
            ShowLineNumbers = true,
            BackColor = isDarkMode ? Color.FromArgb(30, 30, 30) : Color.White,
            ForeColor = isDarkMode ? Color.White : Color.Black,
            ShowScrollBars = true,
            Text = templateText,
            IndentBackColor = isDarkMode ? Color.FromArgb(30, 30, 30) : Color.White,
            LineNumberColor = isDarkMode ? Color.White : Color.Black,
            SelectionStyle = new SelectionStyle(new SolidBrush(isDarkMode ? Color.FromArgb(100, 149, 158, 19) : Color.FromArgb(100, 206, 87, 222))),

        };
        codeEditor.ContextMenuStrip = CreateContextMenu(codeEditor);

        // Create Save Button
        Button saveButton = new()
        {
            Text = "Save",
            Dock = DockStyle.Bottom,
            ForeColor = isDarkMode ? Color.White : Color.Black,
            BackColor = Color.Gray
        };

        // Save File Event
        saveButton.Click += (saveSender, saveEvent) =>
        {
            using (SaveFileDialog saveFileDialog = new()
            {
                Filter = "C# Files|*.cs|Text Files|*.txt|All Files|*.*",
                Title = "Save Generated Methods"
            })
            {
                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    System.IO.File.WriteAllText(saveFileDialog.FileName, codeEditor.Text);
                }
            }
        };

        // Create Append Button
        Button appendButton = new()
        {
            Text = "Append",
            Dock = DockStyle.Bottom,
            ForeColor = isDarkMode ? Color.White : Color.Black,
            BackColor = Color.Gray
        };

        // Open append form
        appendButton.Click += (sender, e) =>
        {
            ForAppendLineForm al = new();
            al.ShowDialog();
        };

        // Create Append Button
        Button copyReflectedMethods = new()
        {
            Text = "Copy Reflected Methods",
            Dock = DockStyle.Bottom,
            ForeColor = isDarkMode ? Color.White : Color.Black,
            BackColor = Color.Gray
        };

        // Open append form
        copyReflectedMethods.Click += (sender, e) =>
        {
            Clipboard.SetText(reflectedMethodsText);
        };

        // Add controls to new form
        this.Controls.Add(codeEditor);
        this.Controls.Add(saveButton);
        this.Controls.Add(appendButton);
        this.Controls.Add(copyReflectedMethods);


        var darkModeButton = new ToolStripMenuItem("Dark Mode")
        {
            Checked = isDarkMode,
            CheckOnClick = true
        };
        darkModeButton.CheckedChanged += InputButton_CheckedChanged;
        void InputButton_CheckedChanged(object? sender, EventArgs e)
        {
            if (darkModeButton.Checked)
            {
                ToggleDarkMode(this, true);

            }
            else
            {
                ToggleDarkMode(this, false);

            }
        }

        var settingsMenu = new ToolStripMenuItem("Settings")
        {
            DropDownItems =
            {


                darkModeButton,
            },
            BackColor = isDarkMode ? Color.FromArgb(30, 30, 30) : Color.White,
            ForeColor = isDarkMode ? Color.White : Color.Black,

        };

        var appendLineTool = new ToolStripMenuItem("AppendLine Converter");
        appendLineTool.Click += (_, _) =>
        {
            if ((Application.OpenForms["ForAppendLine"] as ForAppendLineForm) != null)
            {
                Form existingForm = Application.OpenForms["ForAppendLine"]!;
                existingForm.BringToFront();
            }
            else
            {
                ForAppendLineForm sbe = new();
                sbe.Show();
            }

        };


        var scriptBaseExtendedConverter = new ToolStripMenuItem("Method Converter");
        scriptBaseExtendedConverter.Click += (_, _) =>
        {
            if ((Application.OpenForms["ScriptBaseExtendedConverterForm"] as ScriptBaseExtendedConverterForm) != null)
            {
                Form existingForm = Application.OpenForms["ScriptBaseExtendedConverterForm"]!;
                existingForm.BringToFront();
            }
            else
            {
                ScriptBaseExtendedConverterForm sbe = new();
                sbe.Show();
            }

        };

        var toolsMenu = new ToolStripMenuItem("Tools")
        {
            DropDownItems =
            {


               appendLineTool,
               new ToolStripSeparator(),
               scriptBaseExtendedConverter,
            },
            BackColor = isDarkMode ? Color.FromArgb(30, 30, 30) : Color.White,
            ForeColor = isDarkMode ? Color.White : Color.Black,

        };
        var menuStrip = new MenuStrip
        {
            Dock = DockStyle.Top,
            Parent = this,
            BackColor = isDarkMode ? Color.FromArgb(30, 30, 30) : Color.White,
            ForeColor = isDarkMode ? Color.White : Color.Black,

            Items = { toolsMenu, settingsMenu }
        };
    }
}
