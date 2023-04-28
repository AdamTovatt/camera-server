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

        [HttpGet("mocked-image")]
        public async Task<FileContentResult> GetImage()
        {
            MockedImageProvider imageProvider = new MockedImageProvider();
            return new FileContentResult(await imageProvider.GetImageBytes(), "image/jpeg");
        }

        [HttpGet("image")]
        public async Task<FileContentResult> GetCameraImage()
        {
            LocalCameraImageProvider imageProvider = new LocalCameraImageProvider();
            return new FileContentResult(await imageProvider.GetImageBytes(), "image/jpeg");
        }

        [HttpGet("image-raw")]
        public async Task<IActionResult> GetRawImage()
        {
            LocalCameraImageProvider imageProvider = new LocalCameraImageProvider();
            byte[] imageRaw = await imageProvider.GetImageBytes();
            return File(imageRaw, "application/octet-stream");
        }
    }
}