using System;
using System.Drawing;
using System.Windows.Forms;

namespace SpellWorkLib.Extensions
{
    public static class RichTextBoxExtensions
    {
        public const string DefaultFamily = "Arial Unicode MS";
        public const float  DefaultSize   = 9f;

        public static void AppendFormatLine(this RichTextBox textbox, string format, params object[] arg0)
        {
            textbox.AppendText(string.Format(format, arg0) + Environment.NewLine);
        }

        public static void AppendFormat(this RichTextBox textbox, string format, params object[] arg0)
        {
            textbox.AppendText(string.Format(format, arg0));
        }

        public static void AppendLine(this RichTextBox textbox)
        {
            textbox.AppendText(Environment.NewLine);
        }

        public static void AppendLine(this RichTextBox textbox, string text)
        {
            textbox.AppendText(text + Environment.NewLine);
        }

        public static void Append(this RichTextBox textbox, object text)
        {
            textbox.AppendText(text.ToString());
        }

        public static void SetStyle(this RichTextBox textbox, Color color, FontStyle style)
        {
            textbox.SelectionColor = color;
            textbox.SelectionFont = new Font(DefaultFamily, DefaultSize, style);
        }

        public static void SetBold(this RichTextBox textbox)
        {
            textbox.SelectionFont = new Font(DefaultFamily, DefaultSize, FontStyle.Bold);
        }

        public static void SetDefaultStyle(this RichTextBox textbox)
        {
            textbox.SelectionFont = new Font(DefaultFamily, DefaultSize, FontStyle.Regular);
            textbox.SelectionColor = Color.Black;
        }
    }
}
