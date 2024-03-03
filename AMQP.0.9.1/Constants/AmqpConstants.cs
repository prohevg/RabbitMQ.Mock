using System.Collections.Generic;

namespace AMQP_0_9_1.Constants
{
    public static class AmqpConstants
    {
        // FrameMethod identifier
        public const byte FrameMethod = 1;

        // FrameHeader identifier
        public const byte FrameHeader = 2;

        // FrameBody identifier
        public const byte FrameBody = 3;

        // FrameHeartbeat identifier
        public const byte FrameHeartbeat = 8;

        // FrameMinSize identifier
        public const int FrameMinSize = 4096;

        // FrameEnd identifier
        public const int FrameEnd = 206;

        // ReplySuccess identifier Indicates that the method completed successfully. This reply code is
        // reserved for future use - the current protocol design does not use positive
        // confirmation and reply codes are sent only in case of an error.
        public const int ReplySuccess = 200;

        // ContentTooLarge identifier The client attempted to transfer content larger than the server could accept
        // at the present time. The client may retry at a later time.
        public const int ContentTooLarge = 311;

        // NoConsumers identifier When the exchange cannot deliver to a consumer when the immediate flag is
        // set. As a result of pending data on the queue or the absence of any
        // consumers of the queue.
        public const int NoConsumers = 313;

        // ConnectionForced identifier An operator intervened to close the _connection for some reason. The client
        // may retry at some later date.
        public const int ConnectionForced = 320;

        // InvalidPath identifier The client tried to work with an unknown virtual _host.
        public const int InvalidPath = 402;

        // AccessRefused identifier The client attempted to work with a server entity to which it has no
        // access due to security settings.
        public const int AccessRefused = 403;

        // NotFound identifier The client attempted to work with a server entity that does not exist.
        public const int NotFound = 404;

        // ResourceLocked identifier The client attempted to work with a server entity to which it has no
        // access because another client is working with it.
        public const int ResourceLocked = 405;

        // PreconditionFailed identifier The client requested a method that was not allowed because some precondition
        // failed.
        public const int PreconditionFailed = 406;

        // FrameError identifier The sender sent a malformed frame that the recipient could not decode.
        // This strongly implies a programming error in the sending peer.
        public const int FrameError = 501;

        // SyntaxError identifier The sender sent a frame that contained illegal values for one or more
        // fields. This strongly implies a programming error in the sending peer.
        public const int SyntaxError = 502;

        // CommandInvalid identifier The client sent an invalid sequence of frames, attempting to perform an
        // operation that was considered invalid by the server. This usually implies
        // a programming error in the client.
        public const int CommandInvalid = 503;

        // ChannelError identifier The client attempted to work with a channel that had not been correctly
        // opened. This most likely indicates a fault in the client layer.
        public const int ChannelError = 504;

        // UnexpectedFrame identifier The peer sent a frame that was not expected, usually in the context of
        // a content header and body.  This strongly indicates a fault in the peer's
        // content processing.
        public const int UnexpectedFrame = 505;

        // ResourceError identifier The server could not complete the method because it lacked sufficient
        // resources. This may be due to the client creating too many of some type
        // of entity.
        public const int ResourceError = 506;

        // NotAllowed identifier The client tried to work with some entity in a manner that is prohibited
        // by the server, due to security settings or by some other criteria.
        public const int NotAllowed = 530;

        // NotImplemented identifier The client tried to use functionality that is not implemented in the
        // server.
        public const int NotImplemented = 540;

        // publicError identifier The server could not complete the method because of an public error.
        // The server may require intervention by an operator in order to resume
        // normal operations.
        public const int publicError = 541;

        // ClassConnection identifier
        public const int ClassConnection = 10;

        // MethodConnectionStart identifier
        public const int MethodConnectionStart = 10;

        // MethodConnectionStartOk identifier
        public const int MethodConnectionStartOk = 11;

        // MethodConnectionSecure identifier
        public const int MethodConnectionSecure = 20;

        // MethodConnectionSecureOk identifier
        public const int MethodConnectionSecureOk = 21;

        // MethodConnectionTune identifier
        public const int MethodConnectionTune = 30;

        // MethodConnectionTuneOk identifier
        public const int MethodConnectionTuneOk = 31;

        // MethodConnectionOpen identifier
        public const int MethodConnectionOpen = 40;

        // MethodConnectionOpenOk identifier
        public const int MethodConnectionOpenOk = 41;

        // MethodConnectionClose identifier
        public const int MethodConnectionClose = 50;

        // MethodConnectionCloseOk identifier
        public const int MethodConnectionCloseOk = 51;

        // MethodConnectionBlocked identifier
        public const int MethodConnectionBlocked = 60;

        // MethodConnectionUnblocked identifier
        public const int MethodConnectionUnblocked = 61;

        // ClassChannel identifier
        public const int ClassChannel = 20;

        // MethodChannelOpen identifier
        public const int MethodChannelOpen = 10;

        // MethodChannelOpenOk identifier
        public const int MethodChannelOpenOk = 11;

        // MethodChannelFlow identifier
        public const int MethodChannelFlow = 20;

        // MethodChannelFlowOk identifier
        public const int MethodChannelFlowOk = 21;

        // MethodChannelClose identifier
        public const int MethodChannelClose = 40;

        // MethodChannelCloseOk identifier
        public const int MethodChannelCloseOk = 41;

        // ClassExchange identifier
        public const int ClassExchange = 40;

