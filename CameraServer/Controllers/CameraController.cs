using CameraServer.Helpers;
using CameraServer.Repositories;
using Microsoft.AspNetCore.Mvc;
using Sakur.WebApiUtilities.Models;
using System.Net;

namespace CameraServer.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CameraController : ControllerBase
    {
        private ICameraRepository cameraRepository;

        public CameraController(ICameraRepository cameraRepository)
        {
            this.cameraRepository = cameraRepository;
        }

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
            if (!CameraContainer.Instance.IsInitialized)
                await CameraContainer.Instance.InitializeFromRepository(cameraRepository);

            return new ApiResponse(await CameraContainer.Instance.GetCameraListAsync());
        }

        [HttpPost("update-image")]
        public async Task<ObjectResult> UpdateCameraImage([FromForm] IFormFile image, [FromForm] int cameraId, [FromForm] string token)
        {
            if (cameraId < 1)
                return new ApiResponse("Invalid id in FormData", HttpStatusCode.BadRequest);

            Camera? camera = null;

            if (!CameraContainer.Instance.IsInitialized)
                await CameraContainer.Instance.InitializeFromRepository(cameraRepository);

            if (!CameraContainer.Instance.TryGetCamera(cameraId, out camera) || camera == null)
                return new ApiResponse($"No camera with id {cameraId}", HttpStatusCode.BadRequest);

            if (!camera.IsValidToken(token))
                return new ApiResponse("Invalid token", HttpStatusCode.Unauthorized);

            using (Stream stream = image.OpenReadStream())
            {
                using (MemoryStream memoryStream = new MemoryStream((int)stream.Length))
                {
                    await stream.CopyToAsync(memoryStream);
                    camera.SetImage(memoryStream.ToArray());
                }
            }

            return new ApiResponse();
        }

        [HttpGet("get-image")]
        public async Task<IActionResult> GetCameraImage(int cameraId)
        {
            try
            {
                if (cameraId < 1 && !await CameraContainer.Instance.ContainsKey(cameraId))
                    return new ApiResponse("Could not find the picture", HttpStatusCode.BadRequest);

                if (!CameraContainer.Instance.IsInitialized)
                    await CameraContainer.Instance.InitializeFromRepository(cameraRepository);

                Camera camera = await CameraContainer.Instance.GetCameraAsync(cameraId);
                return (await camera.GetImageAsync()).ToResponse();
            }
            catch (ApiException exception)
            {
                return new ApiResponse(exception);
            }
        }

        [HttpGet("stream-image")]
        public async Task StreamImage(int cameraId, int updateDelay = 0)
        {
            if (updateDelay <= 0)
                return;

            Response.Headers.Add("Content-Type", "text/event-stream");
            Response.Headers.Add("Cache-Control", "no-cache");
            Response.Headers.Add("Connection", "keep-alive");

            if (!CameraContainer.Instance.IsInitialized)
                await CameraContainer.Instance.InitializeFromRepository(cameraRepository);

            while (!Response.HttpContext.RequestAborted.IsCancellationRequested)
            {
                byte[] imageData = (await (await CameraContainer.Instance.GetCameraAsync(cameraId)).GetImageAsync()).Bytes;
                string imageDataString = Convert.ToBase64String(imageData);

                string data = $"data:image/jpg;base64,{imageDataString}";
                string eventString = $"event: image-update\ndata: {data}\n\n";

                await Response.WriteAsync(eventString);
                await Response.Body.FlushAsync();

                await Task.Delay(updateDelay);
            }
        }
    }
}