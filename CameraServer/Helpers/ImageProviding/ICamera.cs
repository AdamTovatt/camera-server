using CameraServer.Models;
using Microsoft.AspNetCore.Mvc;

namespace CameraServer.Helpers.ImageProviding
{
    public interface ICamera
    {
        public Task<CameraImage> GetImage();

        public Task SetImage(byte[] image);
    }
}
