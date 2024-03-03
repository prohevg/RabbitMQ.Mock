using System.Diagnostics;

namespace AMQP_0_9_1.Transport
{
    /// <summary>
    /// Defines the traces levels. Except AmqpFrame, levels are forward inclusive.
    /// For example, Information level includes the Error and Warning levels.
    /// </summary>
    public enum AmqpTraceLevel
    {
        /// <summary>
        /// Specifies that error events should be traced.
        /// </summary>
        Error = 0x01,

        /// <summary>
        /// Specifies that warning events should be traced.
        /// </summary>
        Warning = 0x03,

        /// <summary>
        /// Specifies that informational events should be traced.
        /// </summary>
        Information = 0x07,

        /// <summary>
        /// Specifies that verbose events should be traced.
        /// </summary>
        Verbose = 0x0F,

        /// <summary>
        /// Specifies that AMQP frames should be traced (<see cref="AmqpTrace.WriteFrameNullFields"/>
        /// controls how null fields should be handled).
        /// </summary>
        Frame = 0x10,

        /// <summary>
        /// Specifies that frame buffers should be traced. Cannot be combined with other levels.
        /// </summary>
        Buffer = 0x20,

        /// <summary>
        /// Specifies that application output should be traced.
        /// </summary>
        Output = 0x80
    }

    /// <summary>
    /// The callback to invoke to _write traces.
    /// </summary>
    /// <param name="level">The trace level at which the trace event is raised.</param>
    /// <param name="format">The format string for the arguments.</param>
    /// <param name="args">The arguments attached to the trace event.</param>
    public delegate void WriteTrace(AmqpTraceLevel level, string format, params object[] args);

    /// <summary>
    /// The Trace class for writing traces.
    /// </summary>
    public static class AmqpTrace
    {
        /// <summary>
        /// Gets or sets the trace level.
        /// </summary>
        public static AmqpTraceLevel TraceLevel;

        /// <summary>
        /// Gets or sets the trace callback.
        /// </summary>
        public static WriteTrace TraceListener;

        /// <summary>
        /// Gets or sets the value that controls if null fields should be written in a frame trace.
        /// </summary>
        public static bool WriteFrameNullFields;

        /// <summary>
        /// Writes a debug trace.
        /// </summary>
        /// <param name="format">The format string.</param>
        /// <param name="args">The argument list.</param>
        [Conditional("DEBUG")]
        public static void Debug(string format, params object[] args)
        {
            if (TraceListener != null)
            {
                TraceListener(AmqpTraceLevel.Verbose, format, args);
            }
        }

        /// <summary>
        /// Writes a trace if the specified level is enabled.
        /// </summary>
        /// <param name="level">The trace level.</param>
        /// <param name="format">The content to trace.</param>
        [Conditional("TRACE")]
        public static void WriteLine(AmqpTraceLevel level, string format)
        {
            if (TraceListener != null && (level & TraceLevel) == level)
            {
                TraceListener(level, format);
            }
        }

        /// <summary>
        /// Writes a trace if the specified level is enabled.
        /// </summary>
        /// <param name="level">The trace level.</param>
        /// <param name="format">The format string.</param>
        /// <param name="arg1">The first argument.</param>
        [Conditional("TRACE")]
        public static void WriteLine(AmqpTraceLevel level, string format, object arg1)
        {
            if (TraceListener != null && (level & TraceLevel) == level)
            {
                TraceListener(level, format, arg1);
            }
        }

        /// <summary>
        /// Writes a trace if the specified level is enabled.
        /// </summary>
        /// <param name="level">The trace level.</param>
        /// <param name="format">The format string.</param>
        /// <param name="arg1">The first argument.</param>
        /// <param name="arg2">The second argument.</param>
        [Conditional("TRACE")]
        public static void WriteLine(AmqpTraceLevel level, string format, object arg1, object arg2)
        {
            if (TraceListener != null && (level & TraceLevel) == level)
            {
                TraceListener(level, format, arg1, arg2);
            }
        }

        /// <summary>
        /// Writes a trace if the specified level is enabled.
        /// </summary>
        /// <param name="level">The trace level.</param>
        /// <param name="format">The format string.</param>
        /// <param name="arg1">The first argument.</param>
        /// <param name="arg2">The second argument.</param>
        /// <param name="arg3">The third argument.</param>
        [Conditional("TRACE")]
        public static void WriteLine(AmqpTraceLevel level, string format, object arg1, object arg2, object arg3)
        {
            if (TraceListener != null && (level & TraceLevel) == level)
            {
                TraceListener(level, format, arg1, arg2, arg3);
            }
        }

        [Conditional("TRACE")]
        public static void WriteBuffer(string format, byte[] buffer, int offset, int count)
        {
            if (TraceListener != null && (AmqpTraceLevel.Buffer & TraceLevel) > 0)
            {
                TraceListener(AmqpTraceLevel.Buffer, format, GetBinaryString(buffer, offset, count));
            }
        }

#if DEBUG
        public static string GetBinaryString(byte[] buffer, int offset, int count)
        {
            const string hexChars = "0123456789ABCDEF";
            System.Text.StringBuilder sb = new System.Text.StringBuilder(count * 2);
            for (int i = offset; i < offset + count; ++i)
            {
                sb.Append(hexChars[buffer[i] >> 4]);
                sb.Append(hexChars[buffer[i] & 0x0F]);
            }

            return sb.ToString();
        }
#endif
    }
}