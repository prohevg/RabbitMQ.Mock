using AMQP_0_9_1.Domain;
using AMQP_0_9_1.Exceptions;
using AMQP_0_9_1.Extensions;
using System;
using System.Collections.Generic;

namespace AMQP_0_9_1.Types
{
    /// <summary>
    /// aka Table
    /// </summary>
    public class Table : Dictionary<string, object>, IAmqpType
    {
        public Table()
        {
        }

        public Table(ByteBuffer buffer)
        {
            ReadFrom(buffer);
        }

        public static Table Create(IDictionary<string, object> value)
        {
            var properties = new Table();
           
            if (value == null)
            {
                return properties;
            }

            foreach (var pair in value)
            {
                properties.Add(pair.Key, pair.Value);
            }

            return properties;
        }

        public static Table Create(ByteBuffer buffer)
        {
            var properties = new Table();
            properties.ReadFrom(buffer);
            return properties;
        }

        #region IAmqpType

        public void WriteTo(ByteBuffer buffer)
        {
            if (this is null || this.Count == 0)
            {
                Long.Empty.WriteTo(buffer);
                return;
            }

            var inner = new ByteBuffer(0);
            try
            {
                foreach (KeyValuePair<string, object> entry in this)
                {
                    Shortstr.Create(entry.Key).WriteTo(inner);
                    WriteFieldValue(inner, entry.Key, entry.Value);
                }

                Long.Create(inner.Length).WriteTo(buffer);
                buffer.WriteBytes(inner.Buffer, 0, inner.Length);
            }
            finally
            {
                inner.ReleaseReference();
            }
        }

        public void ReadFrom(ByteBuffer buffer)
        {
            var length = Long.Create(buffer);
            if (length == 0)
            {
                return;
            }

            var readToPos = buffer.Offset + length;
            while (buffer.Offset < readToPos)
            {
                var key = Shortstr.Create(buffer);
                var value = ReadFieldValue(buffer);
                this.Add(key, value);
            }
        }

        public int TypeSize => GetTypeSize();       

        #endregion

        #region private

        /*
        Rabbitmq table fields

        't' bool			boolean
        'b' int8			short-short-int
        's'	int16			short-int
        'I' int32			long-int
        'l' int64			long-long-int
        'f' float			float
        'd' double			double
        'D' Decimal			decimal-value
        'S'	[]byte			long-string
        'T' time.Time		timestamp
        'F' Table			field-table
        'V' nil				no-field
        'x' []interface{} 	field-array
        */
        private void WriteFieldValue(ByteBuffer buffer, string key, object value)
        {
            switch (value)
            {
                case string val:
                    Octet.Create('S').WriteTo(buffer);
                    Longstr.Create(val).WriteTo(buffer);
                    return;
                case bool val:
                    Octet.Create('t').WriteTo(buffer);
                    Boolean.Create(val).WriteTo(buffer);
                    return;
                case IDictionary<string, object> val:
                    Octet.Create('F').WriteTo(buffer);
                    Table.Create(val).WriteTo(buffer);
                    return;
                default:
                    throw new AmqpTypeException($"WriteField. Value of type '{value.GetType().Name}' for key '{key}' unknown");
            }
        }

        private object? ReadFieldValue(ByteBuffer buffer)
        {
            var value = Octet.Create(buffer);
            var ch = (char)(sbyte)value;

            switch (ch)
            {
                case 'S':
                    return Longstr.Create(buffer);
                case 't':
                    return Boolean.Create(buffer);
                case 'F':
                    return Table.Create(buffer);
                case 'V':
                    return null;
                case (char)108:// 'I':
                    return LongLong.Create(buffer);
                case 'T':
                    return Timestamp.Create(buffer);
                default:
                    throw new AmqpTypeException($"ReadField. Value of type '{value.GetType().Name}' for key '{value}' unknown");
            }
        }

        public int GetTypeSize()
        {
            var fieldSizeCollection = Long.Empty.SizeOf();
            var byteCount = fieldSizeCollection;

            if (this is null || this.Count == 0)
            {
                return fieldSizeCollection;
            }

            foreach (KeyValuePair<string, object> entry in this)
            {
                byteCount += AmqpTypeExtension.SizeOf(Shortstr.Create(entry.Key));
                byteCount += GetRequiredBufferSizeForObject(entry.Value); //+1 octet type data. 'S', 't' etc
            }

            return byteCount;
        }

        public int GetRequiredBufferSizeForObject(object value)
        {
            switch (value)
            {
                case null:
                    return 1;
                case string val:
                    return Octet.Create('S').SizeOf() + Longstr.Create(val).SizeOf();
                case bool val:
                    return Octet.Create('t').SizeOf() + Boolean.Create(val).SizeOf();
                case int val:
                    return Octet.Create('I').SizeOf() + Long.Create(val).SizeOf();
                case uint val:
                    return Octet.Create('I').SizeOf() + Long.Create(val).SizeOf();
                case float _:
                    throw new NotImplementedException("GetRequiredBufferSizeForObject. type='float'");
                case byte[] val:
                    throw new NotImplementedException("GetRequiredBufferSizeForObject. type='byte[]'");
                case IDictionary<string, object> val:
                    return Octet.Create('F').SizeOf() + Table.Create(val).SizeOf();
                case double _:
                    throw new NotImplementedException("GetRequiredBufferSizeForObject. type='double'");
                case long _:
                    throw new NotImplementedException("GetRequiredBufferSizeForObject. type='long'");
                case byte val:
                    return Octet.Create('b').SizeOf() + Octet.Create(val).SizeOf();
                case sbyte val:
                    return Octet.Create('b').SizeOf() + Octet.Create(val).SizeOf();
                case short val:
                    return Octet.Create('s').SizeOf() + Short.Create(val).SizeOf();
                case ushort val:
                    return Octet.Create('s').SizeOf() + Short.Create(val).SizeOf();
                default:
                    throw new AmqpTypeException($"Value of type '{value.GetType().Name}' unknown");
            }
        }

        #endregion
    }
}
