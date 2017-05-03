using System;
using System.Text;

namespace SpellWork.Extensions
{
    public static class StringBuilderExtensions
    {
        public const string DefaultFamily = "Arial Unicode MS";
        public const float  DefaultSize   = 9f;

        public static void AppendFormatLine(this StringBuilder textbox, string format, params object[] arg0)
        {
            textbox.Append(string.Format(format, arg0) + Environment.NewLine);
        }

        public static void AppendFormat(this StringBuilder textbox, string format, params object[] arg0)
        {
            textbox.Append(string.Format(format, arg0));
        }


        public static void AppendFormatLineIfNotNull(this StringBuilder builder, string format, float arg)
        {
            if (Math.Abs(arg) > 1.0E-5f)
            {
                builder.AppendFormatLine(format, arg);
            }
        }

        public static void AppendFormatLineIfNotNull(this StringBuilder builder, string format, string arg)
        {
            if (!string.IsNullOrEmpty(arg))
            {
                builder.AppendFormatLine(format, arg);
            }
        }

        public static void AppendFormatIfNotNull(this StringBuilder builder, string format, uint arg)
        {
            if (arg != 0)
            {
                builder.AppendFormat(format, arg);
            }
        }

        public static void AppendFormatIfNotNull(this StringBuilder builder, string format, float arg)
        {
            if (Math.Abs(arg) > 1.0E-5f)
            {
                builder.AppendFormat(format, arg);
            }
        }
    }
}
