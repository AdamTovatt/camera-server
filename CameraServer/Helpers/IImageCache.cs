using CameraServer.Helpers.ImageProviding;

namespace CameraServer.Helpers
{
    public interface IImageCache
    {
        public IImageProvider AddImageProvider(string providerId);
        public bool TryGetImagProvider(string providerId, out IImageProvider provider);
    }
}
