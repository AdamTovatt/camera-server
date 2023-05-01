using CameraServer.Helpers.ImageProviding;
using CameraServer.Models;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace CameraServer.Helpers
{
    public class Camera : ICamera
    {
        private CameraImage? CurrentImage;

        public async Task SetImage(CameraImage image)
        {
            CurrentImage = image;
            await Task.CompletedTask;
        }

        public async Task<CameraImage> GetImage()
        {
            if (CurrentImage == null)
                throw new Exception("no image available");

            await Task.CompletedTask;
            return CurrentImage;
        }
    }
}
