using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Data;
using System.Reflection;

namespace SpellWorkLib.Extensions
{
    public static class Extensions
    {
        /// <summary>
        /// Reads the NULL-terminated string from the current stream.
        /// </summary>
        /// <param name="reader">Stream to read from.</param>
        /// <returns>Resulting string.</returns>
        public static string ReadCString(this BinaryReader reader)
        {
            byte num;
            var temp = new List<byte>();

            while ((num = reader.ReadByte()) != 0)
            {
                temp.Add(num);
            }

            return Encoding.UTF8.GetString(temp.ToArray());
        }

        /// <summary>
        /// Reads the struct from the current stream.
        /// </summary>
        /// <typeparam name="T">Struct type.</typeparam>
        /// <param name="reader">Stream to read from.</param>
        /// <returns>Resulting struct.</returns>
        public static T ReadStruct<T>(this BinaryReader reader) where T : struct
        {
            var rawData = reader.ReadBytes(Marshal.SizeOf(typeof(T)));

            var handle = GCHandle.Alloc(rawData, GCHandleType.Pinned);
            var returnObject = (T)Marshal.PtrToStructure(handle.AddrOfPinnedObject(), typeof(T));

            handle.Free();

            return returnObject;
        }

        public static StringBuilder AppendFormatIfNotNull(this StringBuilder builder, string format, params object[] arg)
        {
            return arg[0].ToUInt32() != 0 ? builder.AppendFormat(format, arg) : builder;
        }

        // Append Format Line
        public static StringBuilder AppendFormatLine(this StringBuilder builder, string format, params object[] arg0)
        {
            return builder.AppendFormat(format, arg0).AppendLine();
        }

        public static StringBuilder AppendFormatLineIfNotNull(this StringBuilder builder, string format, int arg)
        {
            return arg != 0 ? builder.AppendFormat(format, arg).AppendLine() : builder;
        }

        public static StringBuilder AppendFormatLineIfNotNull(this StringBuilder builder, string format, uint arg)
        {
            return arg != 0 ? builder.AppendFormat(format, arg).AppendLine() : builder;
        }

        public static uint ToUInt32(this object val)
        {
            if (val == null)
                return 0;

            uint num;
            uint.TryParse(val.ToString(), out num);
            return num;
        }

        public static int ToInt32(this object val)
        {
            if (val == null)
                return 0;

            int num;
            int.TryParse(val.ToString(), out num);
            return num;
        }

        public static float ToFloat(this object val)
        {
            if (val == null)
                return 0.0f;

            float num;
            float.TryParse(val.ToString().Replace(',', '.'), out num);
            return num;
        }

        public static ulong ToUlong(this object val)
        {
            if (val == null)
                return 0U;

            ulong num;
            ulong.TryParse(val.ToString(), out num);
            return num;
        }

        public static string NormalizeString(this string text, string remove)
        {
            var str = string.Empty;
            if (remove != string.Empty)
            {
                text = text.Replace(remove, string.Empty);
            }

            foreach (var s in text.Split('_'))
            {
                var i = 0;
                foreach (var c in s.ToCharArray())
                {
                    str += i == 0 ? c.ToString().ToUpper() : c.ToString().ToLower();
                    i++;
                }
                str += " ";
            }

            return str.Remove(str.Length - 1);
        }


        /// <summary>
        /// Compares the text on the partial occurrence of a string and ignore case
        /// </summary>
        /// <param name="text">The original text, which will search entry</param>
        /// <param name="compareText">String which will be matched with the original text</param>
        /// <returns>Boolean(true or false)</returns>
        public static bool ContainsText(this string text, string compareText)
        {
            return (text.ToUpper().IndexOf(compareText.ToUpper(), StringComparison.CurrentCultureIgnoreCase) != -1);
        }

        /// <summary>
        /// Compares the text on the partial occurrence of a string and ignore case
        /// </summary>
        /// <param name="text">The original text, which will search entry</param>
        /// <param name="compareText">Array strings which will be matched with the original text</param>
        /// <returns>Boolean(true or false)</returns>
        public static bool ContainsText(this string text, string[] compareText)
        {
            return compareText.Any(str => (text.IndexOf(str, StringComparison.CurrentCultureIgnoreCase) != -1));
        }

        public static bool ContainsElement(this uint[] array, uint[] value)
        {
            return array.Length == value.Length && array.Where((t, i) => (t & value[i]) != 0).Any();
        }

        /// <summary>
        /// Checks if the specified value in a given array
        /// </summary>
        /// <param name="array">Array in which to search</param>
        /// <param name="value">Meaning Search</param>
        /// <returns>true or false</returns>
        public static bool ContainsElement(this uint[] array, uint value)
        {
            return array.Any(i => i == value);
        }

        public static bool ContainsElement(this int[] array, int value)
        {
            return array.Any(i => i == value);
        }

        public static T GetValue<T>(this Dictionary<uint, T> dictionary, uint key)
        {
            T value;
            dictionary.TryGetValue(key, out value);
            return value;
        }

        public static bool IsEmpty(this string str)
        {
            return str == string.Empty;
        }

        public static string GetFullName(this Enum @enum)
        {
            var field = @enum.GetType().GetField(@enum.ToString());
            if (field != null)
            {
                var attrs = (FullNameAttribute[])field.GetCustomAttributes(typeof(FullNameAttribute), false);

                if (attrs.Length > 0)
                    return attrs[0].FullName;
            }

            return @enum.ToString();
        }
    }

    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
    public class FullNameAttribute : Attribute
    {
        public string FullName { get; private set; }

        public FullNameAttribute(string fullName)
        {
            FullName = fullName;
        }
    }
}
