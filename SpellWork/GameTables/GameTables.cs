using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace SpellWork.GameTables
{
    public static class GameTable<T> where T : GameTableRecord
    {
        private static Dictionary<int, T> _records { get; } = new Dictionary<int, T>();

        public static T GetRecord(int key) => _records[key];

        public static void Open(string fileName)
        {
            using (var reader = new StreamReader(fileName))
            {
                var lineIndex = 0;
                while (true)
                {
                    var line = reader.ReadLine();
                    if (line == null)
                        break;

                    var lineTokens = line.Split(new[] { '\t' }, StringSplitOptions.RemoveEmptyEntries);

                    if (lineIndex == 0)
                        continue;

                    var record = (T)Activator.CreateInstance(typeof(T));
                    var fieldIndex = 0;
                    foreach (var field in typeof (T).GetFields(BindingFlags.Public | BindingFlags.Instance))
                    {
                        if (Type.GetTypeCode(field.FieldType) == TypeCode.UInt32)
                            field.SetValue(record, uint.Parse(lineTokens[fieldIndex]));
                        else if (Type.GetTypeCode(field.FieldType) == TypeCode.Int32)
                            field.SetValue(record, int.Parse(lineTokens[fieldIndex]));
                        else if (Type.GetTypeCode(field.FieldType) == TypeCode.Single)
                            field.SetValue(record, float.Parse(lineTokens[fieldIndex]));
                        else
                            throw new ArgumentOutOfRangeException();

                        ++fieldIndex;
                    }

                    _records.Add(int.Parse(lineTokens[0]), record);
                    ++lineIndex;
                }
            }
        }
    }

    public interface GameTableRecord
    {
        uint GetColumnForClass(int columnIndex);
    }
}
