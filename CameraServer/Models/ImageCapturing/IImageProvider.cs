namespace CameraServer.Models.ImageCapturing
{
    public interface IImageProvider
    {
        public Task<byte[]> GetImageBytes();
    }
}
