﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Reflection;

#if FAMISTUDIO_WINDOWS
    using RenderTheme = FamiStudio.Direct2DTheme;
#else
    using RenderTheme = FamiStudio.GLTheme;
#endif

namespace FamiStudio
{
    public enum PropertyType
    {
        String,
        ColoredString,
        NumericUpDown,
        DomainUpDown,
        Slider,
        CheckBox,
        DropDownList,
        CheckBoxList,
        ColorPicker,
        Label,
        Button,
        MultilineString,
        ProgressBar,
        Radio
    };

    public enum CommentType
    {
        Good,
        Warning,
        Error
    };

    public partial class PropertyPage : UserControl
    {
        public delegate void ButtonPropertyClicked(PropertyPage props, int propertyIndex);
        public delegate void ListClicked(PropertyPage props, int propertyIndex, int itemIndex, int columnIndex);
        public delegate string SliderFormatText(double value);

        private static Bitmap[] warningIcons;

        class Property
        {
            public PropertyType type;
            public Label label;
            public Control control;
            public int leftMarging;
            public ButtonPropertyClicked click;
            public ListClicked listDoubleClick;
            public ListClicked listRightClick;
            public SliderFormatText sliderFormat;
            public PictureBox warningIcon;
        };

        private int layoutHeight;
        private Font font;
        private Bitmap colorBitmap;
        private List<Property> properties = new List<Property>();
        private object userData;
        private int advancedPropertyStart = -1;
        private bool showWarnings = false;

        public delegate void PropertyChangedDelegate(PropertyPage props, int idx, object value);
        public event PropertyChangedDelegate PropertyChanged;
        public delegate void PropertyWantsCloseDelegate(int idx);
        public event PropertyWantsCloseDelegate PropertyWantsClose;

        public int LayoutHeight => layoutHeight;
        public int PropertyCount => properties.Count;
        public object UserData { get => userData; set => userData = value; }
        public bool HasAdvancedProperties { get => advancedPropertyStart > 0; }
        public bool ShowWarnings { get => showWarnings; set => showWarnings = value; }

        [DllImport("user32.dll")]
        static extern bool HideCaret(IntPtr hWnd);

        [DllImport("user32.dll")]
        private static extern long ShowScrollBar(IntPtr hwnd, int wBar, bool bShow);
        private int SB_HORZ = 0;
        //private int SB_VERT = 1;
        //private int SB_BOTH = 3;

        public PropertyPage()
        {
            InitializeComponent();

            // Happens in design mode
            try
            {
                font = new Font(PlatformUtils.PrivateFontCollection.Families[0], 10.0f, FontStyle.Regular);
            }
            catch
            {
            }

            if (warningIcons ==  null)
            {
                string suffix = Direct2DTheme.DialogScaling > 1 ? "@2x" : "";
                
                warningIcons = new Bitmap[3];
                warningIcons[0] = Image.FromStream(Assembly.GetExecutingAssembly().GetManifestResourceStream($"FamiStudio.Resources.WarningGood{suffix}.png"))   as Bitmap;
                warningIcons[1] = Image.FromStream(Assembly.GetExecutingAssembly().GetManifestResourceStream($"FamiStudio.Resources.WarningYellow{suffix}.png")) as Bitmap;
                warningIcons[2] = Image.FromStream(Assembly.GetExecutingAssembly().GetManifestResourceStream($"FamiStudio.Resources.Warning{suffix}.png"))       as Bitmap;
            }
        }

        private int GetPropertyIndexForControl(Control ctrl)
        {
            for (int i = 0; i < properties.Count; i++)
            {
                if (properties[i].control == ctrl)
                {
                    return i;
                }
            }

            return -1;
        }

