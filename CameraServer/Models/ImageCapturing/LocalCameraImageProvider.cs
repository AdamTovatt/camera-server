using OpenCvSharp;

namespace CameraServer.Models.ImageCapturing
{
    public class LocalCameraImageProvider : IImageProvider
    {
        private static FrameSource? frameSource = null;
        private static Mat? mat = null;

        public async Task<byte[]> GetImageBytes()
        {
            if (frameSource == null)
                frameSource = Cv2.CreateFrameSource_Camera(0);
            if(mat == null)
                mat = new Mat();

            await Task.Run(() => { frameSource.NextFrame(mat); });
            return mat.ToBytes();
        }
    }
}
