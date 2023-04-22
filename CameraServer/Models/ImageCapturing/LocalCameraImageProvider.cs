using MMALSharp.Common;
using MMALSharp.Handlers;
using MMALSharp;
using OpenCvSharp;

namespace CameraServer.Models.ImageCapturing
{
    public class LocalCameraImageProvider : IImageProvider
    {
        private static FrameSource? frameSource = null;

        public async Task<byte[]> GetImageBytes()
        {
            if (frameSource == null)
                frameSource = Cv2.CreateFrameSource_Camera(0);

            Mat mat = new Mat();
            frameSource.NextFrame(mat);

            await Task.CompletedTask;

            return mat.ToBytes();

            /*
            // Singleton initialized lazily. Reference once in your application.
            MMALCamera cam = MMALCamera.Instance;

            using (var imgCaptureHandler = new ByteArrayCaptureHandler())
            {
                await cam.TakePicture(imgCaptureHandler, MMALEncoding.JPEG, MMALEncoding.I420);
                cam.Cleanup();

                if (imgCaptureHandler.WorkingData == null)
                    throw new Exception("Working data is null");
                
                return imgCaptureHandler.WorkingData;
            }*/
        }
    }
}
