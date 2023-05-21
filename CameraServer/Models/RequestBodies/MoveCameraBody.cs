using Sakur.WebApiUtilities.BaseClasses;

namespace CameraServer.Models.RequestBodies
{
    public class MoveCameraBody : RequestBody
    {
        public int CameraId { get; set; }
        public float Pitch { get; set; }
        public float Yaw { get; set; }

        public override bool Valid => CameraId > 0;
    }
}
