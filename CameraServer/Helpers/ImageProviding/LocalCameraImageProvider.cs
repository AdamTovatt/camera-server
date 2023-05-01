using CameraServer.Models;
using Microsoft.AspNetCore.Mvc;
using OpenCvSharp;

namespace CameraServer.Helpers.ImageProviding
{
    public class LocalCameraImageProvider : ICamera
    {
        private static FrameSource? frameSource = null;
        private static Mat? mat = null;

        public async Task<CameraImage> GetCamera()
        {
            if (frameSource == null)
                frameSource = Cv2.CreateFrameSource_Camera(0);
            if (mat == null)
                mat = new Mat();

            await Task.Run(() => { frameSource.NextFrame(mat); });
            return new CameraImage(mat.ToBytes());
        }

        public Task SetImage(byte[] image)
        {
            throw new NotImplementedException();
        }
    }
}
