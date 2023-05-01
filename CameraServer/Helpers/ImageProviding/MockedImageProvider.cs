using CameraServer.Models;
using Microsoft.AspNetCore.Mvc;

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
    }
}
