using CameraServer.Helpers.ImageProviding;
using CameraServer.Models;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace CameraServer.Helpers
{
    public class Camera : ICamera
    {
        private CameraImage? CurrentImage { get; set; }

        public async Task SetImage(byte[] image)
        {
            CurrentImage = new CameraImage(image, new DateTime());
            await Task.CompletedTask;
        }

        public async Task<CameraImage> GetCamera()
        {
            if (CurrentImage == null)
                throw new Exception("no image available");

            await Task.CompletedTask;
            return CurrentImage;
        }
    }
}
