using AMQP_0_9_1.Domain;
using AMQP_0_9_1.Extensions;
using AMQP_0_9_1.Types;
using System;

namespace AMQP_0_9_1.Methods.Outgoing
{
    public class ConnectionStart : IOutgoingAmqpMethod
    {
        public ConnectionStart()
        {
            var capabilities = new Table();
            capabilities["publisher_confirms"] = true;
            capabilities["exchange_exchange_bindings"] = false;
            capabilities["basic.nack"] = true;
            capabilities["consumer_cancel_notify"] = true;
            capabilities["connection.blocked"] = false;
            capabilities["consumer_priorities"] = false;
            capabilities["authentication_failure_close"] = true;
            capabilities["per_consumer_qos"] = true;

            ServerProperties = new Table();
            ServerProperties["product"] = "AMQP_0_9_1";
            ServerProperties["version"] = "0.1";
            ServerProperties["copyright"] = "Evgeniy Prokhorov, 2024";
            ServerProperties["platform"] = Environment.OSVersion.ToString();
            ServerProperties["capabilities"] = capabilities;
            ServerProperties["host"] = Environment.MachineName;

            Mechanisms = Longstr.Create("PLAIN");
            Locales = Longstr.Create("en_US");
        }

        #region payload

        public Octet VersionMajor { get; set; } = 0;
        public Octet VersionMinor { get; set; } = 9;
        public Table ServerProperties { get; set; }
        public Longstr Mechanisms { get; set; }
        public Longstr Locales { get; set; }

        #endregion

        #region IOutgoingAmqpMethod

        public Short ClassId => Short.Create(10);

        public Short MethodId => Short.Create(10);

        public int WriteTo(ByteBuffer buffer)
        {
            var length = buffer.Length;
            ClassId.WriteTo(buffer);
            MethodId.WriteTo(buffer);
            VersionMajor.WriteTo(buffer);
            VersionMinor.WriteTo(buffer);
            ServerProperties.WriteTo(buffer);
            Mechanisms.WriteTo(buffer);
            Locales.WriteTo(buffer);
            return buffer.Length - length;
        }

        public Long GetPayloadLength()
        {
            var bufferSize = ClassId.SizeOf();
            bufferSize += MethodId.SizeOf();
            bufferSize += VersionMajor.SizeOf();
            bufferSize += VersionMinor.SizeOf();
            bufferSize += ServerProperties.SizeOf();
            bufferSize += Mechanisms.SizeOf();
            bufferSize += Locales.SizeOf();
            return Long.Create(bufferSize);
        }

        #endregion
    }
}
