using CameraServer.Models;

namespace CameraServer.Helpers.ImageProviding
{
    public class MockedImageProvider : ICamera
    {

        public async Task<CameraImage> GetImageAsync()
        {
            return new CameraImage(await EmbeddedResourceHelper.ReadResource("MockedCameraImage"));
        }

        public Task SetImage(CameraImage image)
        {
            throw new NotImplementedException();
        }

        public CameraInformation GetInformation()
        {
            return new CameraInformation(0, "MockedCamera", "This is a camera that is mocked, it doesn't exist for real", DateTime.UtcNow);
        }
    }
}