        private unsafe Bitmap GetColorBitmap()
        {
            if (colorBitmap == null)
            {
                colorBitmap = new Bitmap(ThemeBase.CustomColors.GetLength(0), ThemeBase.CustomColors.GetLength(1));
                var data = colorBitmap.LockBits(new Rectangle(0, 0, colorBitmap.Width, colorBitmap.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
                byte* ptr = (byte*)data.Scan0.ToPointer();

                for (int j = 0; j < colorBitmap.Height; j++)
                {
                    for (int i = 0; i < colorBitmap.Width; i++)
                    {
                        var color = ThemeBase.CustomColors[i, j];

                        ptr[i * 4 + 0] = color.B;
                        ptr[i * 4 + 1] = color.G;
                        ptr[i * 4 + 2] = color.R;
                        ptr[i * 4 + 3] = 255;
                    }

                    ptr += data.Stride;
                }

                colorBitmap.UnlockBits(data);
            }

            return colorBitmap;
        }

        private Label CreateLabel(string str, string tooltip = null, bool multiline = false)
        {
            Debug.Assert(!string.IsNullOrEmpty(str));

            var label = new Label();

            label.Text = str;
            label.Font = font;
            label.AutoSize = true;
            label.ForeColor = ThemeBase.LightGreyFillColor2;
            label.BackColor = BackColor;
            if (multiline)
                label.MaximumSize = new Size(1000, 0);
            toolTip.SetToolTip(label, tooltip);

            return label;
        }

        private Label CreateLinkLabel(string str, string url, string tooltip = null)
        {
            var label = new LinkLabel();

            label.Text = str;
            label.Font = font;
            label.LinkColor = ThemeBase.LightGreyFillColor1;
            label.Links.Add(0, str.Length, url);
            label.LinkClicked += Label_LinkClicked;
            label.AutoSize = true;
            label.ForeColor = ThemeBase.LightGreyFillColor2;
            label.BackColor = BackColor;
            toolTip.SetToolTip(label, tooltip);

            return label;
        }

        private void Label_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            var link = sender as LinkLabel;
            Utils.OpenUrl(link.Links[0].LinkData as string);
        }

        private TextBox CreateColoredTextBox(string txt, Color backColor)
        {
            var textBox = new TextBox();

            textBox.Text = txt;
            textBox.Font = font;
            textBox.BackColor = backColor;

            return textBox;
        }

        private TextBox CreateTextBox(string txt, int maxLength, string tooltip = null)
        {
            var textBox = new TextBox();

            textBox.Text = txt;
            textBox.Font = font;
            textBox.MaxLength = maxLength;
            toolTip.SetToolTip(textBox, tooltip);

            return textBox;
        }

        private TextBox CreateMultilineTextBox(string txt)
        {
            var textBox = new TextBox();

            textBox.Font = new Font(PlatformUtils.PrivateFontCollection.Families[0], 8.0f, FontStyle.Regular);
            textBox.Text = txt;
            textBox.BackColor = ThemeBase.DarkGreyFillColor1;
            textBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            textBox.ForeColor = ThemeBase.LightGreyFillColor2;
            textBox.Location = new System.Drawing.Point(5, 5);
            textBox.Multiline = true;
            textBox.ReadOnly = true;
            textBox.Height = (int)(300 * RenderTheme.DialogScaling);
            textBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            textBox.Select(0, 0);
            textBox.GotFocus += TextBox_GotFocus;

            return textBox;
        }

        private void TextBox_GotFocus(object sender, EventArgs e)
        {
            HideCaret((sender as TextBox).Handle);
        }

        private PictureBox CreateColorPickerPictureBox(Color color)
        {
            var pictureBox = new NoInterpolationPictureBox();
            var bmp = GetColorBitmap();

            pictureBox.Image = bmp;
            pictureBox.Height = (int)Math.Round(Width * (bmp.Height / (float)bmp.Width));
            pictureBox.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBox.BorderStyle = BorderStyle.FixedSingle;
            pictureBox.BackColor = color;
            pictureBox.MouseDown += PictureBox_MouseDown;
            pictureBox.MouseMove += PictureBox_MouseMove;
            pictureBox.MouseDoubleClick += PictureBox_MouseDoubleClick;

            return pictureBox;
        }

        private PictureBox CreatePictureBox(Bitmap bmp)
        {
            var pictureBox = new PictureBox();

            pictureBox.Image = bmp;
            pictureBox.Width  = (int)(bmp.Width  * RenderTheme.DialogScaling);
            pictureBox.Height = (int)(bmp.Height * RenderTheme.DialogScaling);
            pictureBox.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox.BorderStyle = BorderStyle.None;

            return pictureBox;
        }

        private void ChangeColor(PictureBox pictureBox, int x, int y)
        {
            int i = Math.Min(ThemeBase.CustomColors.GetLength(0) - 1, Math.Max(0, (int)(x / (float)pictureBox.Width  * ThemeBase.CustomColors.GetLength(0))));
            int j = Math.Min(ThemeBase.CustomColors.GetLength(1) - 1, Math.Max(0, (int)(y / (float)pictureBox.Height * ThemeBase.CustomColors.GetLength(1))));

            foreach (var prop in properties)
            {
                if (prop.type == PropertyType.ColoredString)
                {
                    prop.control.BackColor = ThemeBase.CustomColors[i, j];
                }
            }

            pictureBox.BackColor = ThemeBase.CustomColors[i, j];
        }

        private void PictureBox_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
                ChangeColor(sender as PictureBox, e.X, e.Y);

            PropertyWantsClose?.Invoke(GetPropertyIndexForControl(sender as Control));
        }

