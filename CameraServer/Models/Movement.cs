namespace CameraServer.Models
{
    public class Movement
    {
        public float DeltaPitch { get; set; }
        public float DeltaYaw { get; set; }
        public bool ContainsValue { get; set; }

        public void Clear()
        {
            DeltaPitch = 0;
            DeltaYaw = 0;
            ContainsValue = false;
        }

        public byte[] GetBytes()
        {
            byte[] pitchBytes = BitConverter.GetBytes(DeltaPitch);
            byte[] yawBytes = BitConverter.GetBytes(DeltaYaw);

            byte[] byteArray = new byte[pitchBytes.Length + yawBytes.Length];
            Array.Copy(pitchBytes, 0, byteArray, 0, pitchBytes.Length);
            Array.Copy(yawBytes, 0, byteArray, pitchBytes.Length, yawBytes.Length);

            return byteArray;
        }
    }
}
