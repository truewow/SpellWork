using System.Drawing;
using System.Windows.Forms;
using SpellWorkLib.Extensions;

namespace SpellWorkLib
{
    public class RichTextBoxSpellInfoWriter : ISpellInfoWriter
    {
        public RichTextBoxSpellInfoWriter(RichTextBox rtb)
        {
            _rtb = rtb;
        }

        private readonly RichTextBox _rtb;
        private const string Line = "=================================================";

        public void AppendSeparator()
        {
            _rtb.AppendFormatLine(Line);
        }

        public void AppendFormatLine(string format, params object[] arg0)
        {
            _rtb.AppendFormatLine(format, arg0);
        }

        public void AppendFormat(string format, params object[] arg0)
        {
            _rtb.AppendFormat(format, arg0);
        }

        public void AppendLine()
        {
            _rtb.AppendLine();
        }

        public void AppendLine(string text)
        {
            _rtb.AppendLine(text);
        }

        public void Append(object text)
        {
            _rtb.Append(text);
        }

        public void AppendText(string text)
        {
            _rtb.AppendText(text);
        }

        public void AppendFormatLineIfNotNull(string format, uint arg)
        {
            if (arg != 0)
                AppendFormatLine(format, arg);
        }

        public void AppendFormatLineIfNotNull(string format, float arg)
        {
            if (arg != 0.0f)
                AppendFormatLine(format, arg);
        }

        public void AppendFormatLineIfNotNull(string format, string arg)
        {
            if (!string.IsNullOrEmpty(arg))
                AppendFormatLine(format, arg);
        }

        public void AppendFormatIfNotNull(string format, uint arg)
        {
            if (arg != 0)
                AppendFormat(format, arg);
        }

        public void AppendFormatIfNotNull(string format, float arg)
        {
            if (arg != 0.0f)
                AppendFormat(format, arg);
        }

        public void SetStyle(Color color, FontStyle style)
        {
            _rtb.SetStyle(color, style);
        }

        public void SetBold()
        {
            _rtb.SetBold();
        }

        public void SetDefaultStyle()
        {
            _rtb.SetDefaultStyle();
        }

        public void Clear()
        {
            _rtb.Clear();
        }
    }
}
