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
            try
            {
                foreach (var prop in typeof(T).GetFields(BindingFlags.Instance | BindingFlags.Public))
                {
                    if (propertyIndex >= FieldCount)
                        return structure;

                    object value;
                    if (prop.FieldType.IsArray)
                        value = BuildArrayField(byteRow, propertyIndex, prop);
                    else
                        value = BuildSimpleField(byteRow, propertyIndex, prop.FieldType, 0);

                    ++propertyIndex;
                    prop.SetValue(structure, value);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Possible error when loading record. {e}");
                return null;
            }

            return structure;
        }

        private object BuildArrayField(byte[] byteRow, int propertyIndex, FieldInfo prop)
        {
            var fieldStructure = FieldStructures[propertyIndex];
            var fieldPosition = (int)fieldStructure.Position;
            var positionOffset = 0;
            var nextMarker = propertyIndex + 1 == FieldCount
                ? RecordSize
                : FieldStructures[propertyIndex + 1].Position;
            var arraySize = (nextMarker - fieldPosition) / fieldStructure.ByteSize;

            var elementType = prop.FieldType.GetElementType();
            var instanceValue = Array.CreateInstance(elementType, arraySize);
            for (var i = 0; i < arraySize; ++i)
            {
                instanceValue.SetValue(BuildSimpleField(byteRow, propertyIndex, elementType, positionOffset), i);
                positionOffset += fieldStructure.ByteSize;
            }

            return instanceValue;
        }

        private object BuildSimpleField(byte[] byteRow, int propertyIndex, Type fieldType, int positionOffset)
        {
            var fieldStructure = FieldStructures[propertyIndex];
            var fieldPosition = (int)fieldStructure.Position + positionOffset;
            object instanceValue = null;
            switch (Type.GetTypeCode(fieldType))
            {
                case TypeCode.SByte:
                    instanceValue = (sbyte)byteRow[fieldPosition];
                    break;
                case TypeCode.Byte:
                    instanceValue = byteRow[fieldPosition];
                    break;
                case TypeCode.Int16:
                    instanceValue = BitConverter.ToInt16(byteRow, fieldPosition);
                    break;
                case TypeCode.UInt16:
                    instanceValue = BitConverter.ToUInt16(byteRow, fieldPosition);
                    break;
                case TypeCode.Int32:
                    {
                        var elementVal = 0;
                        for (var k = 0; k < fieldStructure.ByteSize; ++k)
                            elementVal |= byteRow[fieldPosition + k] << (8 * k);
                        instanceValue = elementVal;
                        break;
                    }
                case TypeCode.UInt32:
                    {
                        var elementVal = 0u;
                        for (var k = 0; k < fieldStructure.ByteSize; ++k)
                            elementVal |= (uint)(byteRow[fieldPosition + k] << (8 * k));
                        instanceValue = elementVal;
                        break;
                    }
                case TypeCode.Int64:
                    instanceValue = BitConverter.ToInt64(byteRow, fieldPosition);
                    break;
                case TypeCode.UInt64:
                    instanceValue = BitConverter.ToUInt64(byteRow, fieldPosition);
                    break;
                case TypeCode.Single:
                    instanceValue = BitConverter.ToSingle(byteRow, fieldPosition);
                    break;
                case TypeCode.String:
                {
                    if (HasInlineStrings)
                    {
                        //! TODO 7.x Implement, only Item-Sparse has this
                        instanceValue = string.Empty;
                    }
                    else
                    {
                        var stringOffset = BitConverter.ToInt32(byteRow, fieldPosition);
                        if (StringTable.ContainsKey(stringOffset))
                            instanceValue = StringTable[stringOffset];
                        else
                            instanceValue = string.Empty;
                    }
                    break;
                }
            }

            return instanceValue;
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
            var flags = reader.ReadInt16();
            var indexField = reader.ReadInt16();
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
            {
                byte[] rowBytes = reader.ReadBytes(RecordSize);
                int id;
                if (!HasNonInlineIDs)
                {
                    var idBytes = new byte[4];
                    Array.Copy(rowBytes, FieldStructures[indexField].Position, idBytes, 0, FieldStructures[indexField].ByteSize);
                    id = BitConverter.ToInt32(idBytes, 0);
                }
                else
                    id = nonInlineIDs[i];

                Add(id, BuildEntry(rowBytes));
            }

            // Populate copy table
            foreach (var kv in copyTable)
                Add(kv.Key, this[kv.Value]);
        }
    }
}
