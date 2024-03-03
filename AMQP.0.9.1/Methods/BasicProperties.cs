using AMQP_0_9_1.Domain;
using AMQP_0_9_1.Extensions;
using AMQP_0_9_1.Types;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace AMQP_0_9_1.Methods
{
    /// <summary>
    /// The AMQP Basic headers class interface,
    /// spanning the union of the functionality offered by versions
    /// 0-8, 0-8qpid, 0-9 and 0-9-1 of AMQP.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Each property is readable, writable and clearable: a cleared
    /// property will not be transmitted over the wire. Properties on a
    /// fresh instance are clear by default.
    /// </para>
    /// </remarks>
    public interface IBasicProperties
    {
        /// <summary>
        /// Application Id.
        /// </summary>
        Shortstr AppId { get; set; }

        /// <summary>
        /// Intra-cluster routing identifier (cluster id is deprecated in AMQP 0-9-1).
        /// </summary>
        Shortstr ClusterId { get; set; }

        /// <summary>
        /// MIME content encoding.
        /// </summary>
        Shortstr ContentEncoding { get; set; }

        /// <summary>
        /// MIME content type.
        /// </summary>
        Shortstr ContentType { get; set; }

        /// <summary>
        /// Application correlation identifier.
        /// </summary>
        Shortstr CorrelationId { get; set; }

        /// <summary>
        /// Non-persistent (1) or persistent (2).
        /// </summary>
        Octet DeliveryMode { get; set; }

        /// <summary>
        /// Message expiration specification.
        /// </summary>
        Shortstr Expiration { get; set; }

        /// <summary>
        /// Message header field table. Is of type <see cref="IDictionary{TKey,TValue}" />.
        /// </summary>
        Table Headers { get; set; }

        /// <summary>
        /// Application message Id.
        /// </summary>
        Shortstr MessageId { get; set; }

        /// <summary>
        /// Message priority, 0 to 9.
        /// </summary>
        Octet Priority { get; set; }

        /// <summary>
        /// Destination to reply to.
        /// </summary>
        Shortstr ReplyTo { get; set; }

        /// <summary>
        /// Convenience property; parses <see cref="ReplyTo"/> property using <see cref="PublicationAddress.TryParse"/>,
        /// and serializes it using <see cref="PublicationAddress.ToString"/>.
        /// Returns null if <see cref="ReplyTo"/> property cannot be parsed by <see cref="PublicationAddress.TryParse"/>.
        /// </summary>
        PublicationAddress ReplyToAddress { get; set; }

        /// <summary>
        /// Message timestamp.
        /// </summary>
        Timestamp Timestamp { get; set; }

        /// <summary>
        /// Message type name.
        /// </summary>
        Shortstr Type { get; set; }

        /// <summary>
        /// User Id.
        /// </summary>
        Shortstr UserId { get; set; }

        int GetSize();

        void WriteTo(ByteBuffer buffer);
    }

    /// <summary>
    /// AMQP specification content header properties for content class "basic".
    /// </summary>
    public struct BasicProperties : IBasicProperties
    {
        public BasicProperties()
        {
        }

        public BasicProperties(ByteBuffer buffer)
        {
            ReadBuffer(buffer);
        }

        #region IBasicProperties
        public Shortstr ContentType { get; set; }
        public Shortstr ContentEncoding { get; set; }
        public Table Headers { get; set; }
        public Octet DeliveryMode { get; set; }
        public Octet Priority { get; set; }
        public Shortstr CorrelationId { get; set; }
        public Shortstr ReplyTo { get; set; }
        public Shortstr Expiration { get; set; }
        public Shortstr MessageId { get; set; }
        public Timestamp Timestamp { get; set; }
        public Shortstr Type { get; set; }
        public Shortstr UserId { get; set; }
        public Shortstr AppId { get; set; }
        public Shortstr ClusterId { get; set; }

        public PublicationAddress ReplyToAddress
        {
            readonly get
            {
                PublicationAddress.TryParse(ReplyTo, out PublicationAddress result);
                return result;
            }

            set { ReplyTo = value.ToString(); }
        }

        public readonly bool IsContentTypePresent() => ContentType != default;
        public readonly bool IsContentEncodingPresent() => ContentEncoding != default;
        public readonly bool IsHeadersPresent() => Headers != default;
        public readonly bool IsDeliveryModePresent() => DeliveryMode != default;
        public readonly bool IsPriorityPresent() => Priority != default;
        public readonly bool IsCorrelationIdPresent() => CorrelationId != default;
        public readonly bool IsReplyToPresent() => ReplyTo != default;
        public readonly bool IsExpirationPresent() => Expiration != default;
        public readonly bool IsMessageIdPresent() => MessageId != default;
        public readonly bool IsTimestampPresent() => Timestamp != default;
        public readonly bool IsTypePresent() => Type != default;
        public readonly bool IsUserIdPresent() => UserId != default;
        public readonly bool IsAppIdPresent() => AppId != default;
        public readonly bool IsClusterIdPresent() => ClusterId != default;

        #endregion

        //----------------------------------
        // First byte
        //----------------------------------
        internal const byte ContentTypeBit = 7;
        internal const byte ContentEncodingBit = 6;
        internal const byte HeaderBit = 5;
        internal const byte DeliveryModeBit = 4;
        internal const byte PriorityBit = 3;
        internal const byte CorrelationIdBit = 2;
        internal const byte ReplyToBit = 1;
        internal const byte ExpirationBit = 0;

        //----------------------------------
        // Second byte
        //----------------------------------
        internal const byte MessageIdBit = 7;
        internal const byte TimestampBit = 6;
        internal const byte TypeBit = 5;
        internal const byte UserIdBit = 4;
        internal const byte AppIdBit = 3;
        internal const byte ClusterIdBit = 2;

        public void WriteTo(ByteBuffer buffer)
        {
            var innerBuffer = new ByteBuffer(0);

            byte bitValue = 0;

            if (IsContentTypePresent())
            {
                SetBit(ref bitValue, ContentTypeBit);
                ContentType.WriteTo(innerBuffer);
            }

            if (IsContentEncodingPresent())
            {
                SetBit(ref bitValue, ContentEncodingBit);
                ContentEncoding.WriteTo(innerBuffer);
            }

            if (IsHeadersPresent())
            {
                SetBit(ref bitValue, HeaderBit);
                Headers.WriteTo(innerBuffer);
            }

            if (IsDeliveryModePresent())
            {
                SetBit(ref bitValue, DeliveryModeBit);
                DeliveryMode.WriteTo(innerBuffer);
            }

            if (IsPriorityPresent())
            {
                SetBit(ref bitValue, PriorityBit);
                Priority.WriteTo(innerBuffer);
            }

            if (IsCorrelationIdPresent())
            {
                SetBit(ref bitValue, CorrelationIdBit);
                CorrelationId.WriteTo(innerBuffer);
            }

            if (IsReplyToPresent())
            {
                SetBit(ref bitValue, ReplyToBit);
                ReplyTo.WriteTo(innerBuffer);
            }

            if (IsExpirationPresent())
            {
                SetBit(ref bitValue, ExpirationBit);
                Expiration.WriteTo(innerBuffer);
            }

            byte bitValue2 = 0;
            if (IsMessageIdPresent())
            {
                SetBit(ref bitValue2, MessageIdBit);
                MessageId.WriteTo(innerBuffer);
            }

            if (IsTimestampPresent())
            {
                SetBit(ref bitValue2, TimestampBit);
                Timestamp.WriteTo(innerBuffer);
            }

            if (IsTypePresent())
            {
                SetBit(ref bitValue2, TypeBit);
                Type.WriteTo(innerBuffer);
            }

            if (IsUserIdPresent())
            {
                SetBit(ref bitValue2, UserIdBit);
                UserId.WriteTo(innerBuffer);
            }

            if (IsAppIdPresent())
            {
                SetBit(ref bitValue2, AppIdBit);
                AppId.WriteTo(innerBuffer);
            }

            if (IsClusterIdPresent())
            {
                SetBit(ref bitValue2, ClusterIdBit);
                ClusterId.WriteTo(innerBuffer);
            }

            buffer.WriteBytes(new byte[] { bitValue, bitValue2 }, 0, 2);
            buffer.WriteBytes(innerBuffer.Buffer, 0, innerBuffer.Length);
        }

        public int GetSize()
        {
            int bufferSize = 2; // number of presence fields (14) in 2 bytes blocks

            if (IsContentTypePresent())
            {
                bufferSize += Shortstr.Create(ContentType).SizeOf();
            }

            if (IsContentEncodingPresent())
            {
                bufferSize += ContentEncoding.SizeOf();
            }

            if (IsHeadersPresent())
            {
                bufferSize += Headers.SizeOf();
            }

            if (IsDeliveryModePresent())
            {
                bufferSize += DeliveryMode.SizeOf();
            }

            if (IsPriorityPresent())
            {
                bufferSize += Priority.SizeOf();
            }

            if (IsCorrelationIdPresent())
            {
                bufferSize += CorrelationId.SizeOf();
            }

            if (IsReplyToPresent())
            {
                bufferSize += ReplyTo.SizeOf();
            }

            if (IsExpirationPresent())
            {
                bufferSize += Expiration.SizeOf();
            }

            if (IsMessageIdPresent())
            {
                bufferSize += MessageId.SizeOf();
            }

            if (IsTimestampPresent())
            {
                bufferSize += Timestamp.SizeOf();
            }

            if (IsTypePresent())
            {
                bufferSize += Type.SizeOf();
            }

            if (IsUserIdPresent())
            {
                bufferSize += UserId.SizeOf();
            }

            if (IsAppIdPresent())
            {
                bufferSize += AppId.SizeOf();
            }

            if (IsClusterIdPresent())
            {
                bufferSize += ClusterId.SizeOf();
            }
            return bufferSize;
        }

        #region private

        private void ReadBuffer(ByteBuffer buffer)
        {
            var bitValue = (sbyte)Octet.Create(buffer);
            var bitValue2 = Octet.Create(buffer);

            if (IsBitSet(bitValue, ContentTypeBit))
            {
                ContentType = Shortstr.Create(buffer);
            }

            if (IsBitSet(bitValue, ContentEncodingBit))
            {
                ContentEncoding = Shortstr.Create(buffer);
            }

            if (IsBitSet(bitValue, HeaderBit))
            {
                Headers = Table.Create(buffer);
            }

            if (IsBitSet(bitValue, DeliveryModeBit))
            {
                DeliveryMode = Octet.Create(buffer);
            }

            if (IsBitSet(bitValue, PriorityBit))
            {
                Priority = Octet.Create(buffer);
            }

            if (IsBitSet(bitValue, CorrelationIdBit))
            {
                CorrelationId = Shortstr.Create(buffer);
            }

            if (IsBitSet(bitValue, ReplyToBit))
            {
                ReplyTo = Shortstr.Create(buffer);
            }

            if (IsBitSet(bitValue, ExpirationBit))
            {
                Expiration = Shortstr.Create(buffer);
            }

            if (IsBitSet(bitValue2, MessageIdBit))
            {
                MessageId = Shortstr.Create(buffer);
            }

            if (IsBitSet(bitValue2, TimestampBit))
            {
                Timestamp = Timestamp.Create(buffer);
            }

            if (IsBitSet(bitValue2, TypeBit))
            {
                Type = Shortstr.Create(buffer);
            }

            if (IsBitSet(bitValue2, UserIdBit))
            {
                UserId = Shortstr.Create(buffer);
            }

            if (IsBitSet(bitValue2, AppIdBit))
            {
                AppId = Shortstr.Create(buffer);
            }

            if (IsBitSet(bitValue2, ClusterIdBit))
            {
                ClusterId = Shortstr.Create(buffer);
            }
        }

        private void SetBit(ref byte value, byte bitPosition)
        {
            value |= (byte)(1 << bitPosition);
        }

        private bool IsBitSet(sbyte value, byte bitPosition)
        {
            return (value & 1 << bitPosition) != 0;
        }

        #endregion
    }

    public class PublicationAddress
    {
        /// <summary>
        /// Regular expression used to extract the exchange-type,
        /// exchange-name and routing-key from a string.
        /// </summary>
        public static readonly Regex PSEUDO_URI_PARSER = new Regex("^([^:]+)://([^/]*)/(.*)$");

        /// <summary>
        ///  Creates a instance of the <see cref="PublicationAddress"/>.
        /// </summary>
        /// <param name="exchangeType">Exchange type.</param>
        /// <param name="exchangeName">Exchange name.</param>
        /// <param name="routingKey">Routing key.</param>
        public PublicationAddress(string exchangeType, string exchangeName, string routingKey)
        {
            ExchangeType = exchangeType;
            ExchangeName = exchangeName;
            RoutingKey = routingKey;
        }

        /// <summary>
        /// Retrieve the exchange name.
        /// </summary>
        public readonly string ExchangeName;

        /// <summary>
        /// Retrieve the exchange type string.
        /// </summary>
        public readonly string ExchangeType;

        /// <summary>
        ///Retrieve the routing key.
        /// </summary>
        public readonly string RoutingKey;

        /// <summary>
        /// Parse a <see cref="PublicationAddress"/> out of the given string,
        ///  using the <see cref="PSEUDO_URI_PARSER"/> regex.
        /// </summary>
        public static PublicationAddress Parse(string uriLikeString)
        {
            Match match = PSEUDO_URI_PARSER.Match(uriLikeString);
            if (match.Success)
            {
                return new PublicationAddress(match.Groups[1].Value,
                    match.Groups[2].Value,
                    match.Groups[3].Value);
            }
            return null;
        }

        public static bool TryParse(string uriLikeString, out PublicationAddress result)
        {
            // Callers such as IBasicProperties.ReplyToAddress
            // expect null result for invalid input.
            // The regex.Match() throws on null arguments so we perform explicit check here
            if (uriLikeString is null)
            {
                result = null;
                return false;
            }
            else
            {
                try
                {
                    PublicationAddress res = Parse(uriLikeString);
                    result = res;
                    return true;
                }
                catch
                {
                    result = null;
                    return false;
                }
            }
        }

        /// <summary>
        /// Reconstruct the "uri" from its constituents.
        /// </summary>
        public override string ToString()
        {
            return $"{ExchangeType}://{ExchangeName}/{RoutingKey}";
        }
    }

    /// <summary>
    /// Convenience enum providing compile-time names for persistent modes.
    /// </summary>
    public enum DeliveryModes : byte
    {
        /// <summary>
        /// Value for transient delivery mode (not durable).
        /// </summary>
        Transient = 1,

        /// <summary>
        /// Value for persistent delivery mode (durable).
        /// </summary>
        Persistent = 2
    }
}
