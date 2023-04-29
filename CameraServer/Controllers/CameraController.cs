using CameraServer.Helpers;
using CameraServer.Helpers.ImageProviding;
using CameraServer.Models;
using Microsoft.AspNetCore.Mvc;
using Sakur.WebApiUtilities.Models;
using System.Net;

namespace CameraServer.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CameraController : ControllerBase
    {
        private CameraContainer container = new CameraContainer();

        [HttpGet("hello")]
        public async Task<ObjectResult> Hello()
        {
            try
            {
                await Task.Delay(1);
                return new ApiResponse("Hello World!", HttpStatusCode.OK);
            }
            catch (ApiException exception)
            {
                return new ApiResponse(exception);
            }
        }

        [HttpGet("mocked-image")]
        public async Task<FileContentResult> GetImage()
        {
            MockedImageProvider imageProvider = new MockedImageProvider();
            return CameraImage.GetResponse(await imageProvider.GetImage());
        }

        [HttpGet("image")]
        public async Task<FileContentResult> GetCameraImage()
        {
            LocalCameraImageProvider imageProvider = new LocalCameraImageProvider();
            return CameraImage.GetResponse(await imageProvider.GetImage());
        }

        [HttpPost("update-image")]
        public async void UpdateCameraImage(byte[] image, int cameraId)
        {
            Camera camera = new Camera();
            await camera.UpdateImage(image);
            container.AddImage(cameraId, camera);
        }

        [HttpGet("get-image")]
        public async Task<FileContentResult> GetCameraImage(int cameraId)
        {
            ICamera camera = await container.GetImage(cameraId);
            return await camera.FileContentResultImage();
        }
    }
}