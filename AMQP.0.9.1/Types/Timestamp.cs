using AMQP_0_9_1.Constants;
using AMQP_0_9_1.Domain;
using System;

namespace AMQP_0_9_1.Types
{
    public class Timestamp : BaseAmqpType<DateTime>, IAmqpType
    {
        private const long EpochTicks = 621355968000000000; // 1970-1-1 00:00:00 UTC
        private const long TicksPerMillisecond = 10000;

        #region Constructor

        private Timestamp(DateTime value)
            : base(value) 
        {
        }

        private Timestamp(ByteBuffer value) 
            : base(value)
        {
        }

        public static Timestamp Create(DateTime value)
        {
            return new Timestamp(value);
        }


        public static Timestamp Create(ByteBuffer value)
        {
            return new Timestamp(value);
        }

        public static Timestamp Empty => new Timestamp(DateTime.MinValue);

        #endregion

        #region IAmqpType

        public void WriteTo(ByteBuffer buffer)
        {
            Octet.Create(FormatCode.TimeStamp).WriteTo(buffer);
            LongLong.Create(DateTimeToTimestamp(_value)).WriteTo(buffer);
        }

        public int TypeSize => 1;

        #endregion

        #region private

        private DateTime TimestampToDateTime(long timestamp)
        {
            if (timestamp > (DateTime.MaxValue.Ticks - EpochTicks) / TicksPerMillisecond)
            {
                return DateTime.MaxValue;
            }
            else if (timestamp < (DateTime.MinValue.Ticks - EpochTicks) / TicksPerMillisecond)
            {
                return DateTime.MinValue;
            }

            return new DateTime(EpochTicks + timestamp * TicksPerMillisecond, DateTimeKind.Utc);
        }

        private long DateTimeToTimestamp(DateTime dateTime)
        {
            return (dateTime.ToUniversalTime().Ticks - EpochTicks) / TicksPerMillisecond;
        }

        #endregion

        #region override

        protected override DateTime ReadFromBuffer(ByteBuffer buffer)
        {
            var type = Octet.Create(buffer);
            if (type != FormatCode.TimeStamp)
            {
                throw new ArgumentNullException();
            }

            var date = LongLong.Create(buffer);
            return TimestampToDateTime(date);
        }

        #endregion

        public override string ToString()
        {
            return $"Timestamp={_value}";
        }
    }
}
