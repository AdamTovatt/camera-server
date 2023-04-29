
using CameraServer.Helpers.ImageProviding;
using CameraServer.Models;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace CameraServer.Helpers
{
    public class Camera : ICamera
    {
        private CameraImage? CameraImage { get; set; }

        public async Task UpdateImage(byte[] image)
        {
            this.CameraImage = new CameraImage(image, new DateTime());
            await Task.CompletedTask;
        }

        public async Task<FileContentResult> FileContentResultImage()
        {
            if (CameraImage == null)
                throw new Exception("no image available");

            await Task.CompletedTask;
            return CameraImage.GetResponse(CameraImage);
        }

        public async Task<CameraImage> GetImage()
        {
            if (CameraImage == null)
                throw new Exception("no image available");

            await Task.CompletedTask;
            return CameraImage;
        }
    }
}