        // MethodExchangeDeclare identifier
        public const int MethodExchangeDeclare = 10;

        // MethodExchangeDeclareOk identifier
        public const int MethodExchangeDeclareOk = 11;

        // MethodExchangeDelete identifier
        public const int MethodExchangeDelete = 20;

        // MethodExchangeDeleteOk identifier
        public const int MethodExchangeDeleteOk = 21;

        // MethodExchangeBind identifier
        public const int MethodExchangeBind = 30;

        // MethodExchangeBindOk identifier
        public const int MethodExchangeBindOk = 31;

        // MethodExchangeUnbind identifier
        public const int MethodExchangeUnbind = 40;

        // MethodExchangeUnbindOk identifier
        public const int MethodExchangeUnbindOk = 51;

        // ClassQueue identifier
        public const int ClassQueue = 50;

        // MethodQueueDeclare identifier
        public const int MethodQueueDeclare = 10;

        // MethodQueueDeclareOk identifier
        public const int MethodQueueDeclareOk = 11;

        // MethodQueueBind identifier
        public const int MethodQueueBind = 20;

        // MethodQueueBindOk identifier
        public const int MethodQueueBindOk = 21;

        // MethodQueueUnbind identifier
        public const int MethodQueueUnbind = 50;

        // MethodQueueUnbindOk identifier
        public const int MethodQueueUnbindOk = 51;

        // MethodQueuePurge identifier
        public const int MethodQueuePurge = 30;

        // MethodQueuePurgeOk identifier
        public const int MethodQueuePurgeOk = 31;

        // MethodQueueDelete identifier
        public const int MethodQueueDelete = 40;

        // MethodQueueDeleteOk identifier
        public const int MethodQueueDeleteOk = 41;

        // ClassBasic identifier
        public const int ClassBasic = 60;

        // MethodBasicQos identifier
        public const int MethodBasicQos = 10;

        // MethodBasicQosOk identifier
        public const int MethodBasicQosOk = 11;

        // MethodBasicConsume identifier
        public const int MethodBasicConsume = 20;

        // MethodBasicConsumeOk identifier
        public const int MethodBasicConsumeOk = 21;

        // MethodBasicCancel identifier
        public const int MethodBasicCancel = 30;

        // MethodBasicCancelOk identifier
        public const int MethodBasicCancelOk = 31;

        // MethodBasicPublish identifier
        public const int MethodBasicPublish = 40;

        // MethodBasicReturn identifier
        public const int MethodBasicReturn = 50;

        // MethodBasicDeliver identifier
        public const int MethodBasicDeliver = 60;

        // MethodBasicGet identifier
        public const int MethodBasicGet = 70;

        // MethodBasicGetOk identifier
        public const int MethodBasicGetOk = 71;

        // MethodBasicGetEmpty identifier
        public const int MethodBasicGetEmpty = 72;

        // MethodBasicAck identifier
        public const int MethodBasicAck = 80;

        // MethodBasicReject identifier
        public const int MethodBasicReject = 90;

        // MethodBasicRecoverAsync identifier
        public const int MethodBasicRecoverAsync = 100;

        // MethodBasicRecover identifier
        public const int MethodBasicRecover = 110;

        // MethodBasicRecoverOk identifier
        public const int MethodBasicRecoverOk = 111;

        // MethodBasicNack identifier
        public const int MethodBasicNack = 120;

        // ClassTx identifier
        public const int ClassTx = 90;

        // MethodTxSelect identifier
        public const int MethodTxSelect = 10;

        // MethodTxSelectOk identifier
        public const int MethodTxSelectOk = 11;

        // MethodTxCommit identifier
        public const int MethodTxCommit = 20;

        // MethodTxCommitOk identifier
        public const int MethodTxCommitOk = 21;

        // MethodTxRollback identifier
        public const int MethodTxRollback = 30;

        // MethodTxRollbackOk identifier
        public const int MethodTxRollbackOk = 31;

        // ClassConfirm identifier
        public const int ClassConfirm = 85;

        // MethodConfirmSelect identifier
        public const int MethodConfirmSelect = 10;

        // MethodConfirmSelectOk identifier
        public const int MethodConfirmSelectOk = 11;

        // ConstantsNameMap map for mapping error codes into error messages
        public static Dictionary<int, string> ConstantsNameMap = new Dictionary<int, string>
        {
            { 1, "FRAME_METHOD" },

            { 2, "FRAME_HEADER" },

            { 3, "FRAME_BODY" },

            { 8, "FRAME_HEARTBEAT" },

            { 4096, "FRAME_MIN_SIZE" },

            { 206, "FRAME_END" },

            { 200, "REPLY_SUCCESS" },

            { 311, "CONTENT_TOO_LARGE" },

            { 313, "NO_CONSUMERS" },

            { 320, "CONNECTION_FORCED" },

            { 402, "INVALID_PATH" },

            { 403, "ACCESS_REFUSED" },

            { 404, "NOT_FOUND" },

            { 405, "RESOURCE_LOCKED" },

            { 406, "PRECONDITION_FAILED" },

            { 501, "FRAME_ERROR" },

            { 502, "SYNTAX_ERROR" },

            { 503, "COMMAND_INVALID" },

            { 504, "CHANNEL_ERROR" },

            { 505, "UNEXPECTED_FRAME" },

            { 506, "RESOURCE_ERROR" },

            { 530, "NOT_ALLOWED" },

            { 540, "NOT_IMPLEMENTED" },

            { 541, "public_ERROR" },
        };
    }
}
