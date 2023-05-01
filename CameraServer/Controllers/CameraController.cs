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
            return (await imageProvider.GetImageAsync()).ToResponse();
        }

        [HttpGet("image")]
        public async Task<FileContentResult> GetCameraImage()
        {
            LocalCameraImageProvider imageProvider = new LocalCameraImageProvider();
            return (await imageProvider.GetImageAsync()).ToResponse();
        }

        [HttpPost("update-image")]
        public async Task<ObjectResult> UpdateCameraImage([FromForm] IFormFile image, [FromForm] int cameraId)
        {
            if (cameraId < 1)
                return new ApiResponse("Invalid id in FormData", HttpStatusCode.BadRequest);

            using (Stream stream = image.OpenReadStream())
            {
                using (MemoryStream memoryStream = new MemoryStream((int)stream.Length))
                {
                    await stream.CopyToAsync(memoryStream);
                    await CameraContainer.Instance.SetImage(cameraId, new CameraImage(memoryStream));
                }
            }

            return new ApiResponse();
        }

        [HttpGet("get-image")]
        public async Task<IActionResult> GetCameraImage(int cameraId)
        {
            //if (cameraId < 1 && !await CameraContainer.Instance.ContainsKey(cameraId))
                //return new FileContentResult();

            ICamera camera = await CameraContainer.Instance.GetCameraAsync(cameraId);
            return (await camera.GetImageAsync()).ToResponse();
        }
    }
}