namespace CameraServer.Models
{
    public class MessageToCameraClient
    {
        public static MessageToCameraClient InvalidMessage = new MessageToCameraClient() { Type = MessageType.Invalid };

        public enum MessageType
        {
            Invalid,
            MoveInformation  
        }

        public MessageType Type { get; set; }
        
        private int moveX { get; set; }
        private int moveY { get; set; }

        public MessageToCameraClient() { }

        public MessageToCameraClient(int moveX, int moveY)
        {
            Type = MessageType.MoveInformation;
            this.moveX = moveX;
            this.moveY = moveY;
        }

        public MessageToCameraClient GetMoveMessage(int moveX, int moveY)
        {
            return new MessageToCameraClient(moveX, moveY);
        }

        public static MessageToCameraClient FromBytes(byte[] bytes)
        {
            if(bytes.Length < 4)
                return InvalidMessage;

            MessageType messageType = (MessageType)BitConverter.ToInt32(bytes, 0);

            if(messageType == MessageType.MoveInformation)
            {
                if(bytes.Length != 12)
                    return InvalidMessage;

                int moveX = BitConverter.ToInt32(bytes, 4);
                int moveY = BitConverter.ToInt32(bytes, 8);

                return new MessageToCameraClient(moveX, moveY);
            }

            return InvalidMessage;
        }

        public byte[]? GetBytes()
        {
            if(Type == MessageType.MoveInformation)
            {
                List<byte> result = new List<byte>();

                result.AddRange(BitConverter.GetBytes((int)Type));
                result.AddRange(BitConverter.GetBytes(moveX));
                result.AddRange(BitConverter.GetBytes(moveY));

                return result.ToArray();
            }

            return null; // if this happens something is wrong
        }
    }
}
