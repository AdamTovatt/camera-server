using CameraServer.Models;
using Microsoft.AspNetCore.Mvc;

namespace CameraServer.Helpers.ImageProviding
{
    public interface ICamera
    {
        public Task<CameraImage> GetImageAsync();

        public Task SetImage(CameraImage image);
    }
}
