
using CameraServer.Helpers.ImageProviding;
using CameraServer.Models;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace CameraServer.Helpers
{
    public class image_provider : i_i_mageProvider
    {
        CameraImage ?CameraImage { get; set; }


        public CameraImage GetImage() {
            if (CameraImage == null)
                throw new Exception("no image available");

            return CameraImage;
        }

        public void UpdateImage(byte[] image ) 
        {
            this.CameraImage = new CameraImage( image, new DateTime() );
        }

        public FileContentResult FileContentResultImage()
        {
            if (CameraImage == null)
                throw new Exception("no image available");

            return CameraImage.GetResponse(CameraImage);
        }
    }
}
