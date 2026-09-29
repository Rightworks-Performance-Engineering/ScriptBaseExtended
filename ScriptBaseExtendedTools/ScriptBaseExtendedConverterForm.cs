using FastColoredTextBoxNS;
using static Helpers;

public class ScriptBaseExtendedConverterForm : Form
{
    private FastColoredTextBox vsiTextBox;
    private FastColoredTextBox scriptBaseExtendedTextBox;
    private Button convertButton;
    private Button convertExtendedButton;
    private Label vsiTextBoxLabel;
    private Label scriptBaseExtendedTextBoxLabel;
    private TableLayoutPanel layoutPanel;
    private Panel buttonPanel;
    private TrackBar fontSizeSlider;
    public ScriptBaseExtendedConverterForm()
    {
        // Form properties
        this.Text = "Method Converter";
        this.Size = new Size(800, 450);
        this.MinimumSize = new Size(600, 350);
        this.StartPosition = FormStartPosition.CenterScreen;
        this.Name = "ScriptBaseExtendedConverterForm";

        // Load dark mode preference from registry
        bool isDarkMode = LoadDarkModePreference();
        this.BackColor = isDarkMode ? Color.Black : Color.LightGray;

        // Main Layout Panel for scaling
        layoutPanel = new TableLayoutPanel()
        {
            Dock = DockStyle.Fill,
            ColumnCount = 3,
            RowCount = 4 // Added row for slider/checkbox/dark mode
        };

        layoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 47));
        layoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 140));
        layoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 47));

        layoutPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 30));
        layoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 70));
        layoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 20));
        layoutPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 50));

        // Labels
        vsiTextBoxLabel = new Label()
        {
            Text = "LoginVSI",
            Dock = DockStyle.Fill,
            TextAlign = ContentAlignment.MiddleCenter,
            ForeColor = isDarkMode ? Color.White : Color.Black
        };

        scriptBaseExtendedTextBoxLabel = new Label()
        {
            Text = "ScriptBaseExtended",
            Dock = DockStyle.Fill,
            TextAlign = ContentAlignment.MiddleCenter,
            ForeColor = isDarkMode ? Color.White : Color.Black
        };

        // Initialize FastColoredTextBox for LoginVSI (Left editor)
        vsiTextBox = CreateCodeEditor(isDarkMode);

        // Initialize FastColoredTextBox for ScriptBaseExtended (Right editor)
        scriptBaseExtendedTextBox = CreateCodeEditor(isDarkMode);

        // Convert Button
        convertButton = new Button()
        {
            Text = "=>",
            Width = 50,
            Height = 40,
            ForeColor = isDarkMode ? Color.White : Color.Black,
            BackColor = Color.Gray
        };

        convertButton.Click += (sender, e) =>
        {
            scriptBaseExtendedTextBox.Text = ConvertMethod(vsiTextBox.Text);
            vsiTextBox.Clear();
        };

        // Convert Extended Button
        convertExtendedButton = new Button()
        {
            Text = "<=",
            Width = 50,
            Height = 40,
            ForeColor = isDarkMode ? Color.White : Color.Black,
            BackColor = Color.Gray
        };

        convertExtendedButton.Click += (sender, e) =>
        {
            vsiTextBox.Text = ConvertExtendedMethod(scriptBaseExtendedTextBox.Text);
            scriptBaseExtendedTextBox.Clear();
        };

        // Buttons Panel (Centered dynamically)
        buttonPanel = new Panel() { Dock = DockStyle.Fill };
        buttonPanel.Controls.Add(convertButton);
        buttonPanel.Controls.Add(convertExtendedButton);
        buttonPanel.Resize += (sender, e) =>
        {
            convertButton.Location = new Point((buttonPanel.Width - convertButton.Width) / 2, (buttonPanel.Height - convertButton.Height) / 2 - 20);
            convertExtendedButton.Location = new Point((buttonPanel.Width - convertExtendedButton.Width) / 2, (buttonPanel.Height - convertExtendedButton.Height) / 2 + 20);
        };
        // Font Size Slider
        fontSizeSlider = new TrackBar()
        {
            Minimum = 4,
            Maximum = 24,
            Value = 10, // Default font size
            TickFrequency = 2,
            Dock = DockStyle.Fill
        };

        // Update font size when slider moves                                                                                  
        fontSizeSlider.ValueChanged += (sender, e) =>
        {
            vsiTextBox.Font = new Font("Consolas", fontSizeSlider.Value);
            scriptBaseExtendedTextBox.Font = new Font("Consolas", fontSizeSlider.Value);
        };

        // Bottom Control Panel (Slider, Word Wrap & Dark Mode)
        Panel bottomControlsPanel = new() { Dock = DockStyle.Fill };

        bottomControlsPanel.Controls.Add(fontSizeSlider);


        layoutPanel.Controls.Add(vsiTextBoxLabel, 0, 0);
        layoutPanel.Controls.Add(scriptBaseExtendedTextBoxLabel, 2, 0);
        layoutPanel.Controls.Add(vsiTextBox, 0, 1);
        layoutPanel.Controls.Add(scriptBaseExtendedTextBox, 2, 1);
        layoutPanel.Controls.Add(buttonPanel, 1, 1);
        layoutPanel.Controls.Add(bottomControlsPanel, 0, 3);
        layoutPanel.SetColumnSpan(bottomControlsPanel, 3);
        this.Controls.Add(layoutPanel);
    }
}

