using AMQP_0_9_1.Framing;
using AMQP_0_9_1.Types;
using System;
using System.Text;

namespace AMQP_0_9_1.Methods.Incoming
{
    //public class ConnectionClose : IAmqpMethod, IIncomingAmqpMethod
    //{
    //    public ushort ReplyCode { get; set; }
    //    public string ReplyText { get; set; }
    //    public ushort ClassID { get; set; }
    //    public ushort MethodID { get; set; }

    //    Octet IAmqpMethod.FrameType => throw new NotImplementedException();

    //    public ShortUint ClassId => throw new NotImplementedException();

    //    public ShortUint MethodId => throw new NotImplementedException();



    //    public override string Name()
    //    {
    //        return "ConnectionClose";
    //    }

    //    public override byte FrameType()
    //    {
    //        return 1;
    //    }

    //    public override ushort ClassIdentifier()
    //    {
    //        return 10;
    //    }

    //    public override ushort MethodIdentifier()
    //    {
    //        return 50;
    //    }

    //    public override bool Sync()
    //    {
    //        return true;
    //    }

    //    //public void Read(ByteBuffer buffer, string protoVersion)
    //    //{
    //    //    ReplyCode = ByteBufferExtension.ReadUShort(buffer);
    //    //    ////ReplyCode = this.ReadShortstr(buffer, c);

    //    //    // var c = ByteBufferExtension.ReadUByte(buffer);
    //    //    ReplyText = this.ReadShortstr(buffer, FormatCode.String8Utf8);

    //    //    //var classId = ByteBufferExtension.ReadUShort(buffer);

    //    //    //var metgodId = ByteBufferExtension.ReadUShort(buffer);
    //    //}


    //    public override int GetPayloadLength()
    //    {
    //        int bufferSize = AmqpMethodSizeExtention.GetSize(ClassIdentifier());
    //        bufferSize += AmqpMethodSizeExtention.GetSize(MethodIdentifier());
    //        bufferSize += AmqpMethodSizeExtention.GetSize(ReplyCode);
    //        bufferSize += AmqpMethodSizeExtention.GetSize(ReplyText);
    //        bufferSize += AmqpMethodSizeExtention.GetSize(ClassID);
    //        bufferSize += AmqpMethodSizeExtention.GetSize(MethodID);
    //        return bufferSize;
    //    }

    //    public override int ReadTo(ByteBuffer buffer)
    //    {
    //        ReplyCode = ByteBufferExtension.ReadUShort(buffer);
    //        ReplyText = ByteBufferExtension.ReadShortstr(buffer);
    //        ClassID = ByteBufferExtension.ReadUShort(buffer);
    //        MethodID = ByteBufferExtension.ReadUShort(buffer);
    //        return 0;

    //    }

    //    public override int WriteTo(ByteBuffer buffer)
    //    {
    //        throw new NotImplementedException();
    //    }

    //    LongUint IAmqpMethod.GetPayloadLength()
    //    {
    //        throw new NotImplementedException();
    //    }
    //}
}
