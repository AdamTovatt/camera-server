using CameraServer.Helpers;
using System.Net.WebSockets;

namespace CameraServer.Models
{
    public class Camera
    {
        public CameraInformation Information { get { return information; } }
        public bool IsConnected { get { return socketConnection != null && !socketConnection.CloseStatus.HasValue; } }

        public delegate void ImageUpdated(object sender, ArraySegment<byte> bytes);
        public event ImageUpdated? OnImageUpdated;

        private CameraImage currentImage;
        private CameraInformation information;
        private WebSocket? socketConnection;

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

            OnImageUpdated?.Invoke(this, new ArraySegment<byte>(bytes)); // invoke the OnImageUpdated event if it exists
        }

        public byte[] GetImageBytes()
        {
            return currentImage.Bytes;
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

            return TokenHelper.GetToken(information.Id) == token;
        }

        public async Task SetPreviewAsync()
        {
            Information.Preview = Convert.ToBase64String((await GetImageAsync()).Bytes);
        }

        public void SetConnection(WebSocket webSocket)
        {
            socketConnection = webSocket;
        }

        public async Task<bool> SendMessageToCameraClient(MessageToCameraClient message)
        {
            if (!IsConnected)
                return false;

            byte[]? messageBytes = message.GetBytes();

            if (messageBytes == null)
                return false;

            await socketConnection!.SendAsync(messageBytes, WebSocketMessageType.Binary, true, CancellationToken.None);
            return true;
        }

        public bool UserIsAllowed(string token)
        {
            return true; // needs to be implemented in a better way obviously
        }
    }
}