        private void PictureBox_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
                ChangeColor(sender as PictureBox, e.X, e.Y);
        }

        private void PictureBox_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
                ChangeColor(sender as PictureBox, e.X, e.Y);
        }

        private NumericUpDown CreateNumericUpDown(int value, int min, int max, string tooltip = null)
        {
            var upDown = new NumericUpDown();

            upDown.Font = font;
            upDown.Minimum = min;
            upDown.Maximum = max;
            upDown.Text = value.ToString();
            upDown.ValueChanged += UpDown_ValueChanged;
            toolTip.SetToolTip(upDown, tooltip);

            return upDown;
        }

        private ProgressBar CreateProgressBar(float value)
        {
            var progress = new ProgressBar();

            progress.Font = font;
            progress.Minimum = 0;
            progress.Maximum = 1000;
            progress.Value = (int)Math.Round(value * 1000);

            return progress;
        }

        private RadioButton CreateRadioButton(string text, bool check)
        {
            var radio = new RadioButton();

            radio.Font = font;
            radio.ForeColor = ThemeBase.LightGreyFillColor2;
            radio.Text = text;
            radio.AutoSize = false;
            radio.Checked = check;

            return radio;
        }

        private void UpDown_ValueChanged(object sender, EventArgs e)
        {
            int idx = GetPropertyIndexForControl(sender as Control);
            PropertyChanged?.Invoke(this, idx, GetPropertyValue(idx));
        }

        private DomainUpDown CreateDomainUpDown(int[] values, int value)
        {
            var upDown = new DomainUpDown();

            upDown.Items.AddRange(values);
            upDown.SelectedItem = value;
            upDown.Font = font;

            return upDown;
        }

        private CheckBox CreateCheckBox(bool value, string text = "", string tooltip = null)
        {
            var cb = new CheckBox();

            cb.Text = text;
            cb.Checked = value;
            cb.Font = font;
            cb.ForeColor = ThemeBase.LightGreyFillColor2;
            cb.CheckedChanged += Cb_CheckedChanged;
            toolTip.SetToolTip(cb, tooltip);

            return cb;
        }

        private void Cb_CheckedChanged(object sender, EventArgs e)
        {
            int idx = GetPropertyIndexForControl(sender as Control);
            PropertyChanged?.Invoke(this, idx, GetPropertyValue(idx));
        }

        private ComboBox CreateDropDownList(string[] values, string value, string tooltip = null)
        {
            var cb = new ComboBox();

            cb.DropDownStyle = ComboBoxStyle.DropDownList;
            cb.Items.AddRange(values);
            cb.Text = value;
            cb.Font = font;
            cb.Enabled = values.Length > 0;
            cb.SelectedIndexChanged += Cb_SelectedIndexChanged;
            toolTip.SetToolTip(cb, tooltip);

            return cb;
        }

        private void Cb_SelectedIndexChanged(object sender, EventArgs e)
        {
            int idx = GetPropertyIndexForControl(sender as Control);
            PropertyChanged?.Invoke(this, idx, GetPropertyValue(idx));
        }

        private CheckedListBox CreateCheckedListBox(string[] values, bool[] selected)
        {
            var listBox = new PaddedCheckedListBox();

            for (int i = 0; i < values.Length; i++)
                listBox.Items.Add(values[i], selected != null ? selected[i] : true);

            listBox.IntegralHeight = false;
            listBox.Font = font;
            listBox.Height = (int)(200 * RenderTheme.DialogScaling);
            listBox.CheckOnClick = true;
            listBox.SelectionMode = SelectionMode.One;

            return listBox;
        }

        private Button CreateButton(string text, string tooltip)
        {
            var button = new Button();
            button.Text = text;
            button.Click += TextBox_Click;
            button.FlatStyle = FlatStyle.Flat;
            button.Font = font;
            button.ForeColor = ThemeBase.LightGreyFillColor2;
            button.Height = (int)(32 * RenderTheme.DialogScaling);
            toolTip.SetToolTip(button, tooltip);
            return button;
        }

        private void TextBox_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < properties.Count; i++)
            {
                if (properties[i].control == sender)
                {
                    properties[i].click(this, i);
                }
            }
        }

        public void UpdateCheckBoxList(int idx, string[] values, bool[] selected)
        {
            var listBox = (properties[idx].control as PaddedCheckedListBox);

            listBox.Items.Clear();
            for (int i = 0; i < values.Length; i++)
                listBox.Items.Add(values[i], selected != null ? selected[i] : true);
        }

        public void UpdateCheckBoxList(int idx, bool[] selected)
        {
            var listBox = (properties[idx].control as PaddedCheckedListBox);

            Debug.Assert(selected.Length == listBox.Items.Count);
            for (int i = 0; i < listBox.Items.Count; i++)
                listBox.SetItemChecked(i, selected[i]);
        }

        public int AddColoredString(string value, Color color)
        {
            properties.Add(
                new Property()
                {
                    type = PropertyType.ColoredString,
                    control = CreateColoredTextBox(value, color)
                });
            return properties.Count - 1;
        }

        public int AddString(string label, string value, int maxLength = 0, string tooltip = null)
        {
            properties.Add(
                new Property()
                {
                    type = PropertyType.String,
                    label = label != null ? CreateLabel(label, tooltip) : null,
                    control = CreateTextBox(value, maxLength, tooltip)
                });
            return properties.Count - 1;
        }

        public int AddMultilineString(string label, string value)
        {
            properties.Add(
                new Property()
                {
                    type = PropertyType.MultilineString,
                    label = label != null ? CreateLabel(label) : null,
                    control = CreateMultilineTextBox(value)
                });
            return properties.Count - 1;
        }

        public int AddButton(string label, string value, ButtonPropertyClicked clickDelegate, string tooltip = null)
        {
            properties.Add(
                new Property()
                {
                    type = PropertyType.Button,
                    label = label != null ? CreateLabel(label, tooltip) : null,
                    control = CreateButton(value, tooltip),
                    click = clickDelegate
                });
            return properties.Count - 1;
        }

        public int AddLabel(string label, string value, bool multiline = false, string tooltip = null)
        {
            properties.Add(
                new Property()
                {
                    type = PropertyType.Label,
                    label = label != null ? CreateLabel(label, tooltip) : null,
                    control = CreateLabel(value, tooltip, multiline)
                });
            return properties.Count - 1;
        }

        public int AddLinkLabel(string label, string value, string url, string tooltip = null)
        {
            properties.Add(
                new Property()
                {
                    type = PropertyType.Label,
                    label = label != null ? CreateLabel(label, tooltip) : null,
                    control = CreateLinkLabel(value, url, tooltip)
                });
            return properties.Count - 1;
        }

        public int AddColorPicker(Color color)
        {
            properties.Add(
                new Property()
                {
                    type = PropertyType.ColorPicker,
                    control = CreateColorPickerPictureBox(color)
                });
            return properties.Count - 1;
        }

        public int AddIntegerRange(string label, int value, int min, int max, string tooltip = null)
        {
            properties.Add(
                new Property()
                {
                    type = PropertyType.NumericUpDown,
                    label = label != null ? CreateLabel(label, tooltip) : null,
                    control = CreateNumericUpDown(value, min, max, tooltip)
                });
            return properties.Count - 1;
        }

        public int AddProgressBar(string label, float value)
        {
            properties.Add(
                new Property()
                {
                    type = PropertyType.ProgressBar,
                    label = label != null ? CreateLabel(label) : null,
                    control = CreateProgressBar(value)
                });
            return properties.Count - 1;
        }

        public int AddRadioButton(string label, string text, bool check)
        {
            properties.Add(
                new Property()
                {
                    type = PropertyType.Radio,
                    label = label != null ? CreateLabel(label) : null,
                    control = CreateRadioButton(text, check)
                });
            return properties.Count - 1;
        }

        public void UpdateIntegerRange(int idx, int min, int max)
        {
            var upDown = (properties[idx].control as NumericUpDown);

            upDown.Minimum = min;
            upDown.Maximum = max;
        }

        public void UpdateIntegerRange(int idx, int value, int min, int max)
        {
            var upDown = (properties[idx].control as NumericUpDown);

            upDown.Minimum = min;
            upDown.Maximum = max;
            upDown.Value = value;
        }

        public void AddDomainRange(string label, int[] values, int value)
        {
            properties.Add(
                new Property()
                {
                    type = PropertyType.DomainUpDown,
                    label = label != null ? CreateLabel(label) : null,
                    control = CreateDomainUpDown(values, value)
                });
        }

        public void UpdateDomainRange(int idx, int[] values, int value)
        {
            var upDown = (properties[idx].control as DomainUpDown);

            upDown.Items.Clear();
            upDown.Items.AddRange(values);
            upDown.Text = " "; // Workaround refresh bug.
            upDown.SelectedItem = value;
        }

        public void SetLabelText(int idx, string text)
        {
            (properties[idx].control as Label).Text = text;
        }

        public void SetDropDownListIndex(int idx, int selIdx)
        {
            (properties[idx].control as ComboBox).SelectedIndex = selIdx;
        }

        public void UpdateDropDownListItems(int idx, string[] values)
        {
            var cb = (properties[idx].control as ComboBox);
            var selectedIdx = cb.SelectedIndex;

            cb.Items.Clear();
            cb.Items.AddRange(values);

            if (selectedIdx < values.Length)
                cb.SelectedIndex = selectedIdx;
            else
                cb.SelectedIndex = 0;
        }

        public void AddCheckBox(string label, bool value, string tooltip = null)
        {
            properties.Add(
                new Property()
                {
                    type = PropertyType.CheckBox,
                    label = label != null ? CreateLabel(label, tooltip) : null,
                    control = CreateCheckBox(value, "", tooltip)
                });
        }

        public int AddLabelCheckBox(string label, bool value, int margin = 0)
        {
            properties.Add(
                new Property()
                {
                    type = PropertyType.CheckBox,
                    control = CreateCheckBox(value, label),
                    leftMarging = margin
                });
            return properties.Count - 1;
        }

        public int AddDropDownList(string label, string[] values, string value, string tooltip = null)
        {
            properties.Add(
                new Property()
                {
                    type = PropertyType.DropDownList,
                    label = label != null ? CreateLabel(label, tooltip) : null,
                    control = CreateDropDownList(values, value, tooltip)
                });
            return properties.Count - 1;
        }

        public int AddCheckBoxList(string label, string[] values, bool[] selected)
        {
            properties.Add(
                new Property()
                {
                    type = PropertyType.CheckBoxList,
                    label = label != null ? CreateLabel(label) : null,
                    control = CreateCheckedListBox(values, selected)
                });
            return properties.Count - 1;
        }

        private Slider CreateSlider(double value, double min, double max, double increment, int numDecimals, string tooltip = null)
        {
            var slider = new Slider(value, min, max, increment, numDecimals);
            slider.FormatValueEvent += Slider_FormatValueEvent;
            slider.ValueChangedEvent += Slider_ValueChangedEvent; // MATTT : Linux too!
            slider.Font = font;
            return slider;
        }

        private void Slider_ValueChangedEvent(Slider slider, double value)
        {
            var idx = GetPropertyIndexForControl(slider);
            PropertyChanged?.Invoke(this, idx, value);
        }

        private string Slider_FormatValueEvent(Slider slider, double value)
        {
            var idx = GetPropertyIndexForControl(slider);

            if (idx >= 0 && properties[idx].sliderFormat != null)
                return properties[idx].sliderFormat(value);

            return null;
        }

        public int AddSlider(string label, double value, double min, double max, double increment, int numDecimals, SliderFormatText format = null, string tooltip = null)
        {
            properties.Add(
                new Property()
                {
                    type = PropertyType.Slider,
                    label = label != null ? CreateLabel(label, tooltip) : null,
                    control = CreateSlider(value, min, max, increment, numDecimals, tooltip),
                    sliderFormat = format
                });
            return properties.Count - 1;
        }

        private ListView CreateListView(string[] columnNames, string[,] data)
        {
            var list = new ListView();

            foreach (var col in columnNames)
            {
                var header = list.Columns.Add(col);
                header.Width = -2; // Auto size.
            }

            if (data != null)
            {
                for (int i = 0; i < data.GetLength(0); i++)
                {
                    var item = list.Items.Add(data[i, 0]);
                    for (int j = 1; j < data.GetLength(1); j++)
                        item.SubItems.Add(data[i, j]);
                }
            }

            list.Font = font;
            list.Height = (int)(300 * RenderTheme.DialogScaling);
            list.MultiSelect = false;
            list.View = View.Details;
            list.GridLines = true;
            list.FullRowSelect = true;
            list.MouseDoubleClick += ListView_MouseDoubleClick;
            list.MouseDown += ListView_MouseDown;
            list.BackColor = ThemeBase.LightGreyFillColor2;

            ShowScrollBar(list.Handle, SB_HORZ, false);

            return list;
        }

        private void ListView_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                var listView = sender as ListView;
                var hitTest = listView.HitTest(e.Location);

                if (hitTest.Item != null)
                {
                    for (int i = 0; i < properties.Count; i++)
                    {
                        if (properties[i].control == sender && properties[i].listRightClick != null)
                        {
                            properties[i].listRightClick(this, i, hitTest.Item.Index, hitTest.Item.SubItems.IndexOf(hitTest.SubItem));
                        }
                    }
                }
            }
        }

        private void ListView_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            var listView = sender as ListView;
            var hitTest = listView.HitTest(e.Location);

            if (hitTest.Item != null)
            {
                for (int i = 0; i < properties.Count; i++)
                {
                    if (properties[i].control == sender && properties[i].listDoubleClick != null)
                    {
                        properties[i].listDoubleClick(this, i, hitTest.Item.Index, hitTest.Item.SubItems.IndexOf(hitTest.SubItem));
                    }
                }
            }
        }
        
        public void AddMultiColumnList(string[] columnNames, string[,] data, ListClicked doubleClick, ListClicked rightClick)
        {
            properties.Add(
                new Property()
                {
                    type = PropertyType.CheckBoxList,
                    control = CreateListView(columnNames, data),
                    listDoubleClick = doubleClick,
                    listRightClick = rightClick
                });
        }

        public void UpdateMultiColumnList(int idx, string[,] data, string[] columnNames = null)
        {
            var list = properties[idx].control as ListView;

            for (int i = 0; i < data.GetLength(0); i++)
            {
                if (i >= list.Items.Count)
                {
                    var item = list.Items.Add(data[i, 0]);
                    for (int j = 1; j < data.GetLength(1); j++)
                        item.SubItems.Add(data[i, j]);
                }
                else
                {
                    var item = list.Items[i];
                    for (int j = 0; j < data.GetLength(1); j++)
                        item.SubItems[j].Text = data[i, j];
                }
            }

            while (list.Items.Count > data.GetLength(0)) 
            {
                list.Items.RemoveAt(data.GetLength(0));
            }

            if (columnNames != null)
            {
                Debug.Assert(list.Columns.Count == columnNames.Length);
                for (int i = 0; i < list.Columns.Count; i++)
                {
                    var header = list.Columns[i] as ColumnHeader;
                    header.Text  = columnNames[i];
                }
            }

            for (int i = 0; i < list.Columns.Count; i++)
            {
                var header = list.Columns[i] as ColumnHeader;
                header.Width = -2;
            }
        }

        public void SetPropertyEnabled(int idx, bool enabled)
        {
            var label = properties[idx].control as Label;

            if (label != null)
            {
                label.ForeColor = enabled ? ThemeBase.LightGreyFillColor2 : ThemeBase.MediumGreyFillColor1;
            }
            else
            {
                properties[idx].control.Enabled = enabled;
            }
        }

        public void AppendText(int idx, string line)
        {
            var textBox = properties[idx].control as TextBox;
            textBox.AppendText(line + "\r\n");
            textBox.SelectionStart = textBox.Text.Length - 1;
            textBox.SelectionLength = 0;
            textBox.ScrollToCaret();
            textBox.Focus();
        }

        public void BeginAdvancedProperties()
        {
            advancedPropertyStart = properties.Count;
        }

        public void SetPropertyWarning(int idx, CommentType type, string comment)
        {
            var prop = properties[idx];

            if (prop.warningIcon == null)
                prop.warningIcon = CreatePictureBox(warningIcons[(int)type]);
            else
                prop.warningIcon.Image = warningIcons[(int)type];

            prop.warningIcon.Width  = (int)(16 * RenderTheme.DialogScaling);
            prop.warningIcon.Height = (int)(16 * RenderTheme.DialogScaling);
            prop.warningIcon.Visible = !string.IsNullOrEmpty(comment);
            toolTip.SetToolTip(prop.warningIcon, comment);
        }

        public object GetPropertyValue(int idx)
        {
            var prop = properties[idx];

            switch (prop.type)
            {
                case PropertyType.String:
                case PropertyType.ColoredString:
                case PropertyType.MultilineString:
                    return (prop.control as TextBox).Text;
                case PropertyType.NumericUpDown:
                    return (int)(prop.control as NumericUpDown).Value;
                case PropertyType.DomainUpDown:
                    return int.TryParse(prop.control.Text, out var val) ? val : 0;
                case PropertyType.Slider:
                    return (prop.control as Slider).Value;
                case PropertyType.Radio:
                    return (prop.control as RadioButton).Checked;
                case PropertyType.CheckBox:
                    return (prop.control as CheckBox).Checked;
                case PropertyType.ColorPicker:
                    return (prop.control as PictureBox).BackColor;
                case PropertyType.DropDownList:
                    return (prop.control as ComboBox).Text;
                case PropertyType.CheckBoxList:
                    {
                        var listBox = prop.control as CheckedListBox;
                        var selected = new bool[listBox.Items.Count];
                        for (int i = 0; i < listBox.Items.Count; i++)
                            selected[i] = listBox.GetItemChecked(i);
                        return selected;
                    }
                case PropertyType.Button:
                    return (prop.control as Button).Text;
            }

            return null;
        }

        public T GetPropertyValue<T>(int idx)
        {
            return (T)GetPropertyValue(idx);
        }

        public int GetSelectedIndex(int idx)
        {
            var prop = properties[idx];

            switch (prop.type)
            {
                case PropertyType.DropDownList:
                    return (prop.control as ComboBox).SelectedIndex;
            }

            return -1;
        }

        public void SetPropertyValue(int idx, object value)
        {
            var prop = properties[idx];

            switch (prop.type)
            {
                case PropertyType.CheckBox:
                    (prop.control as CheckBox).Checked = (bool)value;
                    break;
                case PropertyType.Button:
                    (prop.control as Button).Text = (string)value;
                    break;
                case PropertyType.MultilineString:
                    (prop.control as TextBox).Text = (string)value;
                    break;
                case PropertyType.ProgressBar:
                    (prop.control as ProgressBar).Value = (int)Math.Round((float)value * 1000);
                    break;
                case PropertyType.Slider:
                    (prop.control as Slider).Value = (double)value;
                    break;
            }
        }
        
        private int GetRadioButtonHeight(string text, int width)
        {
            var testLabel = CreateLabel(text, null, true);

            testLabel.MaximumSize = new Size(width - (int)(16 * RenderTheme.DialogScaling), 0);
            Controls.Add(testLabel);
            var height = testLabel.Height + (int)(8 * RenderTheme.DialogScaling);
            Controls.Remove(testLabel);

            return height;
        }

        public void Build(bool advanced = false)
        {
            SuspendLayout();

            int margin = (int)(5 * RenderTheme.DialogScaling);
            int maxLabelWidth = 0;
            int defaultLabelHeight = 24;

            // Workaround scaling issue with checkboxes. 
            // Measure a label and well use this for checkbox.
            if (RenderTheme.DialogScaling > 1.0f)
            {
                Label testLabel = CreateLabel("888");
                Controls.Add(testLabel);
                defaultLabelHeight = Math.Max(defaultLabelHeight, testLabel.Height) + 2;
                Controls.Remove(testLabel);
            }

            var propertyCount = advanced || advancedPropertyStart < 0 ? properties.Count : advancedPropertyStart;

            for (int i = 0; i < propertyCount; i++)
            {
                var prop = properties[i];

                if (prop.label != null)
                {
                    // This is really ugly. We cant measure the labels unless they are added.
                    Controls.Add(prop.label);
                    maxLabelWidth = Math.Max(maxLabelWidth, prop.label.Width);
                    Controls.Remove(prop.label);
                }
            }

            int widthNoMargin = Width - (margin * 2);
            int totalHeight = margin;
            int warningWidth = showWarnings ? (int)(16 * RenderTheme.DialogScaling) + margin : 0;

            for (int i = 0; i < propertyCount; i++)
            {
                var prop = properties[i];
                var height = 0;

                // Hack for checkbox that dont scale with Hi-DPI. 
                if (RenderTheme.DialogScaling > 1.0f && prop.control is CheckBox)
                {
                    prop.control.Height = defaultLabelHeight;
                    if (prop.label != null)
                        prop.label.Height = defaultLabelHeight;
                }

                if (prop.label != null)
                {
                    prop.label.Left    = margin;
                    prop.label.Top     = totalHeight;
                    //prop.label.Width   = widthNoMargin / 2;

                    prop.control.Left  = maxLabelWidth + margin;
                    prop.control.Top   = totalHeight;
                    prop.control.Width = widthNoMargin - maxLabelWidth - warningWidth;

                    Controls.Add(prop.label);
                    Controls.Add(prop.control);

                    height = prop.label.Height;
                }
                else
                {
                    prop.control.Left  = margin + prop.leftMarging;
                    prop.control.Top   = totalHeight;
                    prop.control.Width = widthNoMargin;

                    // HACK : For some multiline controls.
                    if (prop.control is Label && prop.control.MaximumSize.Width != 0)
                        prop.control.MaximumSize = new Size(prop.control.Width, 0);
                    else if (prop.control is RadioButton)
                        prop.control.Height = GetRadioButtonHeight(prop.control.Text, prop.control.Width);

                    Controls.Add(prop.control);
                }

                height = Math.Max(prop.control.Height, height);

                if (prop.warningIcon != null)
                {
                    prop.warningIcon.Top  = totalHeight + (height - prop.warningIcon.Height) / 2;
                    prop.warningIcon.Left = widthNoMargin + margin + margin - warningWidth;
                    Controls.Add(prop.warningIcon);
                }

                totalHeight += height + margin;
            }

            Height = totalHeight;
            layoutHeight = totalHeight;
            ResumeLayout();
        }
    }
}
