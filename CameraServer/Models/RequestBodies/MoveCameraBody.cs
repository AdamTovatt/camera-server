using Sakur.WebApiUtilities.BaseClasses;

namespace CameraServer.Models.RequestBodies
{
    public class MoveCameraBody : RequestBody
    {
        public int CameraId { get; set; }
        public float DeltaPitch { get; set; }
        public float DeltaYaw { get; set; }

        public override bool Valid => CameraId > 0;
    }
}
