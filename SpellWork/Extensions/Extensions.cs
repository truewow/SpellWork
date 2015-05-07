using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using System.Data;
using System.Reflection;
using SpellWorkLib.Extensions;

namespace SpellWork.Extensions
{
    public static class Extensions
    {

        public static void SetCheckedItemFromFlag(this CheckedListBox name, uint value)
        {
            for (var i = 0; i < name.Items.Count; ++i)
                name.SetItemChecked(i, ((value / (1U << (i - 1))) % 2) != 0);
        }

        public static uint GetFlagsValue(this CheckedListBox name)
        {
            uint val = 0;
            for (var i = 0; i < name.CheckedIndices.Count; ++i)
                val |= 1U << (name.CheckedIndices[i]);

            return val;
        }

        public static void SetFlags<T>(this CheckedListBox clb)
        {
            clb.Items.Clear();

            foreach (var elem in Enum.GetValues(typeof(T)))
                clb.Items.Add(elem.ToString().NormalizeString(String.Empty));
        }

        public static void SetFlags<T>(this CheckedListBox clb, String remove)
        {
            clb.Items.Clear();

            foreach (var elem in Enum.GetValues(typeof(T)))
                clb.Items.Add(elem.ToString().NormalizeString(remove));
        }

        public static void SetFlags(this CheckedListBox clb, Type type, String remove)
        {
            clb.Items.Clear();

            foreach (var elem in Enum.GetValues(type))
                clb.Items.Add(elem.ToString().NormalizeString(remove));
        }

        public static void SetEnumValues<T>(this ComboBox cb, string noValue)
        {
            var dt = new DataTable();
            dt.Columns.Add("ID");
            dt.Columns.Add("NAME");

            dt.Rows.Add(new Object[] { -1, noValue });

            foreach (var str in Enum.GetValues(typeof(T)))
                dt.Rows.Add(new Object[] { (int)str, "(" + ((int)str).ToString("000") + ") " + str });

            cb.DataSource = dt;
            cb.DisplayMember = "NAME";
            cb.ValueMember = "ID";
        }

        public static void SetEnumValuesDirect<T>(this ComboBox cb, Boolean setFirstValue)
        {
            cb.BeginUpdate();

            cb.Items.Clear();
            foreach (var value in Enum.GetValues(typeof(T)))
                cb.Items.Add(((Enum)value).GetFullName());

            if (setFirstValue && cb.Items.Count > 0)
                cb.SelectedIndex = 0;

            cb.EndUpdate();
        }

        public static void SetStructFields<T>(this ComboBox cb)
        {
            cb.Items.Clear();

            var dt = new DataTable();
            dt.Columns.Add("ID", typeof(MemberInfo));
            dt.Columns.Add("NAME", typeof(String));

            var type = typeof(T).GetMembers();
            var i = 0;
            foreach (var str in type)
            {
                if (!(str is FieldInfo) && !(str is PropertyInfo))
                    continue;

                var dr = dt.NewRow();
                dr["ID"] = str;
                dr["NAME"] = String.Format("({0:000}) {1}", i, str.Name);
                dt.Rows.Add(dr);
                ++i;
            }

            cb.DataSource = dt;
            cb.DisplayMember = "NAME";
            cb.ValueMember = "ID";
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
