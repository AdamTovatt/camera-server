using CameraServer.Models;
using Microsoft.AspNetCore.Mvc;

namespace CameraServer.Helpers.ImageProviding
{
    public interface ICamera
    {
        public Task<CameraImage> GetCamera();

        public Task SetImage(byte[] image);
    }
}
