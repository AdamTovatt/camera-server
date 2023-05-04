using CameraServer.Helpers.ImageProviding;
using CameraServer.Models;

namespace CameraServer.Helpers
{
    public class Camera : ICamera
    {
        private CameraImage? currentImage;
        private CameraInformation information;

        public Camera(CameraInformation information)
        {
            this.information = information;
        }

        public async Task SetImage(CameraImage image)
        {
            currentImage = image;
            await Task.CompletedTask;
        }

        public async Task<CameraImage> GetImageAsync()
        {
            if (currentImage == null)
                throw new Exception("no image available");

            await Task.CompletedTask;
            return currentImage;
        }

        public CameraInformation GetInformation()
        {
            return information;
        }
    }
}
