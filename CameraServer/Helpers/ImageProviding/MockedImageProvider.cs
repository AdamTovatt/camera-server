using CameraServer.Models;

namespace CameraServer.Helpers.ImageProviding
{
    public class MockedImageProvider : IImageProvider
    {
        public async Task<CameraImage> GetImage()
        {
            return new CameraImage(await EmbeddedResourceHelper.ReadResource("MockedCameraImage"));
        }
    }
}
