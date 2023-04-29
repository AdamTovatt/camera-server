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
            return (await imageProvider.GetImage()).ToResponse();
        }

        [HttpGet("image")]
        public async Task<FileContentResult> GetCameraImage()
        {
            LocalCameraImageProvider imageProvider = new LocalCameraImageProvider();
            return (await imageProvider.GetImage()).ToResponse();
        }

        [HttpPost("update-image")]
        public async void UpdateCameraImage(byte[] image, int cameraId)
        {
            await CameraContainer.Instance.SetImage(cameraId, image);
        }

        [HttpGet("get-image")]
        public async Task<FileContentResult> GetCameraImage(int cameraId)
        {
            ICamera camera = await CameraContainer.Instance.GetImage(cameraId);
            return (await camera.GetImage()).ToResponse();
        }
    }
}