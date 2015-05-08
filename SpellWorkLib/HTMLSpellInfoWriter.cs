using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Web.UI;

namespace SpellWorkLib
{
    public class HTMLSpellInfoWriter : ISpellInfoWriter
    {
        private HtmlTextWriter _writer;
        private readonly Queue<HtmlTextWriterTag> _openTags = new Queue<HtmlTextWriterTag>();

        public override string ToString()
        {
            return _writer.InnerWriter.ToString();
        }

        public HTMLSpellInfoWriter()
        {
            _writer = new HtmlTextWriter(new StringWriter());
        }

        public void AppendSeparator()
        {
            _writer.RenderBeginTag(HtmlTextWriterTag.Hr);
            _writer.RenderEndTag();

            SetDefaultStyle();
        }

        public void AppendFormatLine(string format, params object[] arg0)
        {
            _writer.WriteEncodedText(string.Format(format, arg0));
            //_writer.RenderBeginTag(HtmlTextWriterTag.Br);

            _writer.Write("<br>");

            SetDefaultStyle();
        }

        public void AppendFormat(string format, params object[] arg0)
        {
            _writer.WriteEncodedText(string.Format(format, arg0));

            SetDefaultStyle();
        }

        public void AppendLine()
        {
            _writer.RenderBeginTag(HtmlTextWriterTag.Br);
            _writer.RenderEndTag();

            SetDefaultStyle();
        }

        public void AppendLine(string text)
        {
            _writer.WriteEncodedText(text);
            _writer.RenderBeginTag(HtmlTextWriterTag.Br);
            _writer.RenderEndTag();

            SetDefaultStyle();
        }

        public void Append(object text)
        {
            _writer.WriteEncodedText(text.ToString());

            SetDefaultStyle();
        }

        public void AppendText(string text)
        {
            _writer.WriteEncodedText(text);

            SetDefaultStyle();
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
            SetDefaultStyle();

            var colorStr = color.Name;
            _writer.AddStyleAttribute(HtmlTextWriterStyle.Color, colorStr);

            switch (style)
            {
                case FontStyle.Regular:
                    _writer.AddStyleAttribute(HtmlTextWriterStyle.FontWeight, "normal");
                    break;
                case FontStyle.Bold:
                    _writer.AddStyleAttribute(HtmlTextWriterStyle.FontWeight, "bold");
                    break;
                case FontStyle.Italic:
                    _writer.AddStyleAttribute(HtmlTextWriterStyle.FontStyle, "italic");
                    break;
                case FontStyle.Underline:
                    _writer.AddStyleAttribute(HtmlTextWriterStyle.TextDecoration, "underline");
                    break;
                case FontStyle.Strikeout:
                    _writer.AddStyleAttribute(HtmlTextWriterStyle.TextDecoration, "line-through");
                    break;
            }

            _writer.RenderBeginTag(HtmlTextWriterTag.Div);
            _openTags.Enqueue(HtmlTextWriterTag.Div);
        }

        public void SetBold()
        {
            _writer.RenderBeginTag(HtmlTextWriterTag.Strong);
            _openTags.Enqueue(HtmlTextWriterTag.Strong);
        }

        public void SetDefaultStyle()
        {
            while (_openTags.Count > 0)
            {
                _openTags.Dequeue();
                _writer.RenderEndTag();
            }
        }

        public void Clear()
        {
            _writer = new HtmlTextWriter(new StringWriter());
            _openTags.Clear();
        }
    }
}
