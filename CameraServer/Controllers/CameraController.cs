using CameraServer.Models.ImageCapturing;
using Microsoft.AspNetCore.Authorization;
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

        [HttpGet("image")]
        public async Task<FileContentResult> getImage()
        {
            MockedImageProvider imageProvider = new MockedImageProvider();
            return new FileContentResult(await imageProvider.GetImageBytes(), "image/jpeg");
        }

        [HttpGet("camera-image")]
        public async Task<FileContentResult> GetCameraImage()
        {
            MockedImageProvider imageProvider = new MockedImageProvider();
            return new FileContentResult(await imageProvider.GetImageBytes(), "image/jpeg");
        }
    }
}