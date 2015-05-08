using System.Drawing;

namespace SpellWorkLib
{
    public interface ISpellInfoWriter
    {
        void AppendSeparator();
        void AppendFormatLine(string format, params object[] arg0);
        void AppendFormat(string format, params object[] arg0);
        void AppendLine();
        void AppendLine(string text);
        void Append(object text);
        void AppendText(string text);
        void AppendFormatLineIfNotNull(string format, uint arg);
        void AppendFormatLineIfNotNull(string format, float arg);
        void AppendFormatLineIfNotNull(string format, string arg);
        void AppendFormatIfNotNull(string format, uint arg);
        void AppendFormatIfNotNull(string format, float arg);
        void SetStyle(Color color, FontStyle style);
        void SetBold();
        void SetDefaultStyle();
        void Clear();
    }
}
