using CameraServer.Models;

namespace CameraServer.Helpers.ImageProviding
{
    public interface IImageProvider
    {
        public Task<CameraImage> GetImage();
    }
}
