using CameraServer.Helpers;
using System.Net.WebSockets;

namespace CameraServer.Models
{
    public class Camera
    {
        public CameraInformation Information { get { return information; } }
        public bool IsConnected { get { return socketConnection != null && !socketConnection.CloseStatus.HasValue; } }
        public Movement QueuedMovement { get { return queuedMovement; } }

        public long LastTimeOfCapture { get { return lastTimeOfCapture; } }

        public delegate void ImageUpdated(object sender, ArraySegment<byte> bytes);
        public event ImageUpdated? OnImageUpdated;

        private CameraImage currentImage;
        private CameraInformation information;
        private WebSocket? socketConnection;
        private byte[]? queuedData;
        private long lastTimeOfCapture;

        private Movement queuedMovement;

        public Camera(CameraInformation information)
        {
            this.information = information;
            currentImage = new CameraImage(new byte[0], DateTime.MinValue);
            queuedMovement = new Movement();
        }

        public void SetImage(byte[] bytes)
        {
            currentImage.Bytes = bytes;
            DateTime time = DateTime.UtcNow;
            currentImage.TimeOfCapture = time;
            information.LastActive = time;
            lastTimeOfCapture = time.Ticks;

            OnImageUpdated?.Invoke(this, new ArraySegment<byte>(bytes)); // invoke the OnImageUpdated event if it exists
        }

        public async Task<byte[]> GetNextImageBytesAsync(long previousTimeOfCapture)
        {
            while (lastTimeOfCapture == previousTimeOfCapture)
                await Task.Delay(30);

            return currentImage.Bytes;
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

            return TokenHelper.GetToken(information.Id) == information.Token;
        }

        public async Task SetPreviewAsync()
        {
            Information.Preview = Convert.ToBase64String((await GetImageAsync()).Bytes);
        }

        public void SetConnection(WebSocket webSocket)
        {
            socketConnection = webSocket;
        }

        public void QueueBytes(byte[] bytes)
        {
            queuedData = bytes;
            queuedMovement.Clear();
        }

        public void Move(float newPitch, float newYaw)
        {
            queuedMovement.Pitch = newPitch;
            queuedMovement.Yaw = newYaw;
            queuedMovement.ContainsValue = true;
        }

        private async Task<bool> SendBytesAsync(byte[] bytes)
        {
            if (!IsConnected)
                return false;

            await socketConnection!.SendAsync(new ArraySegment<byte>(bytes), WebSocketMessageType.Binary, true, CancellationToken.None);

            return true;
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

        public async Task<bool> SubmitResponse()
        {
            if (queuedData != null)
            {
                bool result = await SendBytesAsync(queuedData);
                queuedData = null;
                return result;
            }

            if (queuedMovement.ContainsValue)
            {
                queuedData = queuedMovement.GetBytes();
                queuedMovement.Clear();
                bool result = await SendBytesAsync(queuedData);
                queuedData = null;
                return result;
            }

            return await SendBytesAsync(new byte[0]);
        }

        public bool UserIsAllowed(string token)
        {
            return true; // needs to be implemented in a better way obviously
        }
    }
}
