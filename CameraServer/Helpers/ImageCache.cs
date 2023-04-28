using CameraServer.Helpers.ImageProviding;

namespace CameraServer.Helpers
{
    public class ImageCache : IImageCache
    {
        public IImageProvider AddImageProvider(string providerId)
        {
            throw new NotImplementedException();
        }

        public bool TryGetImagProvider(string providerId, out IImageProvider provider)
        {
            throw new NotImplementedException();
        }
    }
}
