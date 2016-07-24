using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace SpellWork.DBC
{
    public class DB2Reader<T> : Dictionary<int, T> where T : class, new()
    {
        public int RecordCount { get; private set; }
        public int FieldCount { get; private set; }
        public int Hash { get; private set; }
        public int Build { get; private set; }
        public bool HasOffsetMap { get; private set; }
        public bool HasInlineStrings => HasOffsetMap;
        public bool HasNonInlineIDs { get; private set; }
        public int RecordSize { get; private set; }

        private Dictionary<int, string> StringTable = new Dictionary<int, string>();
        private FieldStructure[] FieldStructures;

        public string Filename { get; private set; }

        public T this[uint key] => this[(int) key];
        public bool ContainsKey(uint key) => ContainsKey((int) key);

        private T BuildEntry(byte[] byteRow)
        {
            var structure = new T();

            var propertyIndex = 0;
            foreach (var prop in typeof (T).GetFields(BindingFlags.Instance | BindingFlags.Public))
            {
                if (propertyIndex >= FieldCount)
                    return structure;

                if (prop.GetCustomAttribute<DB2Ignored>() != null)
                    continue;

                var fieldStructure = FieldStructures[propertyIndex];
                var fieldPosition = (int) fieldStructure.Position;
                var arraySize = 1;
                if (prop.FieldType.IsArray)
                {
                    var nextMarker = propertyIndex + 1 == FieldCount
                        ? RecordSize
                        : FieldStructures[propertyIndex + 1].Position;
                    arraySize = (nextMarker - fieldPosition) / fieldStructure.ByteSize;
                }

                var elementType = prop.FieldType.IsArray ? prop.FieldType.GetElementType() : prop.FieldType;
                var typeCode = Type.GetTypeCode(elementType);
                var instanceValue = Array.CreateInstance(elementType, arraySize);
                for (var i = 0; i < arraySize; ++i)
                {
                    try
                    {
                        switch (typeCode)
                        {
                            case TypeCode.SByte:
                                instanceValue.SetValue((sbyte) byteRow[fieldPosition], i);
                                break;
                            case TypeCode.Byte:
                                instanceValue.SetValue(byteRow[fieldPosition], i);
                                break;
                            case TypeCode.Int16:
                                instanceValue.SetValue((short)(byteRow[fieldPosition] | (byteRow[fieldPosition + 1] << 8)), i);
                                break;
                            case TypeCode.UInt16:
                                instanceValue.SetValue((ushort)(byteRow[fieldPosition] | (byteRow[fieldPosition + 1] << 8)), i);
                                break;
                            case TypeCode.Int32:
                            {
                                var elementVal = 0;
                                for (var k = 0; k < fieldStructure.ByteSize; ++k)
                                    elementVal |= byteRow[fieldPosition + k] << (8 * k);
                                instanceValue.SetValue(elementVal, i);
                                break;
                            }
                            case TypeCode.UInt32:
                            {
                                var elementVal = 0u;
                                for (var k = 0; k < fieldStructure.ByteSize; ++k)
                                    elementVal |= (uint) (byteRow[fieldPosition + k] << (8 * k));
                                instanceValue.SetValue(elementVal, i);
                                break;
                            }
                            case TypeCode.Int64:
                                instanceValue.SetValue(BitConverter.ToInt64(byteRow, fieldPosition), i);
                                break;
                            case TypeCode.UInt64:
                                instanceValue.SetValue(BitConverter.ToUInt64(byteRow, fieldPosition), i);
                                break;
                            case TypeCode.Single:
                                instanceValue.SetValue(BitConverter.ToSingle(byteRow, fieldPosition), i);
                                break;
                            case TypeCode.String:
                            {
                                if (HasInlineStrings)
                                {
                                    //! TODO 7.x Implement, only Item-Sparse has this
                                    instanceValue.SetValue(string.Empty, i);
                                }
                                else
                                {
                                    var stringOffset = BitConverter.ToInt32(byteRow, fieldPosition);
                                    if (!StringTable.ContainsKey(stringOffset))
                                        Console.WriteLine("Derp");
                                    instanceValue.SetValue(StringTable[stringOffset], i);
                                }
                                break;
                            }
                        }

                        fieldPosition += fieldStructure.ByteSize;
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine($"Possible error when loading record. {e}");
                        return null;
                    }
                }

                ++propertyIndex;
                prop.SetValue(structure, arraySize == 1 ? instanceValue.GetValue(0) : instanceValue);
            }

            return structure;
        }

        private class FieldStructure
        {
            // Size in bits: (32 - Size) / 8
            public ushort Size;
            // Position of the field within the record, relative to the start of the record.
            public ushort Position;

            public int ByteSize => 4 - Size / 8;
        }

        public DB2Reader(string fileName)
        {
            Open(fileName);
        }

        public DB2Reader() { }

        public void Open(string fileName)
        {
            Filename = fileName;
            using (var reader = new BinaryReader(File.Open(fileName, FileMode.Open)))
                Read(reader);
        }

        public new void Clear()
        {
            base.Clear();
            StringTable.Clear();
        }

        private void Read(BinaryReader reader)
        {
            reader.ReadInt32(); // Signature
            RecordCount = reader.ReadInt32();
            FieldCount = reader.ReadInt32(); // Arrays now count as 1
            RecordSize = reader.ReadInt32();
            var stringBlockSize = reader.ReadInt32();
            Hash = reader.ReadInt32();
            Build = reader.ReadInt32();
            var minId = reader.ReadInt32();
            var maxId = reader.ReadInt32();
            reader.ReadInt32(); // Locale
            var copyTableSize = reader.ReadInt32();
            var flags = reader.ReadInt32();
            HasOffsetMap = (flags & 0x01) != 0;
            HasNonInlineIDs = (flags & 0x04) != 0;

            // var offsetMap = new Dictionary<int, short>(maxId - minId + 1);
            var nonInlineIDs = new int[RecordCount];
            var copyTable = new Dictionary<int, int>(copyTableSize / 8);

            FieldStructures = new FieldStructure[FieldCount];
            for (var i = 0; i < FieldCount; ++i)
            {
                // ReSharper disable once UseObjectOrCollectionInitializer
                FieldStructures[i] = new FieldStructure();
                FieldStructures[i].Size = reader.ReadUInt16();
                FieldStructures[i].Position = reader.ReadUInt16();
            }

            var recordOffset = reader.BaseStream.Position;
            reader.BaseStream.Position += RecordSize * RecordCount;

            if (!HasOffsetMap)
            {
                var stringTable = reader.ReadBytes(stringBlockSize);
                var stringLength = 0;
                for (var i = 0; i < stringBlockSize; ++i)
                {
                    if (stringTable[i] == '\0')
                    {
                        StringTable[i - stringLength] = Encoding.UTF8.GetString(stringTable, i - stringLength,
                            stringLength);
                        stringLength = 0;
                        continue;
                    }
                    ++stringLength;
                }
            }
            else //! TODO: Read offset map ?!?
                reader.BaseStream.Position += (4 + 2) * (maxId - minId + 1);
            //     for (var i = 0; i < maxId - minId + 1; ++i)
            //         offsetMap[reader.ReadInt32()] = reader.ReadInt16();

            if (HasNonInlineIDs)
                for (var i = 0; i < RecordCount; ++i)
                    nonInlineIDs[i] = reader.ReadInt32();

            if (copyTableSize > 0)
                for (var i = 0; i < copyTableSize / 8; ++i)
                    copyTable[reader.ReadInt32()] = reader.ReadInt32();

            reader.BaseStream.Position = recordOffset;

            // Populate data storage now.
            for (var i = 0; i < RecordCount; ++i)
                Add(HasNonInlineIDs ? nonInlineIDs[i] : i, BuildEntry(reader.ReadBytes(RecordSize)));

            // Populate copy table
            foreach (var kv in copyTable)
                Add(kv.Key, this[kv.Value]);
        }
    }
}
