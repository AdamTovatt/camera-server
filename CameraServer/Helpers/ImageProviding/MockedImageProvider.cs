using CameraServer.Models;
using Microsoft.AspNetCore.Mvc;

namespace CameraServer.Helpers.ImageProviding
{
    public class MockedImageProvider : ICamera
    {
        public Task<FileContentResult> FileContentResultImage()
        {
            throw new NotImplementedException();
        }

        public async Task<CameraImage> GetImage()
        {
            return new CameraImage(await EmbeddedResourceHelper.ReadResource("MockedCameraImage"));
        }

        public Task UpdateImage(byte[] image)
        {
            throw new NotImplementedException();
        }
    }
}
