using CameraServer.Helpers;

namespace CameraServer.Models.ImageCapturing
{
    public class MockedImageProvider : IImageProvider
    {
        public async Task<byte[]> GetImageBytes()
        {
            return await EmbeddedResourceHelper.ReadResource("MockedCameraImage");
        }
    }
}
