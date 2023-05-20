using Sakur.WebApiUtilities.BaseClasses;

namespace CameraServer.Models.RequestBodies
{
    public class MoveCameraBody : RequestBody
    {
        public int CameraId { get; set; }
        public int DeltaPitch { get; set; }
        public int DeltaYaw { get; set; }

        public override bool Valid => CameraId > 0;
    }
}
