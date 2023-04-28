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
        public image_cache _cache = new image_cache();

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
        public async Task UpdatedImage(byte[] image, int cameraId)
        {
            image_provider imageNew = new();
            imageNew.UpdateImage(image);
            _cache.AddImage(cameraId, imageNew);
        }

        [HttpGet("get-image")]
        public async Task<FileContentResult> UpdatedImage( int cameraId)
        {
            image_provider newImage = _cache.GetImage(cameraId);
            return newImage.FileContentResultImage();
        }
    }
}