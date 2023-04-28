using CameraServer.Helpers.ImageProviding;

namespace CameraServer.Helpers
{
    public interface IImageCache
    {
        public IImageProvider GetImageProvider(string providerId);
        public IImageProvider AddImageProvider(string providerId);
        public bool ContainsImageProvider(string providerId);
    }
}
