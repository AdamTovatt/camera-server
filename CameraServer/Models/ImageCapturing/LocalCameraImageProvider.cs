using MMALSharp.Common;
using MMALSharp.Handlers;
using MMALSharp;

namespace CameraServer.Models.ImageCapturing
{
    public class LocalCameraImageProvider : IImageProvider
    {
        public async Task<byte[]> GetImageBytes()
        {
            // Singleton initialized lazily. Reference once in your application.
            MMALCamera cam = MMALCamera.Instance;

            using (var imgCaptureHandler = new ByteArrayCaptureHandler())
            {
                await cam.TakePicture(imgCaptureHandler, MMALEncoding.JPEG, MMALEncoding.I420);
                cam.Cleanup();

                if (imgCaptureHandler.WorkingData == null)
                    throw new Exception("Working data is null");
                
                return imgCaptureHandler.WorkingData;
            }
        }
    }
}
