using CameraServer.Helpers.ImageProviding;

namespace CameraServer.Helpers
{
    public class MockedImageCache : IImageCache
    {
        public IImageProvider AddImageProvider(string providerId)
        {
            throw new NotImplementedException();
        }

        public bool ContainsImageProvider(string providerId)
        {
            throw new NotImplementedException();
        }

        public IImageProvider GetImageProvider(string providerId)
        {
            throw new NotImplementedException();
        }
    }
}
