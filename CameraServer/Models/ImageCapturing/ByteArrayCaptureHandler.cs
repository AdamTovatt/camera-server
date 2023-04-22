using MMALSharp.Common;
using MMALSharp.Handlers;

namespace CameraServer.Models.ImageCapturing
{
    public class ByteArrayCaptureHandler : IOutputCaptureHandler, ICaptureHandler, IDisposable
    {
        public byte[]? WorkingData { get; set; }

        public void Process(ImageContext context)
        {
            WorkingData = context.Data;
        }

        public void PostProcess()
        {
            return;
        }

        public string TotalProcessed()
        {
            return string.Empty;
        }

        public void Dispose()
        {
            return;
        }
    }
}
