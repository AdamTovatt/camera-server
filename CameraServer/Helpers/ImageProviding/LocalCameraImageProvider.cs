﻿using CameraServer.Models;
using Microsoft.AspNetCore.Mvc;
using OpenCvSharp;

namespace CameraServer.Helpers.ImageProviding
{
    public class LocalCameraImageProvider : ICamera
    {
        private static FrameSource? frameSource = null;
        private static Mat? mat = null;

        public async Task<CameraImage> GetImageAsync()
        {
            if (frameSource == null)
                frameSource = Cv2.CreateFrameSource_Camera(0);
            if (mat == null)
                mat = new Mat();

            await Task.Run(() => { frameSource.NextFrame(mat); });
            return new CameraImage(mat.ToBytes());
        }

        public Task SetImage(CameraImage image)
        {
            throw new NotImplementedException();
        }
    }
}
