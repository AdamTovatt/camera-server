namespace CameraServer.Models
{
    public class Movement
    {
        public float Pitch { get; set; }
        public float Yaw { get; set; }
        public bool ContainsValue { get; set; }

        public void Clear()
        {
            Pitch = 0;
            Yaw = 0;
            ContainsValue = false;
        }

        public byte[] GetBytes()
        {
            byte[] pitchBytes = BitConverter.GetBytes(Pitch);
            byte[] yawBytes = BitConverter.GetBytes(Yaw);

            byte[] byteArray = new byte[pitchBytes.Length + yawBytes.Length];
            Array.Copy(pitchBytes, 0, byteArray, 0, pitchBytes.Length);
            Array.Copy(yawBytes, 0, byteArray, pitchBytes.Length, yawBytes.Length);

            return byteArray;
        }
    }
}
