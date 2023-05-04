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

        [HttpGet("list")]
        public async Task<ObjectResult> GetCameraList()
        {
            await Task.CompletedTask;
            return new ApiResponse(CameraContainer.Instance.GetCameraList());
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
            if (cameraId < 1 && !await CameraContainer.Instance.ContainsKey(cameraId))
                return new ApiResponse("Could not find the picture", HttpStatusCode.BadRequest);

            ICamera camera = await CameraContainer.Instance.GetCameraAsync(cameraId);
            return (await camera.GetImageAsync()).ToResponse();
        }

        [HttpGet("stream-image")]
        public async Task StreamImage(int cameraId)
        {
            Response.Headers.Add("Content-Type", "text/event-stream");
            Response.Headers.Add("Cache-Control", "no-cache");
            Response.Headers.Add("Connection", "keep-alive");

            while (!Response.HttpContext.RequestAborted.IsCancellationRequested)
            {
                byte[] imageData = (await (await CameraContainer.Instance.GetCameraAsync(cameraId)).GetImageAsync()).Bytes;
                string imageDataString = Convert.ToBase64String(imageData);

                string data = $"data:image/jpg;base64,{imageDataString}";
                string eventString = $"event: image-update\ndata: {data}\n\n";

                await Response.WriteAsync(eventString);
                await Response.Body.FlushAsync();

                await Task.Delay(200);
            }
        }
    }
}