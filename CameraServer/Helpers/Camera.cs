using CameraServer.Models;

namespace CameraServer.Helpers
{
    public class Camera
    {
        public CameraInformation Information { get { return information; } }

        private CameraImage currentImage;
        private CameraInformation information;

        public Camera(CameraInformation information)
        {
            this.information = information;
            currentImage = new CameraImage(new byte[0], DateTime.MinValue);
        }

        public void SetImage(byte[] bytes)
        {
            currentImage.Bytes = bytes;
            DateTime time = DateTime.UtcNow;
            currentImage.TimeOfCapture = time;
            information.LastActive = time;
        }

        public async Task<CameraImage> GetImageAsync()
        {
            if (currentImage == null)
                throw new Exception("no image available");

            await Task.CompletedTask;
            return currentImage;
        }

        public bool IsValidToken(string token)
        {
            if (information == null || information.Token == null)
                return false;

            return TokenHelper.GetToken(information.Id) == information.Token;
        }

        public async Task SetPreviewAsync()
        {
            Information.Preview = Convert.ToBase64String((await GetImageAsync()).Bytes);
        }
    }
}
