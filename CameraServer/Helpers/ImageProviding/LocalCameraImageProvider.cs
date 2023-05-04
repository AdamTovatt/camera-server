using CameraServer.Models;
using Microsoft.AspNetCore.Mvc;
using OpenCvSharp;

namespace CameraServer.Helpers.ImageProviding
{
    public class LocalCameraImageProvider : ICamera
    {
        private static FrameSource? frameSource = null;
        private static Mat? mat = null;

        public async Task<CameraImage> GetImageAsync()
        {
            if (frameSource == null)
                frameSource = Cv2.CreateFrameSource_Camera(0);
            if (mat == null)
                mat = new Mat();

            await Task.Run(() => { frameSource.NextFrame(mat); });
            return new CameraImage(mat.ToBytes());
        }

        public async Task SetImage(CameraImage image)
        {
            await Task.CompletedTask;
            return; // can't set the image on a local camera
        }

        public CameraInformation GetInformation()
        {
            return new CameraInformation(0, "LocalCamera", "This is a camera that is on the server", DateTime.UtcNow);
        }
    }
}
