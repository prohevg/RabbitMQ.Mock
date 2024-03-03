using System;

namespace AMQP_0_9_1.Transport.Settings
{
    public class ConnectionSettings
    {
        public int MaxFrameSize { get; set; } = 4096;

        public int MaxChannels { get; set; } = 2047;

        public TimeSpan Heartbeat { get; set; } = TimeSpan.FromSeconds(100);
    }
}
