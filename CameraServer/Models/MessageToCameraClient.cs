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
        
        private float moveX { get; set; }
        private float moveY { get; set; }

        public MessageToCameraClient() { }

        public MessageToCameraClient(float moveX, float moveY)
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
                float moveX = BitConverter.ToSingle(BitConverter.GetBytes(System.Net.IPAddress.NetworkToHostOrder(BitConverter.ToInt32(bytes, 4))), 0);
                float moveY = BitConverter.ToSingle(BitConverter.GetBytes(System.Net.IPAddress.NetworkToHostOrder(BitConverter.ToInt32(bytes, 8))), 0);

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
