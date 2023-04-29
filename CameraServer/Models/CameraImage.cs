using Microsoft.AspNetCore.Mvc;

namespace CameraServer.Models
{
    public class CameraImage
    {
        public byte[] Bytes { get; set; }
        public DateTime TimeOfCapture { get; set; }

        public CameraImage(byte[] bytes, DateTime timeOfCapture)
        {
            Bytes = bytes;
            TimeOfCapture = timeOfCapture;
        }

        public CameraImage(byte[] bytes)
        {
            Bytes = bytes;
            TimeOfCapture = DateTime.Now;
        }

        public FileContentResult ToResponse()
        {
            return new FileContentResult(Bytes, "image/jpeg");
        }
    }
}
