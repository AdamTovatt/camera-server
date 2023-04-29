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
        private CameraContainer cache = new CameraContainer();

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
        
        [HttpPost("updated-image")]
        public async void UpdatedImage(byte[] image, int cameraId)
        {
            Camera imageNew = new Camera();
            imageNew.UpdateImage(image);
            cache.AddImage(cameraId, imageNew);
            await Task.CompletedTask;
        }

        [HttpGet("get-image")]
        public  async Task<FileContentResult> GetCameraImage( int cameraId)
        {
            ICamera newImage = await cache.GetImage(cameraId);
            return await newImage.FileContentResultImage();
        }
    }
}