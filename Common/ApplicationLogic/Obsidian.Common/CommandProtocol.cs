using System;
using System.Collections.Generic;
using Obsidian.Cryptography.TLS;

namespace Obsidian.Common
{
    public static class CommandProtocol
    {
        const int HeaderSize = sizeof(byte) + sizeof(int); // = 5 = 1 + 4 = CommandByte + TotalLenght

        public static bool IsAuthenticationRequired(CommandID commandID)
        {
            return commandID != CommandID.PublishIdentity;
        }

        public static byte[] Serialize(this RequestCommand requestCommand, CommandHeader commandHeader)
        {
            if (commandHeader == CommandHeader.Yes)
                return Serialize(requestCommand.Contents).AddHeader(requestCommand.CommandId);
            return Serialize(requestCommand.Contents);
        }


        static byte[] Serialize(object value)
        {

            if (value == null)
                throw new ArgumentNullException(nameof(value), "Serializer does not accept null.");

            if (value is byte)
                return new[] { (byte)value };

            var s = value as string;
            if (s != null)
                return s.SerializeCore();


            var message = value as XMessage;
            if (message != null)
                return message.SerializeCore();

            var xIdentity = value as XIdentity;
            if (xIdentity != null)
                return xIdentity.SerializeXIdentity();

            var listOfMessage = value as List<XMessage>;
            if (listOfMessage != null)
                return listOfMessage.SerializeCollection(XMessageExtensions.SerializeCore);

            var listOfString = value as List<string>;
            if (listOfString != null)
                return listOfString.SerializeCollection(PocoSerializer.SerializeCore);
               
            throw new NotSupportedException($"Serialization of {value.GetType()} is not supported.");
        }
        public static Command ParseCommand(this TLSRequest tlsRequest)
        {
            var announcedLenght = BitConverter.ToInt32(tlsRequest.CommandData, 1);
            if (announcedLenght != tlsRequest.CommandData.Length)
                throw new InvalidOperationException($"According to the information in the message, length should be {announcedLenght} but actual length is {tlsRequest.CommandData.Length}.");
            var commandWithoutHeader = new byte[tlsRequest.CommandData.Length - HeaderSize];
            Buffer.BlockCopy(tlsRequest.CommandData, HeaderSize, commandWithoutHeader, 0, commandWithoutHeader.Length);
            return new Command((CommandID)tlsRequest.CommandData[0], commandWithoutHeader);
        }

       
       

       

        static byte[] AddHeader(this byte[] serializedResponse, CommandID commandID)
        {
            return CreateReplyWithHeader(commandID, serializedResponse);
        }

        static byte[] CreateReplyWithHeader(CommandID commandID, byte[] serializedResponse)
        {
            int replyLenght = Convert.ToInt32(HeaderSize + serializedResponse.Length);
            byte[] replyLenghtBytes = BitConverter.GetBytes(replyLenght);

            var replyWithHeader = new byte[replyLenght];
            replyWithHeader[0] = (byte)commandID;
            replyWithHeader[1] = replyLenghtBytes[0];
            replyWithHeader[2] = replyLenghtBytes[1];
            replyWithHeader[3] = replyLenghtBytes[2];
            replyWithHeader[4] = replyLenghtBytes[3];

            Buffer.BlockCopy(serializedResponse, 0, replyWithHeader, HeaderSize, serializedResponse.Length);
            return replyWithHeader;
        }


        public static bool IsCommandDefined(this CommandID commandID)
        {
            return Enum.IsDefined(typeof(CommandID), commandID) && commandID != CommandID.Zero;
        }
    }
}
